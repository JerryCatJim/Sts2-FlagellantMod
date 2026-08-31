using Flagellant.Code.Abstract;
using Flagellant.Code.Powers;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.Hooks;

public class DD2Hooks
{
    private static async Task Dispatch<T>(PlayerChoiceContext ctx, Player player, Func<T, Task> invoke)
        where T : class
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            ctx.PushModel(abstractModel);
            await invoke(model);
            ctx.PopModel(abstractModel);
        }
    }

    private static TResult Aggregate<T, TResult>(CombatState combatState, TResult seed,
        Func<T, TResult, TResult> action)
        where T : class =>
        combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (current, model) => action(model, current));

    public static Task OnResoluteOrMeltdownChanged(PlayerChoiceContext ctx, Player player, ResoluteOrMeltdownModel oldRM, ResoluteOrMeltdownModel newRM)
        => Dispatch<IOnResoluteOrMeltdownChanged>(ctx, player, m => m.OnResoluteOrMeltdownChanged(ctx, player, oldRM, newRM));

    public static decimal ModifyHealingHp(Creature? creature, decimal originalHealing)
    {
        if (creature == null) return originalHealing;

        decimal num = originalHealing;
        IRunState? runState = creature.Player?.RunState;  //不用combatState，保留在战斗外监听血量回复的可能
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
        return num;
    }
    
    public static decimal ModifyStressPower(StressPower? stressPower, decimal deltaAmount, Creature? applier, Creature target, CardModel? cardSource)
    {
        if (stressPower == null || stressPower.CombatState == null) return deltaAmount;

        decimal modifiedAmount = stressPower.Amount + deltaAmount;
        if (applier != null && stressPower.CombatState.ContainsCreature(applier))
        {
            modifiedAmount = Hook.ModifyPowerAmountGiven(stressPower.CombatState, stressPower, applier, modifiedAmount, target, cardSource, out IEnumerable<AbstractModel>_);
        }
        modifiedAmount = Hook.ModifyPowerAmountReceived(stressPower.CombatState, stressPower, target, modifiedAmount, applier, out IEnumerable<AbstractModel>_);
        return modifiedAmount;
    }
}