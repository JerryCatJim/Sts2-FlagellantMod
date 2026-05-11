using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class ThrillingTablet : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        Flash();
        Creature creature = base.Owner.Creature;
        await CreatureCmd.GainMaxHp(creature, creature.MaxHp);
        await CreatureCmd.Heal(creature, creature.MaxHp-creature.CurrentHp);
    }
}