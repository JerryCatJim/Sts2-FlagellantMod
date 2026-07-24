using Flagellant.Code.Commands;
using Flagellant.Code.Powers;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.ResoluteOrMeltdown;

public class ToxicMeltdown : ResoluteOrMeltdownModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override ResoluteOrMeltdownType RMType => ResoluteOrMeltdownType.Toxic;

    public override async Task OnEnterResoluteOrMeltdown(PlayerChoiceContext choiceContext, Player player, CardModel? source)
    {
        if(LocalContext.IsMe(player))
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSmokyVignetteVfx.Create(new Color(0.8f, 0.8f, 0.3f, 0.66f), new Color(0f, 4f, 0f, 0.33f)));
        }
        decimal num = Math.Round(player.Creature.MaxHp * 45 / 100m);
        await CreatureCmd.SetCurrentHp(player.Creature, num < 1m ? 1m : num);

        await PowerCmd.Apply<ScourgeFormPower>(choiceContext, Owner.Creature, 2, Owner.Creature, source);
        await PowerCmd.Apply<ToxicPower>(choiceContext, Owner.Creature, 1, Owner.Creature, source);
        await base.OnEnterResoluteOrMeltdown(choiceContext, player, source);
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && dealer != target && result.TotalDamage > 0 && result.Props.IsPoweredAttack())
        {
            if(Owner.Creature.GetPower<ToxicPower>() is ToxicPower toxicPower)
            {
                toxicPower.ToxicPowerFlash();
            }
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGaseousImpactVfx.Create(target, new Color("008000"))); //83eb85
            await PowerCmd.Apply<PoisonPower>(choiceContext, target, result.TotalDamage, Owner.Creature, null);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner.Creature))  //if (side == base.Owner.Side)
        {
            await PowerCmd.Remove<ToxicPower>(Owner.Creature);
            await RMCmd.ExitResoluteOrMeltdown(choiceContext, Owner, null);
        }
    }
}