using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Flagellant.Code.Audio;

public class StressAudioManager
{
    private static AudioStreamPlayer? _stressAudioPlayer;
    public static AudioStreamPlayer StressAudioPlayer
    {
        get
        {
            // IsInstanceValid 能同时检测 null 和已被 QueueFree 的对象
            if (!GodotObject.IsInstanceValid(_stressAudioPlayer))
            {
                _stressAudioPlayer = new AudioStreamPlayer();
            }
            return _stressAudioPlayer;
        }
    }

    public static void PlayStressSfx(string AudioName, bool bIsTemp = false, bool bIsFullPathName = false, float VolumeDb = -10.0f) //声音素材音量太大了，减小一点
    {
        if (NonInteractiveMode.IsActive) return;

        AudioStream stream;
        string path = bIsFullPathName ? AudioName : GetStressSoundPath(AudioName);

        if (path == null || path == "") return;
        try
        {
            stream = PreloadManager.Cache.GetAsset<AudioStream>(path);
        }
        catch
        {
            Log.Info($"[StressAudioManager] Could not load audio: {path}");
            return;
        }

        //单例模式的audioPlayer，不要手动释放
        var audioPlayer = bIsTemp ? new AudioStreamPlayer() : StressAudioPlayer;

        audioPlayer.VolumeDb = 0.0f;
        audioPlayer.VolumeDb += VolumeDb;
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
    private static string GetStressSoundPath(string audioName)
    {
        switch (audioName)
        {
            case "GainStress":
                return "res://Flagellant/Sounds/Stress/sfx_battle_status_stressup.wav";
            case "LoseStress":
                return "res://Flagellant/Sounds/Stress/sfx_battle_status_stressdown.wav";
            default:
                return "";
        }
    }
}
