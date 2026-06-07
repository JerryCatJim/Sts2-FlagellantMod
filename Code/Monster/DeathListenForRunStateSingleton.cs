using BaseLib.Abstracts;
using Flagellant.Code.Config;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace Flagellant.Code.Monster;

public class DeathListenForRunStateSingleton : CustomSingletonModel
{
    public DeathListenForRunStateSingleton() : base(HookType.Run)
    {
        
    }

    //Bug : 关闭游戏后再打开游戏会导致次数清零，单例也没法用[SavedProperty]保存，专门搞个计数遗物也没必要，先不管了
    public static int DeathAppearTime { get; set; } = 0;
    public static bool ShouldSpawnDeathThisRoom { get; set; } = false;

    public static CombatState? CombatState { get; set; }

    private static int _hitCount = 0;
    public static int HitCount
    {
        get
        {
            return _hitCount;
        }
        set
        {
            _hitCount = value % 5;
        }
    }

    public static bool IsDeathExistingInCombat { get; set; } = false;

    public static void ResetValue(bool ShouldClearDeathAppearTime = true)
    {
        DeathAppearTime = ShouldClearDeathAppearTime ? 0 : DeathAppearTime;
        ShouldSpawnDeathThisRoom = false;
        CombatState = null;
        HitCount = 0;
        IsDeathExistingInCombat = false;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        NAudioManager.Instance?.SetBgmVol(SaveManager.Instance.SettingsSave.VolumeBgm);

        ResetValue(false);
        if (room is CombatRoom combatRoom)
        {
            CombatState = combatRoom.CombatState;
            IsDeathExistingInCombat = CombatState.HittableEnemies.Any((Creature c) => c.IsMonster && c.Monster is Death);
            ShouldSpawnDeathThisRoom = CheckSpawnDeathCondition(combatRoom);

            if(ShouldSpawnDeathThisRoom)
            {
                PowerCmd.Apply<SpawnDeathPower>(combatRoom.CombatState.HittableEnemies, 1, null, null);
            }
        }
        return Task.CompletedTask;
    }

    private static int GetRandomIndex(IRunState runState, int a, int b, int c, int N)
    {
        if (N <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(N), "N must be greater than 0.");
        }
        // a: 1~3 (如楼层)
        // b: 1~十几 (如房间号)
        // c: 房间内第一个怪物的血量
        // N: 数组长度，固定传入100

        uint seed = runState.Rng.Seed; //123456789u;               //盐值，增加分散度

        seed = (seed ^ (uint)a) * 0x9E3779B9u; // 混入 a
        seed = (seed ^ (uint)b) * 0x85EBCA6Bu; // 混入 b
        seed = (seed ^ (uint)c) * 0x7A3CFD3Bu; // 混入 c

        // 额外扩散：让高位和低位互相影响
        seed ^= (seed >> 16);
        seed *= 0x85EBCA6Bu;
        seed ^= (seed >> 13);
        seed *= 0x7A3CFD3Bu;
        seed ^= (seed >> 16);

        return (int)(seed % (uint)N);
    }

    private bool CheckSpawnDeathCondition(AbstractRoom room)
    {
        if (room is CombatRoom combatRoom)
        {
            if (FlagellantConfig.ShouldDeathOnlyHuntFlagellant
                && !combatRoom.CombatState.RunState.Players.Any((Player p) => p.Character is Character.Flagellant))
            {
                return false;
            }

            if (combatRoom.Encounter.IsWeak || DeathAppearTime >= FlagellantConfig.DeathAppearMaxTime)
            {
                return false;
            }

            int EncounterChance = Math.Clamp(FlagellantConfig.DeathEncounterChance, 0, 100);
            if (EncounterChance <= 0)
            {
                return false;
            }

            if (CheckCombatRoomTypeForSpawningDeath(combatRoom))
            {
                //按理说应该只让这个Rng方法在所有玩家内只执行一次，多次执行会多人模式数据不同步，但是我没找到好的位置，所以自己写个哈希凑合一下
                //int index0 = combatRoom.CombatState.RunState.Rng.UpFront.NextInt(0, 99);
                IRunState RunState = combatRoom.CombatState.RunState;
                int index0 = GetRandomIndex(RunState, RunState.CurrentActIndex, RunState.TotalFloor, RunState.ActFloor, 100);
                if (index0 < EncounterChance)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckCombatRoomTypeForSpawningDeath(CombatRoom room)
    {
        return (FlagellantConfig.ShouldDeathAppearInMonsterRoom && room.RoomType == RoomType.Monster)
            || (FlagellantConfig.ShouldDeathAppearInEliteRoom && room.RoomType == RoomType.Elite)
            || (FlagellantConfig.ShouldDeathAppearInBossRoom && room.RoomType == RoomType.Boss);
    }
}