using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public sealed class DoomedConstitutionPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (CombatManager.Instance.IsInProgress && creature == base.Owner
            && delta < 0)// && base.CombatState.CurrentSide == base.Owner.Side)
        {
            await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), base.CombatState.HittableEnemies, -delta * Amount, base.Owner, null);
        }
    }
}
