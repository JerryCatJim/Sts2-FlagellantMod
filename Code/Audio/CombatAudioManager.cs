using Flagellant.Code.Config;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Flagellant.Code.Audio;

public static class CombatAudioManager
{
    private static AudioStreamPlayer? _playerAudioPlayer;
    public static AudioStreamPlayer PlayerAudioPlayer
    {
        get
        {
            // IsInstanceValid 能同时检测 null 和已被 QueueFree 的对象
            if (!GodotObject.IsInstanceValid(_playerAudioPlayer))
            {
                _playerAudioPlayer = new AudioStreamPlayer();
            }
            return _playerAudioPlayer;
        }
    }

    private static AudioStreamPlayer? _secondPlayerAudioPlayer;
    public static AudioStreamPlayer SecondPlayerAudioPlayer
    {
        get
        {
            // IsInstanceValid 能同时检测 null 和已被 QueueFree 的对象
            if (!GodotObject.IsInstanceValid(_secondPlayerAudioPlayer))
            {
                _secondPlayerAudioPlayer = new AudioStreamPlayer();
            }
            return _secondPlayerAudioPlayer;
        }
    }

    public static void PlayCombatSfx(string AudioName, bool bIsTemp = false, bool bIsFullPathName = false, float VolumeDb = -10.0f, int audioPlayerIndex = 0) //声音素材音量太大了，减小一点
    {
        if (NonInteractiveMode.IsActive) return;

        AudioStream stream;
        string path = bIsFullPathName ? AudioName : CombatAudioCfg.GetFlagellantPath(AudioName);

        if (path == null || path == "") return;
        try
        {
            stream = PreloadManager.Cache.GetAsset<AudioStream>(path);
        }
        catch
        {
            Log.Info($"[FlagellantCombatAudioManager] Could not load audio: {path}");
            return;
        }

        //单例模式的audioPlayer，不要手动释放
        var audioPlayer = bIsTemp ? new AudioStreamPlayer() : GetPlayerAudioPlayer(audioPlayerIndex);

        audioPlayer.VolumeDb = 0.0f;
        audioPlayer.VolumeDb += VolumeDb + FlagellantConfig.FlagellantAudioSoundVolume;
        audioPlayer.Stream = stream;
        audioPlayer.Bus = "SFX";
        if (bIsTemp)
        {
            audioPlayer.Finished += () => audioPlayer.QueueFreeSafely();
        }

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null)
        {
            if (audioPlayer.GetParent() == null)
            {
                combatRoom.AddChild(audioPlayer);
            }
            audioPlayer.Play();
        }
        else if (bIsTemp)
        {
            audioPlayer.QueueFreeSafely();
        }
    }

    private static AudioStreamPlayer GetPlayerAudioPlayer(int index = 0)
    {
        switch (index)
        {
            case 0:
            default:
                return PlayerAudioPlayer;
            case 1:
                return SecondPlayerAudioPlayer;
        }
    }
}