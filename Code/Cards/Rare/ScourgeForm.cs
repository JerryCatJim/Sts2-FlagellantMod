using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class ScourgeForm : FlagellantCardModel
{
    public ScourgeForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("More");
        WithPower<ScourgeFormPower>(2);
        WithPowerTip<PoisonPower>();
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<ScourgeFormPower>(choiceContext, this);
    }
}
