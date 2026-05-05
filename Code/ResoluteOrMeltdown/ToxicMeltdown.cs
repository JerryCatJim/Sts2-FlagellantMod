using Flagellant.Code.Commands;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.ResoluteOrMeltdown;

public class ToxicMeltdown : ResoluteOrMeltdownModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override ResoluteOrMeltdownType RMType => ResoluteOrMeltdownType.Toxic;

    public override Task OnEnterResoluteOrMeltdown(PlayerChoiceContext ctx, Player player, CardModel? source)
    {
        PowerCmd.Apply<ToxicPower>(Owner.Creature, 1, Owner.Creature, source);
        return base.OnEnterResoluteOrMeltdown(ctx, player, source);
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && dealer != target && result.TotalDamage > 0 && target != null && target.IsAlive)
        {
            await PowerCmd.Apply<PoisonPower>(target, result.TotalDamage, Owner.Creature, null);
        }
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Creature.Side) return;

        await PowerCmd.Remove<ToxicPower>(Owner.Creature);
        await RMCmd.ExitResoluteOrMeltdown(ctx, Owner, null);
    }
}