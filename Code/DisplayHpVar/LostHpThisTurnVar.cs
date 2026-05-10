using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.DisplayHpVar;
public sealed class LostHpThisTurnVar : DynamicVar
{
    public const string defaultName = "LostHpThisTurn";

    public LostHpThisTurnVar(decimal baseValue)
        : base("LostHpThisTurn", baseValue)
    {

    }
    public LostHpThisTurnVar(string name, decimal baseValue) : base(name, baseValue)
    {

    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        BaseValue = LostHpThisTurnNum(card.Owner.Creature);
    }
    protected decimal LostHpThisTurnNum(Creature creature)
    {
        if (creature == null) return 0m;

        var entry = CombatManager.Instance.History.Entries
        .OfType<DamageReceivedEntry>()
        .Where(e => e.HappenedThisTurn(creature.CombatState)
            && e.Receiver == creature
            && e.Result.UnblockedDamage > 0);
        return entry?.Sum(e => e.Result.UnblockedDamage) ?? 0m;
    }
}
