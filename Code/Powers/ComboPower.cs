using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public sealed class ComboPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool AllowNegative => true;
    public override async Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        //PowerCmd.Remove不会触发ModifyAmount，所以不会走进这里，Decrement可以
        //而我触发Combo后都是直接Remove的......如果需要监听Combo减少的变化，需要改为Decrement
        if (amount == 0m || power is not ComboPower || power != this || applier == null || applier.Player == null) return;

        await BroadcastComboChangedEvent(power, amount, applier, cardSource);
    }
    private Task BroadcastComboChangedEvent(PowerModel power, decimal amount, Creature applier, CardModel? cardSource)
    {
        if (amount == 0m || applier == null) return Task.CompletedTask;
        
        foreach (AbstractModel item in base.Owner.CombatState.IterateHookListeners())
        {
            if (item is IAfterComboChanged myModel)
            {
                myModel.AfterComboChanged(power, amount, applier, cardSource);
            }
        }
        return Task.CompletedTask;
    }
}