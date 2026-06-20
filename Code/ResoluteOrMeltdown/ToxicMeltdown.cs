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

    public override Task OnEnterResoluteOrMeltdown(PlayerChoiceContext choiceContext, Player player, CardModel? source)
    {
        PowerCmd.Apply<ScourgeFormPower>(choiceContext, Owner.Creature, 2, Owner.Creature, source);
        PowerCmd.Apply<ToxicPower>(choiceContext, Owner.Creature, 1, Owner.Creature, source);
        return base.OnEnterResoluteOrMeltdown(choiceContext, player, source);
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && dealer != target && result.TotalDamage > 0 && result.Props.IsPoweredAttack())
        {
            if(Owner.Creature.GetPower<ToxicPower>() is ToxicPower toxicPower)
            {
                toxicPower.ToxicPowerFlash();
            }
            await PowerCmd.Apply<PoisonPower>(choiceContext, target, result.TotalDamage, Owner.Creature, null);
        }
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        //if (side != base.Owner.Creature.Side) return;
        if (!participants.Contains(base.Owner.Creature)) return;

        await PowerCmd.Remove<ToxicPower>(Owner.Creature);
        await RMCmd.ExitResoluteOrMeltdown(choiceContext, Owner, null);
    }
}