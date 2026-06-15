using Flagellant.Code.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.Patches;

[HarmonyPatch(typeof(CreatureCmd), "Heal")]
public static class CreatureCmdHealPatch
{
    public static bool Prefix(Creature creature, ref decimal amount)
    {
        if (CombatManager.Instance.IsEnding && !creature.IsPlayer)
        {
            return true;
        }
        IRunState? runState = creature.Player?.RunState; //不用combatState，保留在战斗外监听血量回复的可能
        if (runState != null)
        {
            amount = ModifyHpAmountReceived(runState, creature, amount);
        }
        return true;
    }
    public static decimal ModifyHpAmountReceived(IRunState runState, Creature creature, decimal amount, bool silent = false)
    {
        decimal num = amount;
        if(runState != null)
        {
            foreach (AbstractModel item in runState.IterateHookListeners(creature.CombatState))
            {
                if (item is IModifyHpAmountReceived myModel)
                {
                    myModel.TryModifyHpAmountReceived(creature, num, out var myModifiedAmount, silent);
                    num = myModifiedAmount;
                }
            }
        }
        return num;
    }
}