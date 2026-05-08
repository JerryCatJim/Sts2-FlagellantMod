using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.Abstract;
public interface IAfterStressChanged
{
    public Task AfterStressAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource);
}
public interface IModifyHpAmountReceived
{
    //New version source code changes the CombatState Class into Interface ICombatState.
    public bool TryModifyHpAmountReceived(Creature creature, decimal amount, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        return false;
    }
}