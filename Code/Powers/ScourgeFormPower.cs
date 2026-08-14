using Flagellant.Code.Abstract;
using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Flagellant.Code.Powers;

public sealed class ScourgeFormPower : FlagellantPowerModel, IModifyHpPercentEnterToxicAdditional
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        FlagellantHoverTipFactory.FromResoluteOrMeltdown<ToxicMeltdown>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AdditionalHpPercent", 15)
    ];

    public bool TryModifyHpPercentEnterToxicAdditional(Creature creature, decimal amount, out decimal modifiedAmount, bool silent)
    {
        if (amount <= 0m || Owner.CombatState == null || CombatManager.Instance.IsOverOrEnding || creature == null || creature != Owner)
        {
            modifiedAmount = amount;
            return false;
        }
        modifiedAmount = amount + DynamicVars["AdditionalHpPercent"].BaseValue;
        return false;
    }
}
