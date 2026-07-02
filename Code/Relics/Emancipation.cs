using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Linq;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class Emancipation : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StressPower>()
    ];

    public int _stressPowerAmount = 0;
    [SavedProperty]
    public int StressPowerAmount
    {
        get
        {
            return _stressPowerAmount;
        }
        set
        {
            AssertMutable();
            _stressPowerAmount = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => true;
    public override int DisplayAmount => StressPowerAmount;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Owner.Creature.GetPower<StressPower>() is StressPower stressPower)
        {
            StressPowerAmount = stressPower.Amount;
        }
        return base.AfterCombatEnd(room);
    }
    /*public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Flash();
            await PowerCmd.Apply<StressPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, StressPowerAmount, Owner.Creature, null, true);
            StressPowerAmount = 0;
        }
    }*/
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner.Creature) && base.Owner.PlayerCombatState != null && base.Owner.PlayerCombatState.TurnNumber <= 1)
        {
            Flash();
            await PowerCmd.Apply<StressPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, StressPowerAmount, Owner.Creature, null, true);
            StressPowerAmount = 0;
        }
    }
}