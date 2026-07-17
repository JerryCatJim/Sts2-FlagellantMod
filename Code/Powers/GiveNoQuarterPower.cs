using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public class GiveNoQuarterPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: Fix it with TryModifyEnergyCostInCombatLate after the game updated.
    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (ShouldSkip(card))
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = default(decimal);
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        //判断条件抄的储君的虚空形态
        if(!ShouldSkip(cardPlay.Card)
            && cardPlay != null && !cardPlay.IsAutoPlay && cardPlay.IsLastInSeries)
        {
            await PowerCmd.Decrement(this);
        }
    }

    private bool ShouldSkip(CardModel card)
    {
        if (card.Owner.Creature != base.Owner) return true;

        bool flag = true;
        switch (card.Pile?.Type)
        {
            case PileType.Hand:
            case PileType.Play:
                flag = false;
                break;
            default:
                flag = true;
                break;
        }
        return flag;
    }
}
