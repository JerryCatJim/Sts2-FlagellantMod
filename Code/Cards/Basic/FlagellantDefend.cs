using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Basic;

[Pool(typeof(FlagellantCardPool))]
public class FlagellantDefend : FlagellantCardModel
{
    public FlagellantDefend() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
	{
		WithTags(CardTag.Defend);
		WithBlock(5, 3);
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
	}
}
