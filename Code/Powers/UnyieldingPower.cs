using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public sealed class UnyieldingPower : FlagellantPowerModel
{
    private bool _shouldDecrease = false;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        if (canonicalPower != null && canonicalPower is DoomPower && amount > 0m && target == Owner)
        {
            Flash();
            _shouldDecrease = true;
            modifiedAmount = 0;
            return true;
        }
        modifiedAmount = amount;
        return false;
    }
    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (_shouldDecrease)
        {
            await PowerCmd.Decrement(this);
            _shouldDecrease = false;
        }
    }
}
