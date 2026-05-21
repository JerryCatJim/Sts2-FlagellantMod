using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Nervous : FlagellantCardModel
{
    private decimal _calculatedStress = 0;
    public Nervous() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithStress(2);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithCalculatedBlock(0, ((CardModel card, Creature? c) =>
        {
            if (card != null && card is Nervous myCard)
            {
                if (myCard._calculatedStress != 0)
                {
                    return myCard._calculatedStress;
                }
                else
                {
                    return (myCard.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0m) + myCard.GetStressBeforeReceived();
                }
            }
            return 0;
        }
        ));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _calculatedStress = (base.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0m) + GetStressBeforeReceived();
        await CommonActions.ApplySelf<StressPower>(this);
        await CommonActions.CardBlock(this, base.DynamicVars.CalculatedBlock, cardPlay);
        _calculatedStress = 0;
    }
}
