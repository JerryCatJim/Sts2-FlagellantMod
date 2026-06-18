using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class MoreMore : FlagellantCardModel
{
    public MoreMore() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithAnimName("More");
        WithPower<WeakPower>(2);
        WithStress(2);
        WithPower<ComboPower>(1);
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.None);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.Apply<WeakPower>(choiceContext, base.CombatState.HittableEnemies, this);
        await CommonActions.Apply<ComboPower>(choiceContext, base.CombatState.HittableEnemies, this);
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
    }
}
