using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class DoomedConstitution : FlagellantCardModel
{
    public DoomedConstitution() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPowerTip<DoomPower>();
        WithPower<DoomedConstitutionPower>(1);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<DoomedConstitutionPower>(choiceContext, this);
    }
}
