using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class FearEruption : FlagellantCardModel
{
    private decimal _calculatedStress = 0;
    public FearEruption() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithStress(1,1);
        WithCalculatedDamage(0, ((CardModel card, Creature? c) =>
        {
            if (card != null && card is FearEruption myCard)
            {
                if(myCard._calculatedStress != 0)
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
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        _calculatedStress = 0;
    }
}
