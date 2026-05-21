using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Necrosis : FlagellantCardModel
{
    private decimal _calculatedBlockNum = 0;
    public Necrosis() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithAnimName("Necrosis");
        WithPowerTip<StressPower>();
        WithPowerTip<PoisonPower>();
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithCalculatedBlock(0, ((CardModel card, Creature? c) =>
        {
            if (card != null && card is Necrosis myCard)
            {
                if (myCard._calculatedBlockNum != 0)
                {
                    return myCard._calculatedBlockNum;
                }
                else if(c != null)
                {
                    return (myCard.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0m)
                            + (c.GetPower<PoisonPower>()?.Amount ?? 0);
                }
            }
            return 0;
        }
        ));
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        if (cardPlay.Target != null)
        {
            decimal StressNum = Owner.Creature.GetPower<StressPower>()?.Amount ?? 0;
            if(StressNum > 0)
            {
                await CommonActions.Apply<PoisonPower>(cardPlay.Target, this, StressNum);
            }
            _calculatedBlockNum = cardPlay.Target.GetPower<PoisonPower>()?.Amount ?? 0;
            if (_calculatedBlockNum > 0)
            {
                await CommonActions.CardBlock(this, base.DynamicVars.CalculatedBlock, cardPlay);
            }
        }
        _calculatedBlockNum = 0;
    }
}
