using BaseLib.Utils;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class FatalStrike : FlagellantCardModel
{
    public FatalStrike() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithStress(3);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal LostHp = Owner.Creature.MaxHp - Owner.Creature.CurrentHp;
        await CommonActions.CardAttack(this, cardPlay.Target, LostHp).Execute(choiceContext);
        await CommonActions.ApplySelf<StressPower>(this, -base.DynamicVars["StressPower"].BaseValue);
    }
}
