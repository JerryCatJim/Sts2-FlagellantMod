using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class PainBox : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner.Creature || amount <= 0m)
        {
            return amount;
        }
        if (Owner.Creature.CurrentHp <= amount) //已经是除去格挡值后的伤害了
        {
            Flash();
            //PowerCmd.Apply<DoomPower>(Owner.Creature, Owner.Creature.CurrentHp, Owner.Creature, null);
            PowerCmd.Apply<DoomPower>(Owner.Creature, amount, Owner.Creature, null);
            return 0m;
        }
        return amount;
    }
    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<DeathsHead>();
    }
}