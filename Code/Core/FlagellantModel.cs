using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Flagellant.Code.Events;
using Flagellant.Code.ResoluteOrMeltdown;

namespace Flagellant.Code.Core;

public class FlagellantModel() : CustomSingletonModel(HookType.Combat)
{
    private static readonly SpireField<Player, ResoluteOrMeltdownModel> ActiveRM =
        new(FlagellantModelDb.ResoluteOrMeltdown<NoResoluteAndMeltdown>);


    public static ResoluteOrMeltdownModel GetResoluteOrMeltdownModel(Player player)
    {
        return ActiveRM[player] ?? FlagellantModelDb.ResoluteOrMeltdown<NoResoluteAndMeltdown>();
    }

    public static bool IsInResoluteOrMeltdown<T>(Player player) where T : ResoluteOrMeltdownModel
    {
        return ActiveRM[player] is T;
    }


    public static async Task SetResoluteOrMeltdown<T>(PlayerChoiceContext ctx, Player player, CardModel? source) where T : ResoluteOrMeltdownModel
    {
        await SetResoluteOrMeltdown(ctx, player, FlagellantModelDb.ResoluteOrMeltdown<T>(), source);
    }

    private static async Task SetResoluteOrMeltdown(PlayerChoiceContext ctx, Player player, ResoluteOrMeltdownModel newCanonical, CardModel? source)
    {
        var current = ActiveRM[player];
        //可以重复进入美德/折磨
        //if (current?.GetType() == newCanonical.GetType()) return;

        if (current != null)
            await current.OnExitResoluteOrMeltdown(ctx, player, source);

        var mutable = newCanonical.ToMutable(player);
        ActiveRM[player] = mutable;
        await mutable.OnEnterResoluteOrMeltdown(ctx, player, source);

        //var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        await FlagellantHook.OnResoluteOrMeltdownChanged(ctx, player, current!, ActiveRM[player]!);
    }

    public override Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;

        foreach (var player in state.Players)
        {
            ActiveRM[player] = FlagellantModelDb.ResoluteOrMeltdown<NoResoluteAndMeltdown>();
        }

        return Task.CompletedTask;
    }
    public override bool ShouldReceiveCombatHooks => true;
}