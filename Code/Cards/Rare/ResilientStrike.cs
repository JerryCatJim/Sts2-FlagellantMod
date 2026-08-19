using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using Flagellant.Code.Singleton;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class ResilientStrike : FlagellantCardModel
{
    public ResilientStrike() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithPowerTip<StressPower>();
        WithCalculatedDamage(8, (CardModel card, Creature? c) =>
        {
            if (FlagellantCombatSingleton.GainedStressDictionary.ContainsKey(card.Owner.NetId))
            {
                return FlagellantCombatSingleton.GainedStressDictionary[card.Owner.NetId];
            }
            else
            {
                return 0;
            }
        }, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move, 4
        );
        WithAnimName("Punish");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }
}
