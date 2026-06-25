using BaseLib.Abstracts;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Singleton;

public class FlagellantCombatSingleton : CustomSingletonModel, IAfterStressChanged
{
    public FlagellantCombatSingleton() : base(HookType.Combat)
    {

    }

    public static decimal GainedStress { get; set; } = 0;
    public Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount > 0 && power != null && LocalContext.IsMe(power.Owner.Player))
        {
            GainedStress += amount;
        }
        return Task.CompletedTask;
    }
    public static void ResetValue()
    {
        GainedStress = 0;
    }
}