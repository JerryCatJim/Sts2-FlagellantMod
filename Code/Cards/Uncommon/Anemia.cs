using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Anemia : FlagellantCardModel
{
    public Anemia() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<AnemiaPower>(1,1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<AnemiaPower>(choiceContext, this);
    }
}
