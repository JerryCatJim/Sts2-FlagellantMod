using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public class PressThisAdvantagePower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _isSelfInitialized = false;

    //TODO: Fix it with TryModifyEnergyCostInCombatLate after the game updated.
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (ShouldSkip(card))
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = default(decimal);
        return true;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        //刚获得能力之后会收到附加该能力卡牌的打出完成事件，需要排除
        if(!ShouldSkip(cardPlay.Card) && _isSelfInitialized)
        {
            PowerCmd.Decrement(this);
        }
        _isSelfInitialized = true;
        return Task.CompletedTask;
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
