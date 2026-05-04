using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Flagellant.Code.Config;

namespace Flagellant.Audio;

internal static class AudioManager
{
    public static void PlayCombatSfx(string AudioName, bool bIsTemp = false, bool bIsFullPathName = false, float VolumeDb = -10.0f) //声音素材音量太大了，减小一点
    {
        if (NonInteractiveMode.IsActive) return;

        AudioStream stream;
        String path = bIsFullPathName ? AudioName : AudioCfg.GetPath(AudioName);

        if (path == null || path == "") return;
        try
        {
            stream = PreloadManager.Cache.GetAsset<AudioStream>(path);
        }
        catch
        {
            GD.PrintErr($"[AudioManager] Could not load audio: {path}");
            return;
        }

        //单例模式的audioPlayer，不要手动释放
        var audioPlayer = bIsTemp ? new AudioStreamPlayer() : CombatCardAudioPlayer.audioPlayer;

        audioPlayer.VolumeDb = 0.0f;
        audioPlayer.VolumeDb += VolumeDb + FlagellantConfig.FlagellantAudioSoundVolume;
        audioPlayer.Stream = stream;
        audioPlayer.Bus = "SFX";
        if(bIsTemp)
        {
            audioPlayer.Finished += () => audioPlayer.QueueFree();
        }

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null)
        {
            if(audioPlayer.GetParent() == null)
            {
                combatRoom.AddChild(audioPlayer);
            }
            audioPlayer.Play();
        }
        else if(bIsTemp)
        {
            audioPlayer.QueueFree();
        }
    }
}

