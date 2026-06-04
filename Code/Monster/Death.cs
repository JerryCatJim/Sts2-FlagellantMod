using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Flagellant.Audio;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Monster;

public class Death : CustomMonsterModel
{
    // 根据进阶提高最小血量，进阶8及以上为A，否则为B
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 80, 110);

    // 根据进阶提高最大血量，进阶8及以上为A，否则为B
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 90, 120);

    // 意图1的数值，伤害和格挡，进阶9提高伤害
    private int BasicDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);
    private int BasicBlock => 8;

    // 意图2的数值，重击伤害，进阶9提高伤害
    private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 24, 20);

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://Flagellant/Monster_Death/Death.tscn");

    // 如果你挂载了自己的自定义脚本，使用这个(挂了也用上面的，这个会卡死)
    //public override string? CustomVisualPath => "res://Flagellant/Monster_Death/Death.tscn";

    private Node2D? _vfxInstance;

    // 战斗开始时，在这里给自己上buff之类
    public override Task AfterAddedToRoom()
    {
        _vfxInstance = PreloadManager.Cache.GetScene("res://Flagellant/Monster_Death/EnterEffect/EnterEffect.tscn").Instantiate<Node2D>();
        if (_vfxInstance == null) return Task.CompletedTask;

        _vfxInstance.Position = Vector2.Zero;
        _vfxInstance.Scale = Vector2.One;

        AudioManager.PlayMonsterSfx("Spawn", true, false, -4);
        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null)
        {
            if (_vfxInstance.GetParent() == null)
            {
                combatRoom.AddChild(_vfxInstance);
            }
        }

        //给SubViewportContainer挂了cs脚本，无需在代码中检测
        /*NCreature? node = NCombatRoom.Instance?.GetCreatureNode(base.Creature);
        if(node != null)
        {
            var MyNode = node.GetNodeOrNull<Node2D>("TestDeath");
            if (MyNode != null)
            {
                var SVC = MyNode.GetNodeOrNull<SubViewportContainer>("Visuals/SubViewportContainer");
                if(SVC != null)
                {
                    await Cmd.CustomScaledWait(3.5f, 3.5f);
                    SVC.Material = null;
                }
            }
        }*/
        return Task.CompletedTask;
    }

    public override async Task BeforeDeath(Creature creature)
    {
        if (creature != Creature) return;

        AudioManager.PlayMonsterSfx("Dead", true, false, -2);
        await CreatureCmd.TriggerAnim(Creature, "Dead", 0);
        await Cmd.CustomScaledWait(2f, 2f, true);

        if (_vfxInstance != null)
        {
            _vfxInstance.QueueFree();
        }
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        // 意图1：造成伤害，获得格挡
        var basicAttack = new MoveState(
            "BASIC_ATTACK", // 状态ID
            BasicAttackMove, // 执行函数，或者直接用lambda也可
                             // 以下是可变参数，可以填写任意数量的意图，全部展示
            new SingleAttackIntent(BasicDamage),
            new DefendIntent()
        );

        // 意图2：重击
        var heavyAttack = new MoveState(
            "HEAVY_ATTACK",
            async targets => await DamageCmd // 意图2实际执行效果，这里直接用lambda
                .Attack(HeavyDamage)
                .FromMonster(this)
                .WithAttackerAnim("Attack/Attack_B", 0f) // 如果有攻击动画，可以取消注释并替换成实际动画名称和延迟
                .WithWaitBeforeHit(1, 1)
                .WithAttackerFx(null, AttackSfx)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(null),
            new SingleAttackIntent(HeavyDamage)
        );

        // 或者你也可以创建RandomBranchState（随机意图分支）和ConditionalBranchState（条件意图分支）来实现更复杂的状态转换逻辑

        // 设置状态转换，意图1后接意图2，意图2后接意图1
        basicAttack.FollowUpState = heavyAttack;
        heavyAttack.FollowUpState = basicAttack;

        // 添加2个意图，并且初始意图设成 basicAttack
        return new MonsterMoveStateMachine([basicAttack, heavyAttack], basicAttack);
    }

    // 意图1执行实际效果
    private async Task BasicAttackMove(IReadOnlyList<Creature> targets)
    {
        // 说话
        //TalkCmd.Play(L10NMonsterLookup("TEST-TEST_MONSTER.moves.BASIC_ATTACK.banter"), Creature, VfxColor.Blue);
        await DamageCmd
            .Attack(BasicDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack/Attack_Point", 0f)
            .WithWaitBeforeHit(1, 1)
            .WithAttackerFx(null, AttackSfx) // 攻击音效
            .WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
            .Execute(null);
        await CreatureCmd.GainBlock(Creature, BasicBlock, ValueProp.Move, null);
    }
}