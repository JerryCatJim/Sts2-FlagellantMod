using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Harder : FlagellantCardModel
{
    public Harder() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithHealingPercent(10, 2);
        WithCards(1);
        WithAnimName("Lash");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp());
        await CommonActions.Draw(this, choiceContext);
    }
}
