using BaseLib.Hooks;
using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class JuniasHead : FlagellantRelicModel, IHealAmountModifier
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    /*public bool TryModifyHpAmountReceived(Creature creature, decimal amount, out decimal modifiedAmount, bool silent)
    {
        if (amount <= 0m || !CombatManager.Instance.IsInProgress || CombatManager.Instance.IsOverOrEnding || creature == null || creature != Owner.Creature)
        {
            modifiedAmount = amount;
            return false;
        }
        if (!silent)
        {
            Flash();
        }
        modifiedAmount = amount + 1;
        return true;
    }*/
    public decimal ModifyHealAdditive(Creature creature, decimal amount)
    {
        if (amount <= 0m || !CombatManager.Instance.IsInProgress || CombatManager.Instance.IsOverOrEnding || creature == null || creature != Owner.Creature)
        {
            return 0m;
        }
        return 1m;
    }
}