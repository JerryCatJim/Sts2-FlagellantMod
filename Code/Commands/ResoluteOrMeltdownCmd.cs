using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Commands;

public enum ResoluteOrMeltdownType
{
    Toxic //You can add more types.
}

public static class RMCmd
{
    public static Task EnterResoluteOrMeltdownRandomly(PlayerChoiceContext ctx, Player player, CardModel? cardSource, bool isFlagellant = true)
    {
        if(isFlagellant)
        {
            return EnterToxic(ctx, player, cardSource);
        }

        Array values = Enum.GetValues(typeof(ResoluteOrMeltdownType));
        ResoluteOrMeltdownType randomRM = (ResoluteOrMeltdownType)values.GetValue(new Random().Next(values.Length));

        switch(randomRM)
        {
            case ResoluteOrMeltdownType.Toxic:
            default:
            return EnterToxic(ctx, player, cardSource);
        }
    }
    public static Task EnterToxic(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        return FlagellantModel.SetResoluteOrMeltdown<ToxicMeltdown>(ctx, player, cardSource);
    }

    public static Task ExitResoluteOrMeltdown(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        return FlagellantModel.SetResoluteOrMeltdown<NoResoluteAndMeltdown>(ctx, player, cardSource);
    }

}