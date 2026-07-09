using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Sacrifice : FlagellantCardModel
{
    public Sacrifice() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPowerTip<SacrificePower>();
        WithEnergy(1,1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SacrificePower>(choiceContext, this, base.DynamicVars.Energy.BaseValue);
    }
}
