using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Flagellant.Code.Powers;

public sealed class ManiacFormPower : FlagellantPowerModel, IModifyHpAmountReceived
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        
    ];
    public bool TryModifyHpAmountReceived(Creature creature, decimal amount, out decimal modifiedAmount)
    {
        if (amount <= 0m || base.CombatState == null || creature == null || creature != Owner)
        {
            modifiedAmount = amount;
            return false;
        }
        modifiedAmount = amount + Amount;
        return true;
    }
}
