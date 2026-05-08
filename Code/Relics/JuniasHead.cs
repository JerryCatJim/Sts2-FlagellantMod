using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class JuniasHead : FlagellantRelicModel, IModifyHpAmountReceived
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public bool TryModifyHpAmountReceived(Creature creature, decimal amount, out decimal modifiedAmount)
    {
        if (amount <= 0m || Owner.Creature.CombatState == null || creature == null || creature != Owner.Creature)
        {
            //战斗刚结束，还未进入下一房间时CombatState还不为空，但应该没人会在这个时间点回血，否则他会发现多回了1点。
            modifiedAmount = amount;
            return false;
        }
        Flash();
        modifiedAmount = amount + 1;
        return true;
    }
}