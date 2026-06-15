using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Flagellant.Code.Powers;

public sealed class ManiacFormPower : FlagellantPowerModel, IModifyHpAmountReceived
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public bool TryModifyHpAmountReceived(Creature creature, decimal amount, out decimal modifiedAmount, bool silent)
    {
        if (amount <= 0m || base.CombatState == null || creature == null || creature != Owner)
        {
            modifiedAmount = amount;
            return false;
        }
        if(!silent)
        {
            Flash();
        }
        modifiedAmount = amount + Amount;
        return true;
    }
}
