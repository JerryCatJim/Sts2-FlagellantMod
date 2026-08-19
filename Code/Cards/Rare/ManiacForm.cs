using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Core;
using Flagellant.Code.Powers;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class ManiacForm : FlagellantCardModel
{
    public ManiacForm() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("More");
        WithPower<ManiacFormPower>(1);
        WithTip(new TooltipSource((CardModel _) => FlagellantHoverTipFactory.FromResoluteOrMeltdown<ToxicMeltdown>()));
        WithEnergy(1);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<ManiacFormPower>(choiceContext, this);
    }
}
