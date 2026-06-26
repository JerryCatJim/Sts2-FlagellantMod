using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Flagellant.Code.ResoluteOrMeltdown;
using Flagellant.Code.Abstract;

namespace Flagellant.Code.Events;

public class FlagellantHook
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
}