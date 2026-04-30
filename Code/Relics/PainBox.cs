using Flagellant.Code.Abstract;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Relics;

public class PainBox : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner.Creature)
        {
            return amount;
        }
        if (Owner.Creature.CurrentHp <= amount) //已经是除去格挡值后的伤害了
        {
            //PowerCmd.Apply<DoomPower>(Owner.Creature, Owner.Creature.CurrentHp, Owner.Creature, null);
            PowerCmd.Apply<DoomPower>(Owner.Creature, amount, Owner.Creature, null);
            return 0m;
        }
        return amount;
    }
}