using Flagellant.Audio;
using Flagellant.Code.Abstract;
using Flagellant.Code.Config;
using Flagellant.Code.GameActions;
using Flagellant.Code.Monster;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.Patches;

[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
public static class DeathAnimPatch
{
    public static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Entity == null || __instance.Entity.Monster is not Death)
            return;

        if (__instance.Entity.ModelId.ToString() == "MONSTER.FLAGELLANT-DEATH")
        {
            Log.Info("[>>>Flagellant Monster AnimeTrigger=]" + trigger);
        }

        switch (trigger)
        {
            case "Hit":
                PlayAnim(__instance, "Hit", false);
                break;

            case "":
            case null:
                break;

            case "Dead":  //官方的trigger("Dead")会先判断是否有spine结点，我的MOD人物场景里没有，所以在下面patch修改强行trigger，这回死亡能走进来了
                PlayAnim(__instance, "Dead", true);
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

    private static void PlayAnim(NCreature node, string animName, bool playImmediately = false, bool bHasChildStateMachine = false)
    {
        var visual = node.GetNodeOrNull<Node2D>("TestDeath");
        if (visual == null) return;

        var animPlayer = visual.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer == null) return;

        var animTree = visual.GetNodeOrNull<AnimationTree>("AnimationTree");
        if (animTree == null) return;

        var state_machine = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");

        if (state_machine != null)
        {
            if (animName == "Hit" && (state_machine.GetCurrentNode() == "Spawn" || (state_machine.GetCurrentNode() == "Hit")))
            {
                return;
            }
            if (animName == "Hit")
            {
                AudioManager.PlayMonsterSfx("Hit",true,false,-2f);
            }
            if (!bHasChildStateMachine)
            {
                if (playImmediately)
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
                if (animName.Contains("Attack/"))
                {
                    var Attack_SM = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/Attack/playback");

                    if (state_machine != null && Attack_SM != null)
                    {
                        string attackAnimName = animName.Replace("Attack/", "");
                        if (!String.IsNullOrEmpty(attackAnimName))
                        {
                            state_machine.Start("Attack");
                            Attack_SM.Travel(attackAnimName);
                        }
                        if (!Attack_SM.IsConnected("state_started", _stateStartedCallable))
                        {
                            //持续监听,不要OnShot
                            Attack_SM.Connect("state_started", _stateStartedCallable);
                        }
                    }
                }
            }
        }
    }
    private static readonly Callable _stateStartedCallable = Callable.From((StringName state) =>
    {
        /*if (!FlagellantConfig.ShouldMuteSeparately)
        {
            //按理来说音频可叠加，但测试发现state和state_Recover都用TempAudio播放会失真？所以区分一下
            AudioManager.PlayCombatSfx("Attack/" + state, state.ToString().Contains("Recover"));
        }*/
    });
}