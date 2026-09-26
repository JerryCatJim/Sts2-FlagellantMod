using BaseLib.Utils;
using DD2ModConfig.Code.Helper;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Combat;
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
public class PainBox : FlagellantRelicModel
{
    private decimal _amount = 0;
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress || target != Owner.Creature || amount <= 0m)
        {
            return amount;
        }
        if (Owner.Creature.CurrentHp <= amount) //已经是除去格挡值后的伤害了
        {
            Flash();
            _amount = amount;
            return 0m;
        }
        return amount;
    }
    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (_amount > 0)
        {
            await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, _amount, Owner.Creature, null);
            _amount = 0;
        }
    }
    public override Task AfterCombatVictory(CombatRoom room)
    {
        DD2Helper.ClearStressAfterCombatVictory(Owner);
        return Task.CompletedTask;
    }

    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<DarkImpulse>();
    }
}