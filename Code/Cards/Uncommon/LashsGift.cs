using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class LashsGift : FlagellantCardModel
{
    public LashsGift() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Lash");
        WithBlock(8, 4);
        WithStress(1);
        WithPower<AddComboPower>(1);
        WithPower<ComboPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<StressPower>(this);
        await CommonActions.ApplySelf<AddComboPower>(this);
    }
}
