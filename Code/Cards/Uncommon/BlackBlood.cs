using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class BlackBlood : FlagellantCardModel
{
    public BlackBlood() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<BlackBloodPower>(1);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.Static(StaticHoverTip.Block)));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<BlackBloodPower>(choiceContext, this);
    }
}
