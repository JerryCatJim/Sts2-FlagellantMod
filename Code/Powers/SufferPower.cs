using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Rare;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;

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

    public async Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0m || power.Owner != base.Owner) return;

        Flash();
        decimal healAmount = GetHealingPercentHp(Amount);
        await CreatureCmd.Heal(base.Owner, healAmount);
        if (base.Owner.GetPower<DoomPower>() is DoomPower DPwr)
        {
            Flash();
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), DPwr, -(healAmount + GetExtraHealingHp(base.Owner)), base.Owner, ModelDb.Card<Suffer>());
        }
    }

    private decimal GetExtraHealingHp(Creature creature)
    {
        decimal num = 999;
        IRunState? runState = creature.Player?.RunState;
        if (runState != null)
        {
            foreach (AbstractModel item in runState.IterateHookListeners(creature.CombatState))
            {
                if (item is IModifyHpAmountReceived myModel)
                {
                    myModel.TryModifyHpAmountReceived(creature, num, out var myModifiedAmount, true);
                    num = myModifiedAmount;
                }
            }
        }
        return num - 999;
    }
}
