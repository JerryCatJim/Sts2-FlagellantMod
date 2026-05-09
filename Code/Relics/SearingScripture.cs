using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class SearingScripture : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            await PowerCmd.Apply<AddComboPower>(Owner.Creature, 1 ,Owner.Creature, null);
        }
    }
}