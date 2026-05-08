using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class DismassHead : FlagellantRelicModel, IAfterComboChanged
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public async Task AfterComboChanged(PowerModel power, decimal amount, Creature applier, CardModel? cardSource)
    {
        if (amount <= 0m || applier != Owner.Creature) return;

        Flash();
        await PowerCmd.Apply<VulnerablePower>(power.Owner, 1, Owner.Creature, null);
    }
}