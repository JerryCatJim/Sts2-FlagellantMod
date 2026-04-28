using BaseLib.Utils;
using Flagellant.Code.Commands;
using Flagellant.Code.Powers;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;
using SmartFormat.Core.Extensions;

namespace Flagellant.Code.ResoluteOrMeltdown;

public class ToxicMeltdown : ResoluteOrMeltdownModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task OnEnterResoluteOrMeltdown(PlayerChoiceContext ctx, Player player, CardModel? source)
    {
        PowerCmd.Apply<ToxicPower>(Owner.Creature, 1, Owner.Creature, source);
        return base.OnEnterResoluteOrMeltdown(ctx, player, source);
    }

    /*public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == Owner.Creature && !props.HasFlag(ValueProp.Unpowered))
            return 3m;
        return 1m;
    }*/

    public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Creature.Side) return;

        await PowerCmd.Remove<ToxicPower>(Owner.Creature);
        await RMCmd.ExitResoluteOrMeltdown(ctx, Owner, null);
    }
}