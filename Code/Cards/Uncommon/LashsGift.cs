using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class LashsGift : FlagellantCardModel
{
    public LashsGift() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Lash");
        WithPower<RegenPower>(4);
        WithStress(2);
        WithPower<AddComboPower>(1, 1);
        WithPower<ComboPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<RegenPower>(choiceContext, this);
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
        await CommonActions.ApplySelf<AddComboPower>(choiceContext, this);
    }
}
