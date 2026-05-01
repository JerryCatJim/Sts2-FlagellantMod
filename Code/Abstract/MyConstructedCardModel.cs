using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BaseLib.Abstracts;

public abstract class MyConstructedCardModel(
    int canonicalEnergyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true) :
    ConstructedCardModel(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary)
{
    protected MyConstructedCardModel WithPowerTip<T>() where T : PowerModel
    {
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromPower<ComboPower>()));
        return this;
    }
    protected MyConstructedCardModel WithHpLoss(int baseVal, int upgrade = 0)
    {
        WithVars(new HpLossVar(baseVal).WithUpgrade(upgrade));
        return this;
    }
    protected MyConstructedCardModel WithStress(int baseVal, int upgrade = 0)
    {
        WithPower<StressPower>(baseVal, upgrade);
        return this;
    }
    protected MyConstructedCardModel WithPoison(int baseVal, int upgrade = 0)
    {
        WithPower<PoisonPower>(baseVal, upgrade);
        return this;
    }
    protected MyConstructedCardModel WithLossPercent(int baseVal, int upgrade = 0)
    {
        WithVar("LossPercent", baseVal, upgrade);
        return this;
    }
    protected MyConstructedCardModel WithHealingPercent(int baseVal, int upgrade = 0)
    {
        WithVar("HealingPercent", baseVal, upgrade);
        return this;
    }
    protected decimal GetLossPercentHp(decimal overridePercent = 0m)
    {
        decimal Percent = 0m;
        if(overridePercent == 0m)
        {
            bool isLossPercentExist = base.DynamicVars["LossPercent"] != null;
            Percent = Math.Clamp(isLossPercentExist ? base.DynamicVars["LossPercent"].BaseValue : Percent, 0, 100);
        }
        else
        {
            Percent = Math.Clamp(overridePercent, 0, 100);
        }
        decimal Damage = Math.Round(base.Owner.Creature.CurrentHp * Percent / 100m);
        if(Percent > 0 && Damage < 1m)
        {
            return 1m;
        }
        return Damage;
    }
    protected decimal GetHealingPercentHp(decimal overridePercent = 0m)
    {
        decimal Percent = 0m;
        if (overridePercent == 0m)
        {
            bool isHealPercentExist = base.DynamicVars["HealingPercent"] != null;
            Percent = Math.Clamp(isHealPercentExist ? base.DynamicVars["HealingPercent"].BaseValue : Percent, 0, 100);
        }
        else
        {
            Percent = Math.Clamp(overridePercent, 0, 100);
        }
        return Math.Round(base.Owner.Creature.MaxHp * Percent / 100m);
    }
}
