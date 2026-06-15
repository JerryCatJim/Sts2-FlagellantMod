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
public class PainStrike : FlagellantCardModel
{
    public PainStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithPowerTip<StressPower>();
        WithVar("Multiplier", 1, 1);
        WithCalculatedDamage(9, ((CardModel card, Creature? c) =>
        {
            decimal multiDamage = card.DynamicVars["Multiplier"]?.BaseValue ?? 0;
            int stressNum = card.Owner.Creature.HasPower<StressPower>() ? (card.Owner.Creature?.GetPower<StressPower>()?.Amount ?? 0) : 0;
            return stressNum * multiDamage;
        }
        ));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
    }
}
