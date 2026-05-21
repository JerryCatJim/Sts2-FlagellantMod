using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class WeightTraining : FlagellantCardModel
{
    public WeightTraining() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPowerTip<WeightTrainingPower>();
        WithStress(1);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<WeightTrainingPower>(this, base.DynamicVars["StressPower"].BaseValue);
    }
}
