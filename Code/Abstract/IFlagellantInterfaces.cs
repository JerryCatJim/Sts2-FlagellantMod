using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Abstract;
public interface IAfterStressChanged
{
    public Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource);
}
public interface IModifyHpAmountReceived
{
    //New version source code changes the CombatState Class into Interface ICombatState.
    public bool TryModifyHpAmountReceived(Creature creature, decimal amount, out decimal modifiedAmount, bool silent)
    {
        modifiedAmount = amount;
        return false;
    }
}
public interface IAfterComboChanged
{
    public Task AfterComboChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel? cardSource);
}