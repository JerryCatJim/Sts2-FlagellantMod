using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Trudge : FlagellantCardModel
{
    public Trudge() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithHealingPercent(10, 3);
        WithDamage(10, 3);
        WithPower<ComboPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp());
        await CommonActions.Apply<ComboPower>(choiceContext, cardPlay.Target, this);
    }
}
