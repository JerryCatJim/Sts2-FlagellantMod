using Flagellant.Code.Abstract;
using Flagellant.Code.Commands;
using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public sealed class StressPower : FlagellantPowerModel
{
    public override bool AllowNegative => true; //Creature.cs里的InvokePowerModified()要求 AllowNegative==true才能把层数减少的事件传过来
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        FlagellantHoverTipFactory.FromResoluteOrMeltdown<ToxicMeltdown>(),
        HoverTipFactory.FromPower<ScourgeFormPower>(),
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount == 0m || power is not StressPower || power != this)
            return;

        //有效的变化层数，例如Amount当前为-2，amount为-3，即修改之前Amount为1，减少了1层，所以传入的是-1
        decimal realChangedAmount = Amount < 0 ? amount - Amount : amount;
        if(realChangedAmount != 0m)
        {
            await BroadcastStressChangedEvent(choiceContext, power, realChangedAmount, applier, cardSource);
        }

        //满10点压力触发美德或者折磨判定，触发后把压力值归零
        if (Amount >= 10)
        {
            await BroadcastStressChangedEvent(choiceContext, power, -Amount, applier, cardSource);
            //若不立刻移除则会导致苦楚+极乐时进入怨毒时无限循环
            await SetAmountBorder();
            if (Owner.Player is Player player)
            {
                await RMCmd.TryEnterResoluteOrMeltdown(choiceContext, player, cardSource);
            }
        }
        await SetAmountBorder();
    }
    
    private async Task BroadcastStressChangedEvent(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount == 0m || base.Owner.CombatState == null) return;

        foreach (AbstractModel item in base.Owner.CombatState.IterateHookListeners())
        {
            if (item is IAfterStressChanged myModel)
            {
                await myModel.AfterStressAmountChanged(choiceContext, power, amount, applier, cardSource);
            }
        }
    }
    private async Task SetAmountBorder()
    {
        if (Amount < 0 || Amount >= 10)
        {
            await PowerCmd.Remove(this);
        }
    }
}
