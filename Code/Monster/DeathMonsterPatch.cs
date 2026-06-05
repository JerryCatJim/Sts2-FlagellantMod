using Flagellant.Audio;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
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
        CheckDeathAppearSingleton.ResetValue();
    }
}

//修复千足虫死亡后 若遭遇死神 会导致千足虫凋零后带着死神一起死亡凋零的错误
[HarmonyPatch(typeof(ReattachPower), "DoFadeOutOnAllSegments")]
public static class DecimillipedeAfterDeathPatch
{
    public static bool Prefix(ReattachPower __instance)
    {
        float val = 0f;
        List<NCreature> list = new List<NCreature>();
        foreach (Creature enemy in __instance.CombatState.Enemies)
        {
            //我添加的插入方法
            if (enemy.Monster is Death) continue;

            NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(enemy);
            if (nCreature != null)
            {
                nCreature.AnimHideIntent();
                val = Math.Max(val, nCreature.GetCurrentAnimationLength());
                list.Add(nCreature);
            }
        }

        NMonsterDeathVfx nMonsterDeathVfx = NMonsterDeathVfx.Create(list);
        if (nMonsterDeathVfx == null || list.Count <= 0)
        {
            return false;
        }

        Node parent = list[0].GetParent();
        parent.AddChildSafely(nMonsterDeathVfx);
        parent.MoveChild(nMonsterDeathVfx, list[0].GetIndex());

        var method = AccessTools.Method(typeof(ReattachPower), "PlayVfxAndThenRemoveNodes");
        Task deathAnimationTask = TaskHelper.RunSafely((Task)method.Invoke(__instance, new object[] { nMonsterDeathVfx, list }));
        //Task deathAnimationTask = TaskHelper.RunSafely(__instance.PlayVfxAndThenRemoveNodes(nMonsterDeathVfx, list));
        foreach (NCreature item in list)
        {
            item.DeathAnimationTask = deathAnimationTask;
            NCombatRoom.Instance?.RemoveCreatureNode(item);
        }

        return false;
    }
}