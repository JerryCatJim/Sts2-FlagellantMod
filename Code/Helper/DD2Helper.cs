using Flagellant.Code.Abstract;
using Flagellant.Code.Audio;
using Flagellant.Code.Monster;
using Flagellant.Code.Singleton;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Flagellant.Code.Helper;

public static class DD2Helper
{
    public static bool IsDD2Character(Creature? creature)
    {
        return creature != null && creature.Player != null && creature.Player.Character is IGetDD2CharacterType DD2Character &&
            !string.IsNullOrEmpty(DD2Character.TryGetCharacterType()) && DD2Character.TryGetCharacterType() != "DD2DefaultCharacter";
    }
    public static bool IsDD2Character(Player? player)
    {
        return player != null && player.Character is IGetDD2CharacterType DD2Character &&
            !string.IsNullOrEmpty(DD2Character.TryGetCharacterType()) && DD2Character.TryGetCharacterType() != "DD2DefaultCharacter";
    }
    public static bool IsFlagellant(Creature? creature)
    {
        return creature != null && creature.Player != null && creature.Player.Character is IGetDD2CharacterType DD2Character && DD2Character.TryGetCharacterType() == "Flagellant";
    }
    public static bool IsFlagellant(Player? player)
    {
        return player != null && player.Character is IGetDD2CharacterType DD2Character && DD2Character.TryGetCharacterType() == "Flagellant";
    }
    public static bool WillDieInDoom(Creature? creature, decimal hpDelta = 0m, decimal doomDelta = 0m)
    {
        decimal doomNum = (creature?.GetPower<DoomPower>()?.Amount ?? 0) - doomDelta;
        //delta均为0时获取的是当前的状态，减去delta获取的是上次状态
        return creature != null && doomNum > 0 && (doomNum >= creature.CurrentHp - hpDelta);
    }
    public static void ResetAllValues()
    {
        DD2CombatSingleton.ResetValue();
        DeathListenForRunStateSingleton.ResetValue();
        DD2AudioManager.StopDD2Bgm();
    }

    private static readonly Dictionary<Creature, bool> _isPlayingDeathVfx = new();
    public static void PlayDeathVfx(Creature creature, string DeathBlowName)
    {
        if (_isPlayingDeathVfx.TryGetValue(creature, out bool isPlaying) && isPlaying)
            return;

        PlayDeathSfx(creature, DeathBlowName);

        string NodeName = DeathBlowName;
        NCreature? nCreature = creature.GetCreatureNode();

        if (nCreature == null) return;

        Node2D vfxNode = PreloadManager.Cache.GetScene("res://Flagellant/Scenes/DD2Scenes/" + NodeName + ".tscn").Instantiate<Node2D>();
        if (vfxNode == null)
        {
            Log.Info("[DD2Helper PlayDeathVfx]: " + NodeName + ".tscn load failed.");
            return;
        }
        else
        {
            if (vfxNode.GetParent() == null && NGame.Instance != null)
            {
                NGame.Instance.RootSceneContainer.AddChild(vfxNode);
                vfxNode.GlobalPosition = nCreature.Visuals.GetNodeOrNull<Marker2D>("%CenterPos")?.GlobalPosition ??
                    nCreature.GlobalPosition;
                vfxNode.Scale = nCreature.Visuals.Scale;
            }
        }

        var AnimPlayer = vfxNode.GetNodeOrNull<AnimationPlayer>(NodeName + "AnimPlayer");
        if (AnimPlayer != null)
        {
            _isPlayingDeathVfx[creature] = true;
            AnimPlayer.AnimationFinished += (StringName state) => 
            {
                _isPlayingDeathVfx.Remove(creature);
                vfxNode.QueueFreeSafely();
            };
            AnimPlayer.Stop();
            AnimPlayer.Play("Show");
        }
        else
        {
            vfxNode.QueueFreeSafely();
        }
    }
    public static void PlayDeathSfx(Creature creature, string DeathBlowName)
    {
        if (DeathBlowName == "DeathBlow")
        {
            DD2AudioManager.PlayDD2Sfx(IsFlagellant(creature) ? "DeathBlowDoom" : "DeathBlow", false, false, -2);
        }
        else if (DeathBlowName == "DeathDoor")
        {
            DD2AudioManager.PlayDD2Sfx("DeathDoor", false, false, -4);
        }
    }
}
