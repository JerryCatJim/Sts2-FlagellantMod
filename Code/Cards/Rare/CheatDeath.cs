using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class CheatDeath : FlagellantCardModel
{
    private int _healthChangedTimes = 0;
    public CheatDeath() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithVar("HealthChangedTimes", 3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
    }
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        //这个事件会广播给deck内的卡牌，所以记得排除
        if (!IsInCombat) return;

        if (delta == 0m || creature != Owner.Creature || Owner.Creature.CombatState == null || CombatManager.Instance.IsOverOrEnding)
        {
            return;
        }

        ++_healthChangedTimes;
        if (_healthChangedTimes % 3 == 0 && base.Pile.Type != PileType.Hand)
        {
            await CardPileCmd.Add(this, PileType.Hand);
        }
        return;
    }
}
