using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Undying : FlagellantCardModel
{
    public Undying() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Undying");
        WithPower<UndyingPower>(1,1);
        //WithCostUpgradeBy(-1);
        //WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<UndyingPower>(choiceContext, this);
    }
}
