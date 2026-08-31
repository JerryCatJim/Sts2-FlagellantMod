using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Rare;
using Flagellant.Code.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public sealed class SufferPower : FlagellantPowerModel, IAfterStressChanged
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("HealingHp", 0)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>(),
        HoverTipFactory.FromPower<StressPower>()
    ];

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner || (power is not SufferPower && power is not WeightTrainingPower)) return Task.CompletedTask;

        DynamicVars["HealingHp"].BaseValue = DD2Hooks.ModifyHealingHp(Owner, GetHealingPercentHp(Amount));
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public async Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0m || power.Owner != base.Owner) return;

        Flash();
        decimal healAmount = GetHealingPercentHp(Amount);
        await CreatureCmd.Heal(base.Owner, healAmount);
        if (base.Owner.GetPower<DoomPower>() is DoomPower DPwr)
        {
            Flash();
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), DPwr, -DD2Hooks.ModifyHealingHp(Owner, healAmount), base.Owner, ModelDb.Card<Suffer>());
        }
    }
}
