using BaseLib.Abstracts;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Singleton;

public class FlagellantCombatSingleton : CustomSingletonModel, IAfterStressChanged
{
    public FlagellantCombatSingleton() : base(HookType.Combat)
    {

    }

    public static Dictionary<ulong, decimal> GainedStressDictionary = new Dictionary<ulong, decimal> { };
    public Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount > 0 && power != null && power.Owner!= null && power.Owner.Player != null)
        {
            if (!GainedStressDictionary.TryAdd(power.Owner.Player.NetId, amount))
            {
                // 添加失败，说明键已存在，手动更新
                GainedStressDictionary[power.Owner.Player.NetId] += amount;
            }
        }
        return Task.CompletedTask;
    }
    public static void ResetValue()
    {
        GainedStressDictionary.Clear();
    }
}