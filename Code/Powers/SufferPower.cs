using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Rare;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public sealed class SufferPower : FlagellantPowerModel, IAfterStressChanged
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>(),
        HoverTipFactory.FromPower<StressPower>()
    ];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (base.CombatState.CurrentSide != base.Owner.Side || delta <= 0m || creature == null || creature != Owner) return;
        DoomPower? DPwr = creature.GetPower<DoomPower>();
        if (DPwr != null)
        {
            Flash();
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), DPwr, -Math.Round(delta) * Amount, creature, ModelDb.Card<Suffer>());
        }
    }
    public async Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (base.CombatState.CurrentSide != base.Owner.Side || amount <= 0m || power.Owner != base.Owner) return;

        Flash();
        await CreatureCmd.Heal(base.Owner, GetHealingPercentHp(Amount));
    }
}
