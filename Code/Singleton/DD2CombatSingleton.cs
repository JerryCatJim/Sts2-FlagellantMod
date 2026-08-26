using BaseLib.Abstracts;
using Flagellant.Code.Abstract;
using Flagellant.Code.Audio;
using Flagellant.Code.Config;
using Flagellant.Code.Helper;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.Singleton;

public class DD2CombatSingleton : CustomSingletonModel, IAfterStressChanged, IAfterDeathDoor
{
    public DD2CombatSingleton() : base(HookType.Combat)
    {

    }

    public static Dictionary<ulong, decimal> GainedStressDictionary = new Dictionary<ulong, decimal> { };
    public static Dictionary<ulong, int> StressLockDictionary = new Dictionary<ulong, int> { };
    public Task AfterStressAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount > 0 && power != null && power.Owner != null && power.Owner.Player != null)
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
        StressLockDictionary.Clear();
    }

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        StressLockDictionary.Clear();
        return Task.CompletedTask;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (delta < 0 && creature.IsAlive && (100m * creature.CurrentHp / creature.MaxHp) <= (decimal)FlagellantConfig.ShowDeathDoorVfxHpPercent)
        {
            await BroadcastDeathDoorEvent(creature, delta, 0, IsLowHealthTrigger : true);
        }
        else if (DD2Helper.WillDieInDoom(creature) != DD2Helper.WillDieInDoom(creature, delta, 0))
        {
            if (DD2Helper.WillDieInDoom(creature))
            {
                await BroadcastDeathDoorEvent(creature, delta, 0, false);
            }
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not DoomPower) return;

        Creature creature = power.Owner;
        if (DD2Helper.WillDieInDoom(creature) != DD2Helper.WillDieInDoom(creature, 0, amount))
        {
            if (DD2Helper.WillDieInDoom(creature))
            {
                await BroadcastDeathDoorEvent(creature, 0, amount, false);
            }
        }
    }

    public override Task BeforeDeath(Creature creature)
    {
        if (!ShouldPlayDeathBlowVfx(creature)) return Task.CompletedTask;

        //用AfterDeath会导致PlayDeathVfx内部取不到NCreature，因为它在AfterDeath通知之前被清理了
        DD2Helper.PlayDeathVfx(creature, "DeathBlow");
        return Task.CompletedTask;
    }

    public Task AfterDeathDoor(Creature creature, decimal healthDelta, decimal doomPowerDelta, bool IsLowHealthTrigger)
    {
        if (!ShouldPlayDeathDoorVfx(creature)) return Task.CompletedTask;
        if ((!IsLowHealthTrigger && FlagellantConfig.ShouldPlayDeathDoorVfxIfDoomed) 
            || (IsLowHealthTrigger && FlagellantConfig.ShouldPlayDeathDoorVfxIfLowHealth))
        {
            DD2Helper.PlayDeathVfx(creature, "DeathDoor");
        }
        return Task.CompletedTask;
    }

    private async Task BroadcastDeathDoorEvent(Creature creature, decimal healthDelta, decimal doomPowerDelta, bool IsLowHealthTrigger)
    {
        if ((healthDelta == 0m && doomPowerDelta == 0m)|| creature.CombatState == null) return;

        foreach (AbstractModel item in creature.CombatState.IterateHookListeners())
        {
            if (item is IAfterDeathDoor myModel)
            {
                await myModel.AfterDeathDoor(creature, healthDelta, doomPowerDelta, IsLowHealthTrigger);
            }
        }
    }

    private bool ShouldPlayDeathDoorVfx(Creature creature)
    {
        if (creature == null) return false;

        return (creature.IsPlayer && FlagellantConfig.ShouldPlayerShowDeathDoorVfx) ||
            (!creature.IsPlayer && FlagellantConfig.ShouldMonsterShowDeathDoorVfx);
    }
    private bool ShouldPlayDeathBlowVfx(Creature creature)
    {
        if (creature == null) return false;

        return (creature.IsPlayer && FlagellantConfig.ShouldPlayerShowDeathBlowVfx) ||
            (!creature.IsPlayer && FlagellantConfig.ShouldMonsterShowDeathBlowVfx);
    }
}