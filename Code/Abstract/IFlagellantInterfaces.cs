using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Abstract;
public interface IAfterStressChanged
{
    public Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource);
}
public interface IModifyHpAmountReceived
{
    public bool TryModifyHpAmountReceived(Creature creature, decimal amount, out decimal modifiedAmount, bool silent)
    {
        modifiedAmount = amount;
        return false;
    }
}
public interface IAfterComboChanged
{
    public Task AfterComboChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel? cardSource);
}

public interface IOnResoluteOrMeltdownChanged
{
    public Task OnResoluteOrMeltdownChanged(PlayerChoiceContext choiceContext, Player player, ResoluteOrMeltdownModel oldRM, ResoluteOrMeltdownModel newRM);
}

public interface IModifyHpPercentEnterToxicAdditional
{
    public bool TryModifyHpPercentEnterToxicAdditional(Creature creature, decimal amount, out decimal modifiedAmount, bool silent)
    {
        modifiedAmount = amount;
        return false;  //是否打断后续其他相同接口的修改
    }
}