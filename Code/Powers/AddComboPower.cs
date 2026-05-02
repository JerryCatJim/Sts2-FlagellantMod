using BaseLib.Utils;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using System.Threading.Tasks;

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
        if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay.Card.Type == CardType.Attack)
        {
            if (cardPlay.Card != null && cardPlay.Target != null)
            {
                Flash();
                CommonActions.Apply<ComboPower>(cardPlay.Target, cardPlay.Card, 1);
                await PowerCmd.Decrement(this);
            }
            else if(cardPlay.Card != null && cardPlay.Card.TargetType == TargetType.AllEnemies)
            {
                Flash();
                CommonActions.Apply<ComboPower>(base.CombatState.HittableEnemies, cardPlay.Card);
                await PowerCmd.Decrement(this);
            }
        }
    }
}
