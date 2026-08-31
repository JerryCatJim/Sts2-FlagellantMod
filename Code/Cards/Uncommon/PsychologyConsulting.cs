using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Hooks;
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
        WithVar("Multiplier", 1, 1);
        WithCalculatedVar("CalculatedHealing", 0,
            ((CardModel card, Creature? target) =>
            {
                if (card.Owner.Creature == null || !card.Owner.Creature.HasPower<StressPower>()) return 0;

                if (card is PsychologyConsulting myCard)
                {
                    decimal multiNum = 0m;
                    if (myCard.DynamicVars.TryGetValue("Multiplier", out var dynamicVar))
                    {
                        multiNum = dynamicVar.BaseValue;
                    }
                    decimal healingAmount = (myCard.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0) * multiNum;
                    return healingAmount > 0 ? DD2Hooks.ModifyHealingHp(myCard.Owner.Creature, healingAmount) : 0;
                }
                return 0;
            }
            ));
        WithKeyword(CardKeyword.Ethereal);
        WithCards(2);
        WithAnimName("Deathless");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        
        if (base.Owner.Creature.GetPower<StressPower>() is StressPower SP)
        {
            decimal healingAmount = SP.Amount * DynamicVars["Multiplier"].BaseValue;
            await PowerCmd.ModifyAmount(choiceContext, SP, -SP.Amount, Owner.Creature, this);
            if (healingAmount > 0)
            {
                await CreatureCmd.Heal(Owner.Creature, healingAmount);
            }
        }
        await CommonActions.Draw(this, choiceContext);
    }
}
