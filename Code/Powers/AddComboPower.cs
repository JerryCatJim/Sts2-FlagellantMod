using BaseLib.Utils;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Flagellant.Code.Powers;

public sealed class AddComboPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter; 
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ComboPower>()
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay == null) return;

        if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay.Card.Type == CardType.Attack)
        {
            if (cardPlay.Card.TargetType == TargetType.AnyEnemy && cardPlay.Target != null)
            {
                Flash();
                await CommonActions.Apply<ComboPower>(cardPlay.Target, cardPlay.Card, 1);
                await PowerCmd.Decrement(this);
            }
            else if(cardPlay.Card.TargetType == TargetType.AllEnemies)
            {
                Flash();
                await PowerCmd.Apply<ComboPower>(base.CombatState.HittableEnemies, 1, Owner, cardPlay.Card);
                await PowerCmd.Decrement(this);
            }
        }
    }
}
