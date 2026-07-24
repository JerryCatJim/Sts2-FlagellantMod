using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class DarkImpulse : FlagellantRelicModel
{
    private decimal _amount = 0;
    public override RelicRarity Rarity => RelicRarity.Ancient;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>(),
        HoverTipFactory.FromPower<StressPower>()
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
        if(_amount > 0)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSmokePuffVfx.Create(base.Owner.Creature, NSmokePuffVfx.SmokePuffColor.Purple));
            await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, _amount, Owner.Creature, null);
            _amount = 0;
        }
    }
    /*public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Flash();
            await PowerCmd.Apply<StressPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 5, Owner.Creature, null, true);
        }
    }*/
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner.Creature) && base.Owner.PlayerCombatState != null && base.Owner.PlayerCombatState.TurnNumber <= 1)
        {
            Flash();
            await PowerCmd.Apply<StressPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 5, Owner.Creature, null);
        }
    }
}