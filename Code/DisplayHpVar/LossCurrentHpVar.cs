using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.DisplayHpVar;
public sealed class LossCurrentHpVar : DynamicVar
{
    public const string defaultName = "LossCurrentHp";
    public string postFix = "";

    public LossCurrentHpVar(decimal baseValue)
        : base("LossCurrentHp", baseValue)
    {
        
    }
    public LossCurrentHpVar(string name, decimal baseValue) : base(name, baseValue)
    {
        if (name.Contains(defaultName))
        {
            postFix = name.Replace(defaultName, "");
        }
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        BaseValue = GetLossPercentHp(card);
    }
    private decimal GetLossPercentHp(CardModel card)
    {
        if (card == null) return 0m;

        decimal Percent = 0m;
        Percent = Math.Clamp(card.DynamicVars["LossPercent" + postFix]?.BaseValue ?? Percent, 0, 100);
        decimal Damage = Math.Round(card.Owner.Creature.CurrentHp * Percent / 100m, MidpointRounding.AwayFromZero);
        if (Percent > 0 && Damage < 1m)
        {
            return 1m;
        }
        return Damage;
    }
}
