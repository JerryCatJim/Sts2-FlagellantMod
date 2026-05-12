using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Powers;

public sealed class BlackBloodPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (CombatManager.Instance.IsInProgress && target == base.Owner
            && result.UnblockedDamage > 0 && base.CombatState.CurrentSide == base.Owner.Side)
        {
            await CreatureCmd.GainBlock(base.Owner, result.UnblockedDamage * Amount, ValueProp.Move, null);
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, result.UnblockedDamage * Amount, ValueProp.Unpowered | ValueProp.SkipHurtAnim, base.Owner, null);
        }
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
