using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Flagellant.Code.DisplayHpVar;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Abstract;

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
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromPower<T>()));
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
        WithLossCurrentHpDisplay();
        return this;
    }
    protected MyConstructedCardModel WithHealingPercent(int baseVal, int upgrade = 0)
    {
        WithVar("HealingPercent", baseVal, upgrade);
        WithHealingMaxHpDisplay();
        return this;
    }
    protected MyConstructedCardModel WithLossCurrentHpDisplay()
    {
        WithVars(new LossCurrentHpVar(0));
        return this;
    }
    protected MyConstructedCardModel WithHealingMaxHpDisplay()
    {
        WithVars(new HealingMaxHpVar(0));
        return this;
    }
    protected MyConstructedCardModel WithLostHpThisTurnDisplay()
    {
        WithVars(new LostHpThisTurnVar(0));
        return this;
    }
    protected decimal GetLossPercentHp(decimal overridePercent = 0m)
    {
        decimal Percent = 0m;
        if(overridePercent == 0m)
        {
            Percent = Math.Clamp(base.DynamicVars["LossPercent"]?.BaseValue ?? Percent, 0, 100);
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
            Percent = Math.Clamp(base.DynamicVars["HealingPercent"]?.BaseValue ?? Percent, 0, 100);
        }
        else
        {
            Percent = Math.Clamp(overridePercent, 0, 100);
        }
        decimal Healing =  Math.Round(base.Owner.Creature.MaxHp * Percent / 100m);
        if (Percent > 0 && Healing < 1m)
        {
            return 1m;
        }
        return Healing;
    }
    protected decimal GetStressBeforeReceived()
    {
        //目前只有RapturousPower会预先修改Stress的获得量(Stress一旦大于等于10点就会立刻归零，没啥好地方去接收归零前的实际值，先这样取巧地修改一下吧)
        decimal rapturousAmount = base.Owner.Creature.GetPower<RapturousPower>()?.Amount ?? 0;
        if (base.DynamicVars.ContainsKey("StressPower"))
        {
            return base.DynamicVars["StressPower"].BaseValue + (base.DynamicVars["StressPower"].BaseValue > 0 ? rapturousAmount : 0);
        }
        else
        {
            return 0;
        }
    }
}
