using BaseLib.Abstracts;
using Flagellant.Code.Config;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.Monster;

public class CheckDeathAppearSingleton : CustomSingletonModel
{
    public CheckDeathAppearSingleton() : base(true, true)
    {
    }

    //Bug : 关闭游戏后再打开游戏会导致次数清零，单例也没法用[SavedProperty]保存，专门搞个计数遗物也没必要，先不管了
    public static int DeathAppearTime { get; set; } = 0;

    public static bool ShouldSpawnDeathThisRoom { get; set; } = false;

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

    public static void ResetValue()
    {
        DeathAppearTime = 0;
        HitCount = 0;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        ShouldSpawnDeathThisRoom = false;
        if(room is CombatRoom combatRoom)
        {
            if(FlagellantConfig.ShouldDeathOnlyHuntFlagellant
                && !combatRoom.CombatState.RunState.Players.Any((Player p) => p.Character is Character.Flagellant))
            {
                return Task.CompletedTask;
            }

            if(DeathAppearTime >= FlagellantConfig.DeathAppearMaxTime)
            {
                return Task.CompletedTask;
            }
            
            int EncounterChance = Math.Clamp(FlagellantConfig.DeathEncounterChance, 0, 100);
            if(EncounterChance <= 0)
            {
                return Task.CompletedTask;
            }

            if(CheckRoomTypeForSpawningDeath(combatRoom))
            {
                //按理说应该只让这个Rng方法只在所有玩家内执行一次，多次执行会多人模式数据不同步，但是我没找到好的位置，所以自己写个哈希凑合一下
                //int index0 = combatRoom.CombatState.RunState.Rng.UpFront.NextInt(0, 99);
                int CurrentHP = 1;
                IRunState RunState = combatRoom.CombatState.RunState;
                Creature? FirstEnemy = combatRoom.CombatState.HittableEnemies.FirstOrDefault();
                if(FirstEnemy != null)
                {
                    CurrentHP = FirstEnemy.CurrentHp;
                }
                //只用TotalFloor和CurrentRoomCount的组合数太少，加入第三个混合数凑一下组合数
                int index0 = GetRandomIndex(RunState, RunState.TotalFloor, RunState.CurrentRoomCount, CurrentHP, 100);
                if(index0 < EncounterChance)
                {
                    ShouldSpawnDeathThisRoom = true;
                    PowerCmd.Apply<SpawnDeathPower>(combatRoom.CombatState.HittableEnemies, 1, null, null);
                }
            }
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if((creature.Monster is Axebot && !creature.HasPower<StockPower>())
            || creature.Monster is Wriggler)
        {
            if(ShouldSpawnDeathThisRoom == true)
            {
                await PowerCmd.Apply<SpawnDeathPower>(creature.CombatState.HittableEnemies, 1, null, null);
            }
        }
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

    private bool CheckRoomTypeForSpawningDeath(CombatRoom room)
    {
        return (FlagellantConfig.ShouldDeathAppearInMonsterRoom && room.RoomType == RoomType.Monster)
            || (FlagellantConfig.ShouldDeathAppearInEliteRoom && room.RoomType == RoomType.Elite)
            || (FlagellantConfig.ShouldDeathAppearInBossRoom && room.RoomType == RoomType.Boss);
    }
}