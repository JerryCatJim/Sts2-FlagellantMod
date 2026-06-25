using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class ExplodeInSilence : FlagellantCardModel
{
    public ExplodeInSilence() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("More");
        WithEnergy(1);
        WithPowerTip<ExplodeInSilencePower>();
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<ExplodeInSilencePower>(choiceContext, this, base.DynamicVars.Energy.BaseValue);
    }
}
