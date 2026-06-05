using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Flagellant.Audio;
using Flagellant.Code.Powers;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Monster;

public class Death : CustomMonsterModel
{
    // 根据进阶提高最小和最大血量，进阶8及以上为A，否则为B
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 80, 110);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 90, 120);

    // 意图的数值，进阶9提高
    private int MementoMoriDoomNum => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 13);
    private int WaningCrescentDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 12);
    private int SoulReaverDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 24, 20);
    private int TrampleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 6);
    private int TrampleStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 3);

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://Flagellant/Monster_Death/Death.tscn");

    // 如果你挂载了自己的自定义脚本，使用这个(挂了也用上面的，这个会卡死)
    //public override string? CustomVisualPath => "res://Flagellant/Monster_Death/Death.tscn";

    private Node2D? _vfxInstance;

    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {
        CheckDeathAppearSingleton.DeathAppearTime++;

        NCreature? DeathNode = NCombatRoom.Instance?.GetCreatureNode(base.Creature);
        if(DeathNode != null)
        {
            DeathNode.Visible = false;
            DeathNode.GlobalPosition = new Vector2(1417, 739);
            await Cmd.CustomScaledWait(0.1f, 0.1f);
            DeathNode.Visible = true;
        }

        _vfxInstance = PreloadManager.Cache.GetScene("res://Flagellant/Monster_Death/EnterEffect/EnterEffect.tscn").Instantiate<Node2D>();
        if (_vfxInstance == null) return;

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

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {

    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        // 你也可以创建RandomBranchState（随机意图分支）和ConditionalBranchState（条件意图分支）来实现更复杂的状态转换逻辑
        var MementoMori = new MoveState(
            "MEMENTO_MORI",
            MementoMoriMove,
            new DebuffIntent(true)
            );
        var SoulReaver = new MoveState(
            "SOUL_REAVER",
            SoulReaverMove,
            new SingleAttackIntent(SoulReaverDamage)
            );
        var WaningCrescent = new MoveState(
            "WANING_CRESCENT",
            WaningCrescentMove,
            new SingleAttackIntent(WaningCrescentDamage),
            new DefendIntent()
            );
        var Trample = new MoveState(
            "TRAMPLE",
            TrampleMove,
            new SingleAttackIntent(TrampleDamage),
            new BuffIntent(),
            new DebuffIntent()
            );

        MementoMori.FollowUpState = SoulReaver;
        SoulReaver.FollowUpState = WaningCrescent;
        WaningCrescent.FollowUpState = Trample;
        Trample.FollowUpState = MementoMori;

        return new MonsterMoveStateMachine([MementoMori, SoulReaver, WaningCrescent, Trample], MementoMori);
    }

    private async Task MementoMoriMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "Attack/Attack_Point", 0);
        await Cmd.CustomScaledWait(1.5f, 1.5f);
        NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Normal, 180f + MegaCrit.Sts2.Core.Random.Rng.Chaotic.NextFloat(-10f, 10f));
        await PowerCmd.Apply<DoomPower>(targets, MementoMoriDoomNum, Creature, null);
        await PowerCmd.Apply<VulnerablePower>(targets, 2, Creature, null);
        foreach (Creature target in targets)
        {
            if(target.Player != null && target.Player.Character is Character.Flagellant)
            {
                await PowerCmd.Apply<ComboPower>(target, 1, Creature, null);
                await PowerCmd.Apply<StressPower>(target, 1, Creature, null);
            }
        }
    }

    private async Task SoulReaverMove(IReadOnlyList<Creature> targets)
    {
        // 说话
        //TalkCmd.Play(L10NMonsterLookup("FLAGELLANT-DEATH.moves.SOUL_REAVER.banter"), Creature, VfxColor.Blue);
        await DamageCmd
            .Attack(SoulReaverDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack/Attack_C", 0f) //原版其实是Attack_B，为了视觉冲击力，更有力量感，调换了一下动画
            .AfterAttackerAnim(delegate
            {
                NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Normal, 180f + MegaCrit.Sts2.Core.Random.Rng.Chaotic.NextFloat(-10f, 10f));
                return Task.CompletedTask;
            })
            .WithWaitBeforeHit(1.2f, 1.2f)
            .WithHitFx("vfx/vfx_attack_slash") // 攻击特效
            .Execute(null);

        foreach (Creature target in targets)
        {
            if (target.Player != null && target.Player.Character is Character.Flagellant)
            {
                await PowerCmd.Apply<StressPower>(target, 2, Creature, null);
            }
        }
    }

    private async Task WaningCrescentMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd
            .Attack(WaningCrescentDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack/Attack_B", 0f) //原版其实是Attack_C
            .AfterAttackerAnim(delegate
            {
                NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Normal, 180f + MegaCrit.Sts2.Core.Random.Rng.Chaotic.NextFloat(-10f, 10f));
                return Task.CompletedTask;
            })
            .WithWaitBeforeHit(1.2f, 1.2f)
            .WithHitFx("vfx/vfx_attack_slash") // 攻击特效
            .Execute(null);
        await CreatureCmd.GainBlock(Creature, WaningCrescentDamage, ValueProp.Move, null);
    }

    private async Task TrampleMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd
            .Attack(TrampleDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack/Attack_Trample", 0f)
            .WithWaitBeforeHit(1.3f, 1.3f)
            .WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
            .Execute(null);
        foreach (Creature target in targets)
        {
            if (target.Player != null && target.Player.Character is Character.Flagellant)
            {
                if(target.GetPower<ComboPower>() is ComboPower comboP)
                {
                    NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Normal, 180f + MegaCrit.Sts2.Core.Random.Rng.Chaotic.NextFloat(-10f, 10f));
                    await PowerCmd.ModifyAmount(comboP, -1, target, null);
                    await PowerCmd.Apply<RingingPower>(targets, 1m, Creature, null);
                }
                await PowerCmd.Apply<StressPower>(target, 2, Creature, null);
            }
        }
        await PowerCmd.Apply<WeakPower>(targets, 2, Creature, null);
        await PowerCmd.Apply<StrengthPower>(Creature, TrampleStrength, Creature, null);
    }
}