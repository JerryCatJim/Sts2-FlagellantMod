using BaseLib.Abstracts;
using Flagellant.Code.Abstract;
using Flagellant.Code.Config;
using Flagellant.Code.Helper;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;

namespace Flagellant.Code.Singleton;

public class FlagellantCombatSingleton : CustomSingletonModel, IAfterDeathDoor
{
    public FlagellantCombatSingleton() : base(HookType.Combat)
    {

    }

    public async Task AfterDeathDoor(Creature creature, decimal healthDelta, decimal doomPowerDelta, bool IsLowHealthTrigger)
    {
        if (DD2Helper.IsFlagellant(creature) && FlagellantConfig.ShouldUseDeathDoorIdle)
        {
            FlagellantHelper.ResetAdvancedConditions(null, creature);
            if (DD2Helper.WillDieInDoom(creature) != DD2Helper.WillDieInDoom(creature, healthDelta, doomPowerDelta))
            {
                if (FlagellantHelper.IsInAnyIdle(null, creature))
                {
                    await CreatureCmd.TriggerAnim(creature, DD2Helper.WillDieInDoom(creature) ? "DeathDoor" : "Revive", 0f);
                }
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