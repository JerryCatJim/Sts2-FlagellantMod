using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Commands;

public static class RMCmd
{
    public static Task TryEnterResoluteOrMeltdown(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        if(player.Character is Flagellant.Code.Character.Flagellant)
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
        int playerMaxHP = player.Creature?.MaxHp ?? 0;
        int floorCount  = player.RunState?.TotalFloor ?? 0;
        int roundNumber = player.Creature?.CombatState?.RoundNumber ?? 0;

        //int index0 = player.PlayerRng.Transformations.NextInt(0, 9);
        int index1 = GetRandomIndex(player, playerMaxHP, floorCount, roundNumber, 10);
        int index2 = GetRandomIndex(player, playerMaxHP, floorCount, roundNumber, Enum.GetNames(typeof(ResoluteType)).Length);
        int index3 = GetRandomIndex(player, playerMaxHP, floorCount, roundNumber, Enum.GetNames(typeof(MeltdownType)).Length);

        bool isMeltdown = index1 >= 2; //0到9,Meltdown概率80%，所以2到9为Meltdown, 0和1为Resolute
        if(isMeltdown)
        {
            return index2 switch
            {
                //0 => EnterResoluteOrMeltdown<YourResoluteClass>(ctx, player, cardSource),
                //1 => and so on...
                //_ => EnterResoluteOrMeltdown<YourResoluteClassDefault>(ctx, player, cardSource),
                _ => EnterResoluteOrMeltdown<NoResoluteAndMeltdown>(ctx, player, cardSource)
            };
        }
        else
        {
            return index3 switch
            {
                //0 => EnterResoluteOrMeltdown<YourMeltdownClass>(ctx, player, cardSource),
                //1 => and so on...
                //_ => EnterResoluteOrMeltdown<YourMeltdownClassDefault>(ctx, player, cardSource),
                _ => EnterResoluteOrMeltdown<NoResoluteAndMeltdown>(ctx, player, cardSource)
            };
        }
    }
    public static int GetRandomIndex(Player player, int a, int b, int c, int N)
    {
        if (N <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(N), "N must be greater than 0.");
        }

        ulong seed = player.PlayerRng.Seed;     //盐值，增加分散度

        seed = (seed ^ (ulong)a) * 0x9E3779B9u; // 混入 a
        seed = (seed ^ (ulong)b) * 0x85EBCA6Bu; // 混入 b
        seed = (seed ^ (ulong)c) * 0x7A3CFD3Bu; // 混入 c

        // 额外扩散：让高位和低位互相影响
        seed ^= (seed >> 16);
        seed *= 0x85EBCA6Bu;
        seed ^= (seed >> 13);
        seed *= 0x7A3CFD3Bu;
        seed ^= (seed >> 16);

        return (int)(seed % (ulong)N);
    }
}