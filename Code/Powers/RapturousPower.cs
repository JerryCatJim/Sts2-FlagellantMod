using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Ancient;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public sealed class RapturousPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StressPower>()
    ];

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        if(canonicalPower != null && canonicalPower is StressPower && amount > 0m && target == Owner)
        {
            modifiedAmount = amount + Amount;
            return true;
        }
        modifiedAmount = amount;
        return false;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(Owner == null || side != Owner.Side) return;

        StressPower? SP = Owner.GetPower<StressPower>();
        if(SP != null)
        {
            await PowerCmd.ModifyAmount(SP, -Amount, base.Owner, ModelDb.Card<Rapturous>());
        }
    }
}
