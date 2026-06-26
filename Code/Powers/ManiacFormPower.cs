using Flagellant.Code.Abstract;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Powers;

public sealed class ManiacFormPower : FlagellantPowerModel, IOnResoluteOrMeltdownChanged
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnResoluteOrMeltdownChanged(PlayerChoiceContext choiceContext, Player player, ResoluteOrMeltdownModel oldRM, ResoluteOrMeltdownModel newRM)
    {
        if(newRM is ToxicMeltdown && base.Owner.Player != null && base.Owner.Player == player)
        {
            await PlayerCmd.GainEnergy(Amount, player);
            await CardPileCmd.Draw(choiceContext, Amount, player);
        }
    }
}
