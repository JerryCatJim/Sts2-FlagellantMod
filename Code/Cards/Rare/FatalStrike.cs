using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class FatalStrike : FlagellantCardModel
{
    public FatalStrike() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithStress(3);
        WithCostUpgradeBy(-1);
        WithCalculatedDamage(0, ((CardModel card, Creature? _) => card.Owner.Creature.MaxHp - card.Owner.Creature.CurrentHp));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        await CommonActions.ApplySelf<StressPower>(this, -base.DynamicVars["StressPower"].BaseValue);
    }
}
