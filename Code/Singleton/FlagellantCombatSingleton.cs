using BaseLib.Abstracts;
using Flagellant.Code.Abstract;
using Flagellant.Code.Config;
using Flagellant.Code.Helper;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Flagellant.Code.Singleton;

public class FlagellantCombatSingleton : CustomSingletonModel, IAfterDeathDoor
{
    public FlagellantCombatSingleton() : base(HookType.Combat)
    {

    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (DD2Helper.IsFlagellant(creature) && delta > 0 && DD2Helper.WillDieInDoom(creature) != DD2Helper.WillDieInDoom(creature, delta, 0))
        {
            FlagellantHelper.ResetAdvancedConditions(null, creature);
            if (FlagellantHelper.IsInAnyIdle(null, creature))
            {
                await CreatureCmd.TriggerAnim(creature, "Revive", 0f);
            }
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not DoomPower || amount >= 0) return;

        Creature creature = power.Owner;
        if (DD2Helper.IsFlagellant(creature) && amount < 0 && DD2Helper.WillDieInDoom(creature) != DD2Helper.WillDieInDoom(creature, 0, amount))
        {
            FlagellantHelper.ResetAdvancedConditions(null, creature);
            if (FlagellantHelper.IsInAnyIdle(null, creature))
            {
                await CreatureCmd.TriggerAnim(creature, "Revive", 0f);
            }
        }
    }

    public async Task AfterDeathDoor(Creature creature, decimal healthDelta, decimal powerDelta, DeathDoorType type)
    {
        if (DD2Helper.IsFlagellant(creature) && FlagellantConfig.ShouldUseDeathDoorIdle && type == DeathDoorType.Doom)
        {
            FlagellantHelper.ResetAdvancedConditions(null, creature);
            if (FlagellantHelper.IsInAnyIdle(null, creature) && DD2Helper.ShouldPlayDeathDoorVfx(creature))
            {
                await CreatureCmd.TriggerAnim(creature, DD2Helper.WillDieInDoom(creature) ? "DeathDoor" : "Revive", 0f);
            }
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        foreach (Creature creature in room.CombatState.PlayerCreatures)
        {
            if (DD2Helper.IsFlagellant(creature))
            {
                await CreatureCmd.TriggerAnim(creature, "Revive", 0f);
            }
        }
    }
}