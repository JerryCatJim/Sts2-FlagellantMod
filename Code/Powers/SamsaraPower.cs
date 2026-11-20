using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Powers;

public sealed class SamsaraPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => true;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        Flash();
        if (Amount < 0)
        {
            await CreatureCmd.Damage(choiceContext, Owner, -Amount, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, base.Owner, null, null);
        }
        else if (Amount > 0)
        {
            await CreatureCmd.Heal(Owner, Amount);
        }
        await PowerCmd.Remove(this);
    }
}
