using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Powers;

public class SacrificePower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _usedThisTurn;
    public bool UsedThisTurn
    {
        get
        {
            return _usedThisTurn;
        }
        private set
        {
            if (_usedThisTurn != value)
            {
                AssertMutable();
                _usedThisTurn = value;
            }
        }
    }
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (CombatManager.Instance.IsInProgress && target == base.Owner
            && result.UnblockedDamage > 0 && base.CombatState.CurrentSide == base.Owner.Side
            && !UsedThisTurn && base.Owner != null && base.Owner.Player != null)
        {
            Flash();
            UsedThisTurn = true;
            await PlayerCmd.GainEnergy(Amount, base.Owner.Player);
        }
    }
    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        //if (side != base.Owner.Side) return Task.CompletedTask;
        if(!participants.Contains(base.Owner)) return Task.CompletedTask;

        UsedThisTurn = false;
        return Task.CompletedTask;
    }
}