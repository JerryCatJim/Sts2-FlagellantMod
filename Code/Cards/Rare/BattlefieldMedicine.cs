using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class BattlefieldMedicine : FlagellantCardModel
{
    public BattlefieldMedicine() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<RegenPower>(2,1);
        WithCards(1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RegenPower>(this);
        await CommonActions.Draw(this, choiceContext);
    }
}
