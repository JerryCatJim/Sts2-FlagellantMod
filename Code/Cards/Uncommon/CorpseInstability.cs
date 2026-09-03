using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class CorpseInstability : FlagellantCardModel
{
    public CorpseInstability() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<CorpseInstabilityPower>(2, 1);
        WithPowerTip<PoisonPower>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<CorpseInstabilityPower>(choiceContext, this);
    }
}
