using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public sealed class HoldTheLinePower : FlagellantPowerModel, IAfterComboChanged
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterComboChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel? cardSource)
    {
        if (amount == 0m || base.Owner.Player == null) return;

        if(power is ComboPower CP && CP.LastApplier == base.Owner)
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, Amount, base.Owner.Player);
        }
    }
}
