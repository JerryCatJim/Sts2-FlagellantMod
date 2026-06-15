using Flagellant.Code.Abstract;
using Flagellant.Code.Powers;
using Flagellant.Code.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.DisplayHpVar;
public sealed class HealingMaxHpVar : DynamicVar
{
    public const string defaultName = "HealingMaxHp";
    public string postFix = "";

    public HealingMaxHpVar(decimal baseValue)
        : base("HealingMaxHp", baseValue)
    {
        
    }
    public HealingMaxHpVar(string name, decimal baseValue) : base(name, baseValue)
    {
        if(name.Contains(defaultName))
        {
            postFix = name.Replace(defaultName, "");
        }
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        BaseValue = GetHealingPercentHp(card);
        if(card.IsInCombat)
        {
            //Creature myTarget = (card.TargetType != TargetType.Self && target != null) ? target : card.Owner.Creature;
            Creature myTarget = card.Owner.Creature;
            PreviewValue = BaseValue + GetExtraHealingHp(myTarget);
        }
    }
    private decimal GetHealingPercentHp(CardModel card)
    {
        if (card == null) return 0m;

        decimal Percent = 0m;
        Percent = Math.Clamp(card.DynamicVars["HealingPercent" + postFix]?.BaseValue ?? Percent, 0, 100);
        decimal Healing = Math.Round(card.Owner.Creature.MaxHp * Percent / 100m);
        if (Percent > 0 && Healing < 1m)
        {
            return 1m;
        }
        return Healing;
    }
    private decimal GetExtraHealingHp(Creature creature)
    {
        decimal num = 999;
        IRunState? runState = creature.Player?.RunState;
        if(runState != null)
        {
            foreach (AbstractModel item in runState.IterateHookListeners(creature.CombatState))
            {
                if (item is IModifyHpAmountReceived myModel)
                {
                    myModel.TryModifyHpAmountReceived(creature, num, out var myModifiedAmount, true);
                    num = myModifiedAmount;
                }
            }
        }
        return num - 999;
    }
}
