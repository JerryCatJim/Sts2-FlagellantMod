using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class DarkImpulse : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];

    public int DoomPowerAmount = 0;

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner.Creature || amount <= 0m)
        {
            return amount;
        }
        if (Owner.Creature.CurrentHp <= amount && Owner.Creature.CombatState != null) //已经是除去格挡值后的伤害了
        {
            Flash();
            PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, amount, Owner.Creature, null);
            return 0m;
        }
        return amount;
    }
    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Owner.Creature.GetPower<DoomPower>() is DoomPower doomPower)
        {
            DoomPowerAmount = doomPower.Amount;
        }
        return base.AfterCombatEnd(room);
    }
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        //CombatManager.cs的EndCombatInternal里会调用player2.AfterCombatEnd(),其中会清空所有Power,所以要在此之前记录DoomPower层数
        if (!base.Owner.Creature.IsDead)
        {
            if (DoomPowerAmount > 0)
            {
                Flash();
                await CreatureCmd.Heal(base.Owner.Creature, DoomPowerAmount);
            }
        }
        DoomPowerAmount = 0;
    }
}