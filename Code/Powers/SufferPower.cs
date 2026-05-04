using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Flagellant.Code.Cards.Rare;

namespace Flagellant.Code.Powers;

public sealed class SufferPower : FlagellantPowerModel, IAfterStressChanged
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>(),
        HoverTipFactory.FromPower<StressPower>()
    ];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (base.CombatState.CurrentSide != base.Owner.Side || delta <= 0m || creature == null || creature != Owner) return;
        DoomPower? DPwr = creature.GetPower<DoomPower>();
        if (DPwr != null)
        {
            await PowerCmd.ModifyAmount(DPwr, -Math.Round(delta), creature, ModelDb.Card<Suffer>());
        }
    }
    public async Task AfterStressAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (base.CombatState.CurrentSide != base.Owner.Side) return;
        await CreatureCmd.Heal(base.Owner, GetHealingPercentHp(Amount));
    }
}
