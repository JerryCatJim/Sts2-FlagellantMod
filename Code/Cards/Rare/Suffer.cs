using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Suffer : FlagellantCardModel
{
    public Suffer() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Suffer");
        WithPowerTip<SufferPower>();
        WithPowerTip<DoomPower>();
        WithPowerTip<StressPower>();
        WithHealingPercent(1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<SufferPower>(choiceContext, this, base.DynamicVars["HealingPercent"].BaseValue);
    }
}
