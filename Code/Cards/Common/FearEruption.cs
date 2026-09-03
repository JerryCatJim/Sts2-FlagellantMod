using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Hooks;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class FearEruption : FlagellantCardModel
{
    private decimal _calculatedStress = 0;
    public FearEruption() : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithStress(1, 1);
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
                    decimal delta = 0m;
                    if (myCard.DynamicVars.TryGetValue("StressPower", out var dynamicVar))
                    {
                        delta = dynamicVar.BaseValue;
                    }
                    return DD2Hooks.ModifyStressPower(myCard.Owner.Creature.CombatState, myCard.Owner.Creature.GetPower<StressPower>(), delta, myCard.Owner.Creature, myCard.Owner.Creature, myCard);
                }
            }
            return 0;
        }
        ));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _calculatedStress = DD2Hooks.ModifyStressPower(Owner.Creature.CombatState, Owner.Creature.GetPower<StressPower>(), DynamicVars["StressPower"].BaseValue, Owner.Creature, Owner.Creature, this);
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        _calculatedStress = 0;
    }
}
