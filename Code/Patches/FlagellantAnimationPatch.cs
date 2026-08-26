using Flagellant.Code.Abstract;
using Flagellant.Code.Audio;
using Flagellant.Code.Config;
using Flagellant.Code.Helper;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;

namespace Flagellant.Code.Patches;

[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
public static class FlagellantAnimationPatch
{
    public static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Entity == null || !__instance.Entity.IsPlayer || !DD2Helper.IsFlagellant(__instance.Entity.Player))
            return;

        switch (trigger)
        {
            case "Hit":
                PlayAnim(__instance, "Hit", true);
                break;

            case "Cast":    //原版角色的施法动作
            case "Attack":  //攻击卡牌在attackcmd里默认赋值trigger为attack,所以传入的是attack的话什么也不做
            case "DoNothing":
                break;

            case "Dead":  //官方的trigger("Dead")会先判断是否有spine结点，我的MOD人物场景里没有，所以在下面patch修改强行trigger，这回死亡能走进来了
                PlayAnim(__instance, "Dead", true);
                break;

            case "Revive": //同上
                PlayAnim(__instance, "Revive");
                break;

            case "DeathDoor":
                PlayAnim(__instance, "DeathDoor");
                break;

            case "Idle":
                PlayAnim(__instance, "Idle", false, true);
                break;

            default:
                PlayAnim(__instance, trigger, false, trigger.Contains("/"));
                break;
        }
    }

    private static void PlayAnim(NCreature node, string animName, bool playImmediately = false, bool bHasChildStateMachine = false)
    {
        if (!FlagellantConfig.ShouldPlayCardAnimAndSound) return;

        var visual = node.GetNodeOrNull<Node2D>("TestFlagellant");
        if (visual == null) return;

        var animPlayer = visual.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer == null) return;

        var animTree = visual.GetNodeOrNull<AnimationTree>("AnimationTree");
        if (animTree == null) return;

        //根据当前血量和灾厄的关系决定播放哪种Idle和Hit动画
        FlagellantHelper.ResetAdvancedConditions(animTree, node.Entity);

        var state_machine = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");
        AnimationNodeStateMachine? rootStateMachine = animTree.TreeRoot as AnimationNodeStateMachine;

        if (state_machine != null)
        {
            #region FixIdleAnimTravel
            //不要重复链接
            if (!_idleCallables.TryGetValue(node, out var IdleCallable) ||
                IdleCallable.Equals(default(Callable)) ||
                !state_machine.IsConnected("state_started", IdleCallable))
            {
                IdleCallable = Callable.From((StringName state) =>
                {
                    if (state == "Idle")
                    {
                        var Idle_SM = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/Idle/playback");
                        if (Idle_SM != null)
                        {
                            //闭包用到了node实例变量，所以需要用creature来做区分，而不能用一个static callable连接一切
                            Idle_SM.Travel(DD2Helper.WillDieInDoom(node.Entity) && FlagellantConfig.ShouldUseDeathDoorIdle ? "DeathIdle" : "Idle");
                        }
                        state_machine.Disconnect("state_started", IdleCallable);
                        _idleCallables.Remove(node);
                    }
                });
                _idleCallables[node] = IdleCallable;
                //持续监听,手动移除
                state_machine.Connect("state_started", IdleCallable);
            }
            #endregion FixIdleAnimTravel;

            if (!bHasChildStateMachine)
            {
                //检测到状态机中不存在的结点(例如使用了属于其他角色卡池的卡牌而触发动画时)则什么也不做
                if (rootStateMachine == null || !rootStateMachine.HasNode(animName)) return;

                bool shouldStartTo = playImmediately;
                if (animName == "Hit")
                {
                    if (state_machine.GetCurrentNode() == "Hit" || state_machine.GetCurrentNode() == "HitRecover" || state_machine.GetCurrentNode() == "DeathDoor")
                    {
                        //有人反馈连续挨打容易触发T姿势，我没遇到过，还是限制一下吧
                        return;
                    }
                    //若处于任意Idle时则平滑切换
                    shouldStartTo = !FlagellantHelper.IsInAnyIdle(animTree, node.Entity);
                }
                if (shouldStartTo)
                {
                    state_machine.Start(animName);
                }
                else
                {
                    state_machine.Travel(animName);
                }
            }
            if (bHasChildStateMachine) //打出卡牌动画先传送到CardPlay状态机再在内部传送到子动画
            {
                if (animName.Contains("CardSelect/"))
                {
                    var AR_SM = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/CardSelect/playback");
                    if (AR_SM != null)
                    {
                        string cardAnimName = animName.Replace("CardSelect/", "");
                        if (!string.IsNullOrEmpty(cardAnimName) && cardAnimName != "DoNothing")
                        {
                            //必须用Start立刻传送，否则在上一张牌动画Recover阶段没结束时迅速选择下一张牌，动画会无法正确播放
                            state_machine.Start("CardSelect");
                            AR_SM.Travel(cardAnimName);
                            if (!FlagellantConfig.ShouldMuteSeparately)
                            {
                                CombatAudioManager.PlayCombatSfx("CardSelect/" + cardAnimName
                                    //, false, 
                                    //false, 
                                    //AudioCfg.GetFlagellantVolumeDB("CardSelect/" + cardAnimName)
                                    );
                            }
                        }
                    }
                }
                else if (animName.Contains("CardPlay/"))
                {
                    var Attack_SM = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/CardPlay/playback");
                    if (Attack_SM != null)
                    {
                        string cardAnimName = animName.Replace("CardPlay/", "");
                        //单独调整melee_recover(Punish和Necrosis状态用的)动画，把前面的头掐掉看着更顺畅
                        if (cardAnimName == "Punish" || cardAnimName == "Necrosis")
                        {
                            animTree.Set("parameters/CardPlay/" + cardAnimName + "_Recover/TimeSeek/seek_request", 0.35f);
                        }

                        //不要重复链接
                        if (!Attack_SM.IsConnected("state_started", _stateStartedCallable))
                        {
                            //持续监听,不要OnShot
                            Attack_SM.Connect("state_started", _stateStartedCallable);
                        }

                        state_machine.Travel("CardPlay");
                        Attack_SM.Travel(cardAnimName);
                    }
                }
                else if (animName == "Idle")
                {
                    if (FlagellantHelper.IsInAnyIdle(animTree, node.Entity))
                    {
                        //已经在CalmIdle(Idle_A)状态时收到取消打出卡牌信号(例如取消打出没有选牌动画的卡牌)后不再调为Idle(Idle_B)状态
                        return;
                    }
                    //由信号通知来切换Idle内部状态
                    state_machine.Travel("Idle");
                    /*var Idle_SM = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/Idle/playback");
                    if (Idle_SM != null)
                    {
                        state_machine.Travel("Idle");
                        Idle_SM.Travel(DD2Helper.WillDieInDoom(node.Entity) ? "DeathIdle" : "Idle");
                    }*/
                }
            }
        }
    }
    private static readonly Dictionary<NCreature, Callable> _idleCallables = new();
    private static readonly Callable _stateStartedCallable = Callable.From((StringName state) =>
    {
        if (!FlagellantConfig.ShouldMuteSeparately)
        {
            float VolumeDB = CombatAudioCfg.GetFlagellantVolumeDB("CardPlay/" + state);
            //按理来说音频可叠加，但测试发现state和state_Recover都用TempAudio播放会失真？所以区分一下
            CombatAudioManager.PlayCombatSfx("CardPlay/" + state, state.ToString().Contains("Recover"), false, VolumeDB);
            if (state == "Lash")
            {
                //Lash类技能有锤肉的音效，忘了加了在这补上
                CombatAudioManager.PlayCombatSfx("CardPlay/Suffer", false, false, -10, 1);
            }
        }
    });
}

[HarmonyPatch(typeof(HoveredModelTracker), "OnLocalCardSelected")]
public class FlagellantOnSelectedPatch
{
    public static void Postfix(CardModel cardModel)
    {
        if (!FlagellantConfig.ShouldPlayCardAnimAndSound) return;

        if (cardModel is not FlagellantCardModel) return;

        FlagellantCardModel? MyCard = cardModel as FlagellantCardModel;
        if (MyCard == null || MyCard.CardSelectAnimName == "DoNothing") return;

        CreatureCmd.TriggerAnim(MyCard.Owner.Creature, "CardSelect/" + MyCard.CardSelectAnimName, 0);
    }
}

[HarmonyPatch(typeof(NMultiplayerPlayerIntentHandler), "RefreshHoverTips")]
public static class FlagellantOnCardSelectedPatch
{
    private static CardModel? _lastSelectedCard = null;
    public static void Postfix(NMultiplayerPlayerIntentHandler __instance)
    {
        if (!FlagellantConfig.ShouldShowCardAnimInMultiplayerMode) return;

        //bool ShouldShowHoverTip = Traverse.Create(__instance).Field("_shouldShowHoverTip").GetValue<bool>();
        Player? CurrentPlayer = Traverse.Create(__instance).Field("_player").GetValue<Player>();

        //Log.Info("ShouldShowHoverTip : " + ShouldShowHoverTip);  //不知道为什么ShouldShowHoverTip一直是false，先屏蔽了
        if (CurrentPlayer == null || LocalContext.IsMe(CurrentPlayer)) return;// || !ShouldShowHoverTip) return;

        NMultiplayerCardIntent CardIntent = Traverse.Create(__instance).Field("_cardIntent").GetValue<NMultiplayerCardIntent>();
        if (CardIntent != null && CardIntent.Card != null && CardIntent.Visible)
        {
            CardModel cardModel = CardIntent.Card;
            if (cardModel is FlagellantCardModel MyCard)
            {
                if (MyCard.CardSelectAnimName == "DoNothing")
                {
                    CreatureCmd.TriggerAnim(CurrentPlayer.Creature, "Idle", 0);
                }
                else
                {
                    CreatureCmd.TriggerAnim(CurrentPlayer.Creature, "CardSelect/" + MyCard.CardSelectAnimName, 0);
                }
            }
            else  //选中非苦修卡池的卡
            {
                CreatureCmd.TriggerAnim(CurrentPlayer.Creature, "Idle", 0);
            }
        }
        //取消选择卡牌且没打出
        else
        {
            if (_lastSelectedCard?.Pile?.Type == PileType.Hand)
            {
                CreatureCmd.TriggerAnim(CurrentPlayer.Creature, "Idle", 0);
            }
        }
        _lastSelectedCard = CardIntent?.Card;
    }
}


[HarmonyPatch(typeof(NPlayerHand), "StartCardPlay")]
public class TestCardPlayPatch
{
    public static void Postfix(NPlayerHand __instance)
    {
        //不知道为什么，更新BaseLib 3.0.9以后会无法Patch到OnLocalCardSelected，加了这个空Patch之后就正常了？？？
        //而且还必须放在OnLocalCardSelected的Patch的下面？？？。。。我已没招
    }
}

//DO NOT USE : [HarmonyPatch(typeof(HoveredModelTracker), "OnLocalCardDeselected")]
//Because it is [OnSelected -> CancelPlayCard -> OnPlay] BUT [OnSelected -> CancelPlayCard -> OnPlay -> OnLocalCardDeselected]
[HarmonyPatch(typeof(NCardPlay), "CancelPlayCard")]
public class FlagellantCancelPlayCardPatch
{
    public static void Prefix(NCardPlay __instance)  //返回类型为void会继续执行原方法
    {
        if (!FlagellantConfig.ShouldPlayCardAnimAndSound) return;

        if (!GodotObject.IsInstanceValid(__instance)) return;

        CardModel Card = Traverse.Create(__instance).Property("Card").GetValue<CardModel>();
        if (Card == null || Card is not FlagellantCardModel) return;

        #region FixPowerCardPlayedTravelToIdle
        //发现PowerCard在打出时会先停顿一小会再播放打出动画(Attack和Skill倒是会立刻播放，但其实也经历了Travel到Idle的过程，只不过随后又立刻切换了)。
        //由于打出卡牌后会先触发CancelPlayCard再触发OnPlay,所以检测到卡牌不是手动取消时,直接返回不Travel到Idle以保持动画流畅

        //为什么打出卡牌前会先触发一次CancelPlayCard？
        //NMouseCardPlay或者NControllerCardPlay中的_Input()函数中会接收到取消按键时调用CancelPlayCard()
        //而正常打出卡牌时，可能是因为NMouseCardPlay打出卡牌时点击了左键，被_EnterTree中链接的NControllerManager.SignalName.MouseDetected捕获到按键而调用CancelCardPlay?
        //用控制器打出卡牌时没看到NControllerCardPlay中有类似的一检测到按键就CancelCardPlay的信号链接，可能走的NPlayerHand中_UnhandledInput中的Mode.Play里的CancelCardPlay？
        if (!(MouseAndControllerPatch.isMouseCanceled || MouseAndControllerPatch.isControllerCanceled)
            && Card.CanPlay()) return;

        /*//本来没用新的patch记录是否由按键手动取消卡牌，用的下面的堆栈检测是否是_Input里调用的CancelCardPlay
        //但是发现NControllerCardPlay里的_Input发送打出卡牌通知后会在Start中TaskHelper.RunSafely(SingleCreatureTargeting(base.Card.TargetType));
        //导致用Controller打出单体卡牌时无法查看堆栈来把是否有_Input作为条件，想了想还是直接patch按键事件算了。
        var stack = new System.Diagnostics.StackTrace();
        bool isManuallyCancel = false;
        foreach (var frame in stack.GetFrames())
        {
            //从堆栈中检测这个CancelPlayCard是不是由_Input()函数引起的
            var method = frame.GetMethod();
            if (method != null && method.Name == "_Input")
            {
                isManuallyCancel =  true; // 执行原方法
            }
        }
        if (!isManuallyCancel) return;*/
        #endregion FixPowerCardPlayedTravelToIdle

        CreatureCmd.TriggerAnim(Card.Owner.Creature, "Idle", 0);
    }
}

[HarmonyPatch(typeof(NCardPlay), "OnCreatureUnhover")]
public class FlagellantOnCreatureUnhoverPatch
{
    public static void Postfix(NCardPlay __instance, NCreature _)
    {
        if (__instance is not NControllerCardPlay) return;

        CardModel Card = Traverse.Create(__instance).Property("Card").GetValue<CardModel>();
        if (Card == null || Card is not FlagellantCardModel) return;

        CreatureCmd.TriggerAnim(Card.Owner.Creature, "Idle", 0);
    }
}

//可能改为在卡牌OnPlay时调用attackcommand.WithAttackerAnim好一些?
[HarmonyPatch(typeof(AttackCommand), "FromCard")]
public class FlagellantAttackCommandPatch
{
    public static void Postfix(AttackCommand __instance, CardModel card)
    {
        if (card is not FlagellantCardModel) return;

        if (!DD2Helper.IsFlagellant(card.Owner)) return;

        FlagellantCardModel MyCard = (FlagellantCardModel)card;
        if (MyCard != null && MyCard.CardPlayAnimName != "DoNothing")
        {
            Traverse.Create(__instance).Field("_attackerAnimName").SetValue("CardPlay/" + MyCard.CardPlayAnimName);
        }
    }
}

[HarmonyPatch(typeof(NCreature), "StartDeathAnim")]
public class FlagellantDeathAnimPatch
{
    public static void Postfix(NCreature __instance)
    {
        if (!DD2Helper.IsFlagellant(__instance.Entity.Player)) return;

        __instance.SetAnimationTrigger("Dead");
    }
}

[HarmonyPatch(typeof(NCreature), "StartReviveAnim")]
public class FlagellantReviveAnimPatch
{
    public static void Postfix(NCreature __instance)
    {
        if (!DD2Helper.IsFlagellant(__instance.Entity.Player)) return;

        if (!DD2Helper.WillDieInDoom(__instance.Entity))
        {
            __instance.SetAnimationTrigger("Revive");
        }
    }
}