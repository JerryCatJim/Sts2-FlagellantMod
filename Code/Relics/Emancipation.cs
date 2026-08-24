using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class Emancipation : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [

    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("LowHealthPercent", 30m),
        new DynamicVar("AdditionalDamagePercent", 25m)
    ];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (props.HasFlag(ValueProp.Unpowered) || base.Owner.Creature.CombatState == null)
        {
            return 1m;
        }

        decimal AdditionalDamagePercent = DynamicVars.ContainsKey("AdditionalDamagePercent") ? DynamicVars["AdditionalDamagePercent"].BaseValue : 0m;
        return dealer == base.Owner.Creature && IsLowHealth() ? 1m + AdditionalDamagePercent / 100m : 1m;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature == base.Owner.Creature)
        {
            base.Status = IsLowHealth() ? RelicStatus.Active : RelicStatus.Normal;
        }
        return Task.CompletedTask;
    }

    public override Task AfterObtained()
    {
        base.Status = IsLowHealth() ? RelicStatus.Active : RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        base.Status = IsLowHealth() ? RelicStatus.Active : RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        base.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    private bool IsLowHealth()
    {
        if (base.Owner.Creature == null) return false;

        decimal Percent = DynamicVars.ContainsKey("LowHealthPercent") ? DynamicVars["LowHealthPercent"].BaseValue : 0m;
        return (decimal)base.Owner.Creature.CurrentHp / (decimal)base.Owner.Creature.MaxHp * 100m <= Percent;
    }
}