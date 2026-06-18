using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class BreakingDead : FlagellantCardModel
{
    public BreakingDead() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<BreakingDeadPower>(1);
        WithPowerTip<PoisonPower>();
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<BreakingDeadPower>(choiceContext, this);
    }
}
