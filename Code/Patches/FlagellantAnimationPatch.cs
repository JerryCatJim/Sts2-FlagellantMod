using Flagellant.Code.Abstract;
using Flagellant.Audio;
using Flagellant.Code.Character;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using static Godot.GodotObject;
using static Godot.Node;
using static Godot.PackedScene;

namespace Flagellant.Code.Patch;

[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
public static class FlagellantAnimationPatch
{
	public static void Postfix(NCreature __instance, string trigger)
	{
		if (__instance.Entity == null || !__instance.Entity.IsPlayer)
			return;

		if (__instance.Entity.ModelId.ToString() == "CHARACTER.FLAGELLANT-FLAGELLANT")
			Log.Info("[>>>Flagellant AnimeTrigger=]" + trigger);

		switch (trigger)
		{
			case "Hit":
				PlayAnim(__instance, "Hit", true);
				break;

            case "Cast":    //是丢东西的动作吗?
            case "Attack":  //攻击卡牌在attackcmd里默认赋值trigger为attack,所以传入的是attack的话什么也不做
            case "DoNothing":
                break;

			case "Dead":  //官方的trigger("Dead")会先判断是否有spine结点，我的MOD人物场景里没有，所以在下面patch修改强行trigger，这回死亡能走进来了
				PlayAnim(__instance, "DeathDoor", true);
				break;

           case "Revive":
           case "Idle":
                PlayAnim(__instance, "Idle");
                break;

            default:
                PlayAnim(__instance, trigger, false, true);
                break;
        }
    }

	private static void PlayAnim(NCreature node, string animName, bool playImmediately = false, bool bCardPlayAnim = false)
	{
		var visual = node.GetNodeOrNull<Node2D>("TestFlagellant");
		if (visual == null) return;

		var animPlayer = visual.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (animPlayer == null) return;

		var animTree = visual.GetNodeOrNull<AnimationTree>("AnimationTree");
		if (animTree == null) return;

		var state_machine = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/StateMachine/playback");

		if (state_machine != null)
		{
			//animPlayer.Stop(); //animPlayer.Stop对状态机没用?
			if(playImmediately && !bCardPlayAnim)
			{
                state_machine.Start(animName);
            }
            else
            {
                state_machine.Travel(animName);
            }
			if (bCardPlayAnim) //打出卡牌动画先传送到CardPlay状态机再在内部传送到子动画
			{
                var Attack_SM = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/StateMachine/CardPlay/playback");
                if (Attack_SM != null)
                {
                    state_machine.Travel("CardPlay");
                    Attack_SM.Travel(animName);
                    //单独调整melee_recover(Punish和Necrosis状态用的)动画，把前面的头掐掉看着更顺畅
                    if(animName == "Punish" || animName == "Necrosis")
                    {
                        animTree.Set("parameters/StateMachine/CardPlay/" + animName + "_Recover/TimeSeek/seek_request", 0.35f);
                    }
                    //鞭笞之赐的声音调大点
                    if(animName == "Lash")
                    {
                        AudioManager.PlayCombatSfx("CardPlay/Lash", false, false, 0);
                    }
                    else
                    {
                        AudioManager.PlayCombatSfx("CardPlay/" + animName);
                    }

                    //不要重复链接
                    if (!Attack_SM.IsConnected("state_started", _stateStartedCallable))
                    {
                        //持续监听,不要OnShot
                        Attack_SM.Connect("state_started", _stateStartedCallable);
                    }
                }
            }
        }
	}
    private static readonly Callable _stateStartedCallable = Callable.From((StringName state) =>
    {
        //按理来说音频可叠加，但测试发现state和state_Recover都用TempAudio播放会失真？所以区分一下
        AudioManager.PlayCombatSfx("CardPlay/" + state, state.ToString().Contains("Recover"));
    });
}

[HarmonyPatch(typeof(HoveredModelTracker), "OnLocalCardSelected")]
public class FlagellantOnSelectedPatch
{
	public static void Postfix(CardModel cardModel)
	{
        if (cardModel is not FlagellantCardModel) return;

        FlagellantCardModel MyCard = (FlagellantCardModel)cardModel;
        if (MyCard == null || MyCard.CardSelectAnimName == "DoNothing") return;

        NCreature CharNode = NCombatRoom.Instance?.GetCreatureNode(cardModel.Owner.Creature);
		if(CharNode == null) return;

        var visual = CharNode.GetNodeOrNull<Node2D>("TestFlagellant");
        if (visual == null) return;

        var animPlayer = visual.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer == null) return;

        var animTree = visual.GetNodeOrNull<AnimationTree>("AnimationTree");
        if (animTree == null) return;

        var state_machine = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/StateMachine/playback");
        var AR_SM = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/StateMachine/CardSelect/playback");

        if (state_machine != null && AR_SM != null)
        {
            state_machine.Start("CardSelect");
            AR_SM.Travel(MyCard.CardSelectAnimName);
            AudioManager.PlayCombatSfx("CardSelect/" + MyCard.CardSelectAnimName);
        }
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

//[HarmonyPatch(typeof(HoveredModelTracker), "OnLocalCardDeselected")]
[HarmonyPatch(typeof(NCardPlay), "CancelPlayCard")] //不要使用OnLocalCardDeselected，这在卡牌被打出之后也会被执行，导致跳转到攻击动画后又立刻跳转回到Idle
public class FlagellantCancelPlayCardPatch
{
    public static void Prefix(NCardPlay __instance)  //返回类型为void会继续执行原方法
    {
        if (!GodotObject.IsInstanceValid(__instance)) return;

        CardModel Card = Traverse.Create(__instance).Property("Card").GetValue<CardModel>();
        if (Card == null) return;

        NCreature CharNode = NCombatRoom.Instance?.GetCreatureNode(Card.Owner.Creature);
        if (CharNode == null) return;

        var visual = CharNode.GetNodeOrNull<Node2D>("TestFlagellant");
        if (visual == null) return;

        var animPlayer = visual.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer == null) return;

        var animTree = visual.GetNodeOrNull<AnimationTree>("AnimationTree");
        if (animTree == null) return;

        var state_machine = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/StateMachine/playback");

        if (state_machine != null && state_machine.GetCurrentNode() != "CalmIdle")
        {
            state_machine.Travel("Idle");
        }
    }
}

//可能改为在卡牌OnPlay时调用attackcommand.WithAttackerAnim好一些?
[HarmonyPatch(typeof(AttackCommand), "FromCard")]
public class FlagellantAttackCommandPatch
{
    public static void Postfix(AttackCommand __instance, CardModel card)
    {
        if (card is not FlagellantCardModel) return;

        FlagellantCardModel MyCard = (FlagellantCardModel)card;
        if (MyCard != null && MyCard.CardPlayAnimName != "DoNothing")
        {
            Traverse.Create(__instance).Field("_attackerAnimName").SetValue(MyCard.CardPlayAnimName);
        }
    }
}

[HarmonyPatch(typeof(NCreature), "StartDeathAnim")]
public class FlagellantDeathAnimPatch
{
    public static void Postfix(NCreature __instance)
    {
        __instance.SetAnimationTrigger("Dead");
    }
}
