using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Powers;

public sealed class UntilDeathPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [

    ];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (props.HasFlag(ValueProp.Unpowered) || base.Owner.CombatState == null)
        {
            return 1m;
        }

        if (dealer == base.Owner || target == base.Owner)
        {
            decimal damageMulti = 1m + (decimal)base.Amount / 100m;
            //如果自己打自己应该把伤害倍数乘起来，虽然一般不会这样，但还是做一下计算
            return dealer == base.Owner && target == base.Owner ? damageMulti * damageMulti : damageMulti;
        }
        return 1m;
    }
}
