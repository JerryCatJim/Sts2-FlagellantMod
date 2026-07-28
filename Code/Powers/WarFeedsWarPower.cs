using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public sealed class WarFeedsWarPower : FlagellantPowerModel, IAfterStressChanged
{
    private class Data
    {
        public int stressGained;

        public int triggerCount;
    }

    private const int _comboIncrement = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => _comboIncrement - GetInternalData<Data>().stressGained % _comboIncrement;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StressPower>(),
        HoverTipFactory.ForEnergy(this)
    ];

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("GainStressAmount", _comboIncrement)
    ];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public async Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner == base.Owner && amount > 0)
        {
            Data data = GetInternalData<Data>();
            data.stressGained += (int)amount;
            int triggers = data.stressGained / _comboIncrement - data.triggerCount;
            if (triggers > 0)
            {
                Flash();
                await PowerCmd.Apply<AddComboPower>(choiceContext, base.Owner, base.Amount * triggers, base.Owner, null);
                data.triggerCount += triggers;
            }
            InvokeDisplayAmountChanged();
        }
    }
}
