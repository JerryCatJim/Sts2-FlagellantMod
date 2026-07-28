using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class WarFeedsWar : FlagellantCardModel
{
    public WarFeedsWar() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<WarFeedsWarPower>(1);
        WithCostUpgradeBy(-1);
        WithVar("GainStressAmount", 5);
        WithPower<ComboPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<WarFeedsWarPower>(choiceContext, this);
    }
}
