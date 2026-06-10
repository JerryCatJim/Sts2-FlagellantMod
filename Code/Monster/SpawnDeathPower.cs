using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

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
                bool ShouldSpawnDeath = !target.CombatState.Enemies.Any((Creature c) => 
                (c.IsPrimaryEnemy && c.IsAlive) || 
                (c.Monster is TestSubject ts && !ts.ShouldDisappearFromDoom)); //实验体前两次死了还在Enemies里

                if (ShouldSpawnDeath)
                {
                    //如果爪牙存活时击杀了主人，发现爪牙不会自动死亡，所以清理一下
                    await CreatureCmd.Kill(target.CombatState.HittableEnemies); //Kill(CombatState.Enemies)会直接闪退?

                    //拥有ShouldCreatureBeRemovedFromCombatAfterDeath的power的怪，彻底死后需要在CombatState将其移除才能让与死神的战斗在结束后正常结算
                    IEnumerable<Creature> UnremovedCreatures = target.CombatState.Enemies.ToList();
                    foreach (Creature creature in UnremovedCreatures)
                    {
                        if (creature.Powers.Any((PowerModel p) => !p.ShouldCreatureBeRemovedFromCombatAfterDeath(creature)))
                        {
                            NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(creature);
                            if (nCreature != null)
                            {
                                nCreature.AnimHideIntent();
                                nCreature.DeathAnimationTask = TaskHelper.RunSafely(DeleteNCreature(nCreature));
                                nCreature.StartDeathAnim(true);
                                NCombatRoom.Instance?.RemoveCreatureNode(nCreature);
                            }
                        }
                    }

                    await Cmd.CustomScaledWait(2.5f, 2.5f);
                    Death DeathMonster = (Death)ModelDb.Monster<Death>().ToMutable();
                    await CreatureCmd.Add(DeathMonster, target.CombatState, target.Side, null);

                    //若在死神生成前清除CombatState残留怪物会生不出死神?
                    foreach (Creature creature in UnremovedCreatures)
                    {
                        if (creature.Side == CombatSide.Enemy && (creature.CombatState?.Enemies.Contains(creature) ?? false))
                        {
                            CombatManager.Instance.RemoveCreature(creature);
                            MonsterModel? monster = creature.Monster;
                            if (monster != null && !monster.IsPerformingMove)
                            {
                                creature.CombatState.RemoveCreature(creature);
                            }
                        }
                    }
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

    private Task DeleteNCreature(NCreature nCreature)
    {
        nCreature.QueueFreeSafely();
        return Task.CompletedTask;
    }
}
