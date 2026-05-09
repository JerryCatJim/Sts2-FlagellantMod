using Flagellant.Code.Abstract;
using Flagellant.Code.Commands;
using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public sealed class StressPower : FlagellantPowerModel
{
    public override bool AllowNegative => true; //Creature.cs里的InvokePowerModified()要求 AllowNegative==true才能把层数减少时间传过来
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        FlagellantHoverTipFactory.FromResoluteOrMeltdown<ToxicMeltdown>()
    ];

    public override async Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        var player = Owner.Player;
        if (power is not StressPower || amount == 0m || applier != Owner || player == null)
            return;

        if(Amount < 0)
        {
            //不太完美，会有负数的StressPowerIcon一闪而过，但我追踪到Powercmd里尝试重写PowerModel的TryModifyPowerAmountReceived没有效果，先这样吧
            await PowerCmd.Remove(this);
            if(amount - Amount != 0m)
            {
                await BroadcastStressChangedEvent(power, amount - Amount, applier, cardSource);
            }
            return;
        }

        await BroadcastStressChangedEvent(power, amount, applier, cardSource);

        //满10点压力触发美德或者折磨判定，触发后把压力值归零
        if (Amount >= 10)
        {
            //await PowerCmd.ModifyAmount(this, -Amount, Owner, cardSource);
            await PowerCmd.Remove(this);
            await BroadcastStressChangedEvent(power, -Amount, applier, cardSource);
            var ctx = new ThrowingPlayerChoiceContext();
            await RMCmd.EnterResoluteOrMeltdownRandomly(ctx, player, cardSource);
        }
    }
    
    private Task BroadcastStressChangedEvent(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount == 0m) return Task.CompletedTask;

        //给房间内人物的所有Power和玩家的所有遗物广播压力变化事件
        foreach(PowerModel PM in Owner.GetPowerInstances<PowerModel>())
        {
            if (PM is IAfterStressChanged target)
            {
                target.AfterStressAmountChanged(power, amount, applier, cardSource);
            }
        }

        if(Owner.Player == null) return Task.CompletedTask;

        foreach (RelicModel RM in Owner.Player.Relics)
        {
            if (RM is IAfterStressChanged target)
            {
                target.AfterStressAmountChanged(power, amount, applier, cardSource);
            }
        }
        return Task.CompletedTask;
    }
}
