using Flagellant.Code.Abstract;
using Flagellant.Code.Config;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Monster;

public sealed class SpawnDeathPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.None;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => DeathListenForRunStateSingleton.ShouldPredictWhetherDeathWillAppear;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!wasRemovalPrevented && target == base.Owner && target.IsPrimaryEnemy && target.Monster is not Death
            && !target.HasPower<InfestedPower>() 
            && !target.HasPower<StockPower>() 
            && !target.HasPower<SurprisePower>()) //排除蛆,地精和斧头机器人的死亡后召唤power
        {
            if (target.CombatState != null)
            {
                bool ShouldSpawnDeath = true;
                foreach (Creature c in target.CombatState.Enemies)
                {
                    if ((c.IsPrimaryEnemy && c.IsAlive)
                        || (c.Monster is TestSubject ts && !ts.ShouldDisappearFromDoom)) //实验体前两次死了还在Enemies里
                    {
                        ShouldSpawnDeath = false;
                        break;
                    }
                }
                if (ShouldSpawnDeath)
                {
                    //如果爪牙存活时击杀了主人，发现爪牙不会自动死亡，所以清理一下
                    await CreatureCmd.Kill(target.CombatState.HittableEnemies);

                    await Cmd.CustomScaledWait(2.5f, 2.5f);
                    Death DeathMonster = (Death)ModelDb.Monster<Death>().ToMutable();
                    await CreatureCmd.Add(DeathMonster, target.CombatState, target.Side, null);
                }
            }
        }
    }
    public override bool ShouldStopCombatFromEnding()
    {
        return true;
    }
    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }
}
