using BaseLib.Utils;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;

namespace Flagellant.Code.Cards.Ancient;

[Pool(typeof(FlagellantCardPool))]
public class Rapturous : FlagellantCardModel
{
    public Rapturous() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithAnimName("More");
        WithPower<RapturousPower>(1);
        WithPowerTip<StressPower>();
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<RapturousPower>(choiceContext, this);
    }
}
