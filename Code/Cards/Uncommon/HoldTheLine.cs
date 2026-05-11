using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class HoldTheLine : FlagellantCardModel
{
    public HoldTheLine() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<HoldTheLinePower>(1);
        WithPower<ComboPower>(1);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<HoldTheLinePower>(this, base.DynamicVars["HoldTheLinePower"].BaseValue);
    }
}
