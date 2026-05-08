using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Commands;
using Flagellant.Code.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public sealed class ComboPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        //PowerCmd.Remove不会触发ModifyAmount，所以不会走进这里，Decrement可以
        //而我触发Combo后都是直接Remove的......如果需要监听Combo减少的变化，需要改为Decrement
        if (amount == 0m || power is not ComboPower || applier == null || applier.Player == null) return;

        await BroadcastComboChangedEvent(power, amount, applier, cardSource);
    }
    private Task BroadcastComboChangedEvent(PowerModel power, decimal amount, Creature applier, CardModel? cardSource)
    {
        if (amount == 0m || applier == null) return Task.CompletedTask;

        foreach (PowerModel PM in applier.GetPowerInstances<PowerModel>())
        {
            if (PM is IAfterComboChanged target)
            {
                target.AfterComboChanged(power, amount, applier, cardSource);
            }
        }

        if (applier.Player == null) return Task.CompletedTask;

        foreach (RelicModel RM in applier.Player.Relics)
        {
            if (RM is IAfterComboChanged target)
            {
                target.AfterComboChanged(power, amount, applier, cardSource);
            }
        }
        return Task.CompletedTask;
    }
}