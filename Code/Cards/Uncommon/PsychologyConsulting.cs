using BaseLib.Cards.Variables;
using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class PsychologyConsulting : FlagellantCardModel
{
    public PsychologyConsulting() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("Multiplier", 2, 1);
        WithCalculatedVar("CalculatedHealing", 0,
            ((CardModel card, Creature? target) =>
            {
                if (card.Owner.Creature == null || !card.Owner.Creature.HasPower<StressPower>()) return 0;

                if(card is PsychologyConsulting myCard)
                {
                    decimal healingAmount = (myCard.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0) * myCard.DynamicVars["Multiplier"].BaseValue;
                    healingAmount += healingAmount > 0 ? myCard.GetExtraHealingHp(myCard.Owner.Creature) : 0;
                    return healingAmount;
                }
                return 0;
            }
            ));
        WithKeyword(CardKeyword.Ethereal);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal healingAmount = 0;
        CustomCalculatedVar? MyVar = base.DynamicVars["CalculatedHealing"] as CustomCalculatedVar;
        if(MyVar != null)
        {
            healingAmount = MyVar.CalculateCustom(base.Owner.Creature);
            healingAmount -= healingAmount > 0 ? GetExtraHealingHp(base.Owner.Creature) : 0;
        }
        if (base.Owner.Creature.GetPower<StressPower>() is StressPower SP)
        {
            await PowerCmd.ModifyAmount(choiceContext, SP, -SP.Amount, Owner.Creature, this);
        }
        if(healingAmount > 0)
        {
            await CreatureCmd.Heal(Owner.Creature, healingAmount);
        }
    }
}
