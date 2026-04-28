using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagellant.Code.Abstract;
using Flagellant.Code.Powers;

namespace Flagellant.Code.Relics;

public class PainBox : FlagellantRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature == Owner.Creature && delta < 0)
        {
            await PowerCmd.Apply<RelicPower>(Owner.Creature, 2m, Owner.Creature, null);
        }
    }
}