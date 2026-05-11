using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class Emancipation : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not DoomPower || amount <= 0m)
            return;

        Flash();
        await PowerCmd.Apply<StressPower>(Owner.Creature, 1, Owner.Creature, null);
    }
}