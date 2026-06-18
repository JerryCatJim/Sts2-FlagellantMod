using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class BeingTowardsDeath : FlagellantCardModel
{
    public BeingTowardsDeath() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Lash");
        WithPower<BeingTowardsDeathPower>(2);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<BeingTowardsDeathPower>(choiceContext, this);
    }
}
