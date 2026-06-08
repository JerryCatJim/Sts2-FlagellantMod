using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Monster;

public class DeathListenForCombatStateSingleton : CustomSingletonModel
{
    public DeathListenForCombatStateSingleton() : base(HookType.Combat)
    {

    }

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (!DeathListenForRunStateSingleton.ShouldSpawnDeathThisRoom) return;

        if (creature.IsMonster && creature.Side != MegaCrit.Sts2.Core.Combat.CombatSide.Player
            && creature.Monster is not Death 
            && !creature.HasPower<StockPower>()
            && !creature.HasPower<InfestedPower>()
            && !creature.HasPower<SurprisePower>()
            )
        {
            await PowerCmd.Apply<SpawnDeathPower>(creature, 1, null, null);
        }
    }
}