using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Core;
using Flagellant.Code.Powers;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class ScourgeForm : FlagellantCardModel
{
    public ScourgeForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Deathless");
        WithPower<ScourgeFormPower>(1);
        WithPower<ToxicFormPower>(2);
        WithPowerTip<PoisonPower>();
        WithTip(new TooltipSource((CardModel _) => FlagellantHoverTipFactory.FromResoluteOrMeltdown<ToxicMeltdown>()));
        WithCostUpgradeBy(-1);
        WithVar("AdditionalHpPercent", 15);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<ScourgeFormPower>(choiceContext, this);
        await CommonActions.ApplySelf<ToxicFormPower>(choiceContext, this);
    }
}
