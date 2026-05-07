using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public sealed class ScourgeFormPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (delta >= 0m || creature == null || creature != Owner) return;

        await PowerCmd.Apply<PoisonPower>(base.CombatState.HittableEnemies, base.Amount, base.Owner, null);
    }
}
