using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Powers;

public sealed class ExplodeInSilencePower : FlagellantPowerModel, IAfterStressChanged
{
    private class Data
    {
        public int stressGained;

        public int triggerCount;
    }

    private const int _energyIncrement = 4;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => _energyIncrement - GetInternalData<Data>().stressGained % _energyIncrement;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StressPower>(),
        HoverTipFactory.ForEnergy(this)
    ];

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DynamicVar("GainStressTimes", _energyIncrement)
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
            data.stressGained += 1;  // amount;
            int triggers = data.stressGained / _energyIncrement - data.triggerCount;
            if (triggers > 0 && base.Owner.Player != null)
            {
                Flash();
                await PlayerCmd.GainEnergy(base.Amount * triggers, base.Owner.Player);
                data.triggerCount += triggers;
            }
            InvokeDisplayAmountChanged();
        }
    }
}
