using Flagellant.Code.Audio;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.Monster;

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

            case "Dead":
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
                AudioManager.PlayMonsterSfx("Hit",true);
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
            if (bHasChildStateMachine)
            {
                if (animName.Contains("Attack/"))
                {
                    var Attack_SM = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/Attack/playback");

                    if (state_machine != null && Attack_SM != null)
                    {
                        string attackAnimName = animName.Replace("Attack/", "");
                        if (!String.IsNullOrEmpty(attackAnimName))
                        {
                            if (!Attack_SM.IsConnected("state_started", _stateStartedCallable))
                            {
                                //持续监听,不要OnShot
                                Attack_SM.Connect("state_started", _stateStartedCallable);
                            }
                            state_machine.Start("Attack");
                            Attack_SM.Travel(attackAnimName);
                        }
                    }
                }
            }
        }
    }
    private static readonly Callable _stateStartedCallable = Callable.From((StringName state) =>
    {
        bool isTemp = state.ToString().Contains("Recover");// || state.ToString().Contains("Action");
        AudioManager.PlayMonsterSfx(state, isTemp);
    });
}

[HarmonyPatch(typeof(RunManager), "OnEnded")]
public static class OnGameEndedPatch
{
    public static void Postfix()
    {
        DeathListenForRunStateSingleton.ResetValue();
    }
}