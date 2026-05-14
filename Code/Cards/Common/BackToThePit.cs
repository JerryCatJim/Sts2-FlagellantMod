using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class BackToThePit : FlagellantCardModel
{
    public BackToThePit() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(12,3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }
        if (base.IsClone)
        {
            return Task.CompletedTask;
        }

        int amount = CombatManager.Instance.History.Entries.
            OfType<DamageReceivedEntry>().
            Count((DamageReceivedEntry e) => e.HappenedThisTurn(Owner.Creature.CombatState)
                && e.Receiver == Owner.Creature
                && e.Result.UnblockedDamage > 0);
        ReduceCostBy(amount);
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        //这个事件会广播给deck内的卡牌，所以记得排除
        if (!IsInCombat) return Task.CompletedTask;

        if (delta >= 0m || creature != Owner.Creature || base.CombatState == null || CombatManager.Instance.IsOverOrEnding
            || base.CombatState.CurrentSide != base.Owner.Creature.Side)
        {
            return Task.CompletedTask;
        }
        ReduceCostBy(1);
        return Task.CompletedTask;
    }
    private void ReduceCostBy(int amount)
    {
        base.EnergyCost.AddThisTurn(-amount);
    }
}
