using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class ManiacForm : FlagellantCardModel
{
    public ManiacForm() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Deathless");
        WithPower<ManiacFormPower>(1);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<ManiacFormPower>(choiceContext, this);
    }
}
