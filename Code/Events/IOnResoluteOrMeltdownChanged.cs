using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Flagellant.Code.ResoluteOrMeltdown;

namespace Flagellant.Code.Events;

public interface IOnResoluteOrMeltdownChanged
{
    Task OnResoluteOrMeltdownChanged(PlayerChoiceContext ctx, Player player, ResoluteOrMeltdownModel oldStance, ResoluteOrMeltdownModel newStance);
}