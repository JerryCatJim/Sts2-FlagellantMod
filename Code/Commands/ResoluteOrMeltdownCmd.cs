using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Commands;

public static class RMCmd
{
    public static Task EnterResoluteOrMeltdownRandomly(PlayerChoiceContext ctx, Player player, CardModel? cardSource, bool isFlagellant = true)
    {
        if(isFlagellant)
        {
            return EnterToxic(ctx, player, cardSource);
        }
        return EnterRandomResoluteOrMeltdown(ctx, player ,cardSource);
    }
    public static Task EnterToxic(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        return FlagellantModel.SetResoluteOrMeltdown<ToxicMeltdown>(ctx, player, cardSource);
    }

    public static Task EnterResoluteOrMeltdown<T>(PlayerChoiceContext ctx, Player player, CardModel? cardSource) where T : ResoluteOrMeltdownModel
    {
        return FlagellantModel.SetResoluteOrMeltdown<T>(ctx, player, cardSource);
    }

    public static Task ExitResoluteOrMeltdown(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        return FlagellantModel.SetResoluteOrMeltdown<NoResoluteAndMeltdown>(ctx, player, cardSource);
    }
    private static Task EnterRandomResoluteOrMeltdown(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        //You can also use official RNG class to get syncable random values.
        int handCount = player.PlayerCombatState?.Hand?.Cards?.Count ?? 0;
        int roundNumber = player.Creature?.CombatState?.RoundNumber ?? 0;
        int index1 = (player.RunState.TotalFloor * 7 + roundNumber * 13 + handCount * 3) % 10;
        int index2 = (player.RunState.TotalFloor * 7 + roundNumber * 13 + handCount * 3) % Enum.GetNames(typeof(ResoluteType)).Length;
        int index3 = (player.RunState.TotalFloor * 7 + roundNumber * 13 + handCount * 3) % Enum.GetNames(typeof(MeltdownType)).Length;

        bool isMeltdown = index1 >= 2; //0到9,Meltdown概率80%，所以2到9为Meltdown, 0和1为Resolute
        if(isMeltdown)
        {
            return index2 switch
            {
                //0 => EnterResoluteOrMeltdown<YourResoluteClass>(ctx, player, cardSource),
                //1 => and so on...
                //_ => EnterResoluteOrMeltdown<YourResoluteClassDefault>(ctx, player, cardSource),
            };
        }
        else
        {
            return index3 switch
            {
                //0 => EnterResoluteOrMeltdown<YourMeltdownClass>(ctx, player, cardSource),
                //1 => and so on...
                //_ => EnterResoluteOrMeltdown<YourMeltdownClassDefault>(ctx, player, cardSource),
            };
        }
    }
}