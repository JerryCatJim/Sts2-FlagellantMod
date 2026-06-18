using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Anemia : FlagellantCardModel
{
    public Anemia() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<AnemiaPower>(1);
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.Static(StaticHoverTip.Block)));
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<AnemiaPower>(choiceContext, this);
    }
}
