using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public sealed class CorpseInstabilityPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (CombatManager.Instance.IsInProgress && creature == base.Owner
            && delta != 0 && base.CombatState.CurrentSide == base.Owner.Side)
        {
            Flash();
            await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), base.CombatState.HittableEnemies, Amount, base.Owner, null);
        }
    }
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))  //if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
