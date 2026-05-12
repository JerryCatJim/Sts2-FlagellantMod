using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class ExanimateForm : FlagellantCardModel
{
    public ExanimateForm() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Lash");
        WithPower<PoisonPower>(1);
        WithPowerTip<StressPower>();
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<ExanimateFormPower>(this, base.DynamicVars["PoisonPower"].BaseValue);
    }
}
