using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Saves;

namespace Flagellant.Code.Monster;

public static class MonsterAudioManager
{
    private static AudioStreamPlayer? _monsterAudioPlayer;
    public static AudioStreamPlayer MonsterAudioPlayer
    {
        get
        {
            // IsInstanceValid 能同时检测 null 和已被 QueueFree 的对象
            if (!GodotObject.IsInstanceValid(_monsterAudioPlayer))
            {
                _monsterAudioPlayer = new AudioStreamPlayer();
            }
            return _monsterAudioPlayer;
        }
    }
    private static AudioStreamPlayer? _monsterBgmPlayer;
    public static AudioStreamPlayer MonsterBgmPlayer
    {
        get
        {
            // IsInstanceValid 能同时检测 null 和已被 QueueFree 的对象
            if (!GodotObject.IsInstanceValid(_monsterBgmPlayer))
            {
                _monsterBgmPlayer = new AudioStreamPlayer();
            }
            return _monsterBgmPlayer;
        }
    }
    public static float ModifiedMonsterBgmLinear { get; set; } = 0.0f;

    public static void PlayMonsterSfx(string AudioName, bool bIsTemp = false, bool bIsFullPathName = false, float VolumeDb = -4f)
    {
        AudioStream stream;
        string path = bIsFullPathName ? AudioName : MonsterAudioCfg.GetDeathPath(AudioName);

        if (path == null || path == "") return;
        try
        {
            stream = PreloadManager.Cache.GetAsset<AudioStream>(path);
        }
        catch
        {
            Log.Info($"[MonsterAudioManager] Could not load audio: {path}");
            return;
        }

        //单例模式的audioPlayer，不要手动释放
        var audioPlayer = bIsTemp ? new AudioStreamPlayer() : MonsterAudioPlayer;

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
    public static void PlayMonsterBgm(float VolumeDb = 0f)
    {
        AudioStream stream;
        string path = "res://Flagellant/Monster_Death/Bgm/DD2_TheMountain_BGM.mp3";
        try
        {
            stream = PreloadManager.Cache.GetAsset<AudioStream>(path);
        }
        catch
        {
            GD.PrintErr($"[MonsterAudioManager] Could not load audio: {path}");
            return;
        }

        //单例模式的audioPlayer，不要手动释放
        var audioPlayer = MonsterBgmPlayer;

        audioPlayer.VolumeDb = 0.0f;
        audioPlayer.VolumeDb += VolumeDb;
        audioPlayer.Stream = stream;
        audioPlayer.Bus = "SFX";

        ModifiedMonsterBgmLinear = audioPlayer.VolumeLinear;

        SetMonsterBgmPlayerVolumeByPercent(SaveManager.Instance.SettingsSave.VolumeBgm);

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null)
        {
            if (audioPlayer.GetParent() == null)
            {
                combatRoom.AddChild(audioPlayer);
                audioPlayer.Play();
                NAudioManager.Instance?.SetBgmVol(0);
            }
        }
    }
    public static void StopMonsterBgm()
    {
        var audioPlayer = MonsterBgmPlayer;
        audioPlayer.Stop();
        audioPlayer.QueueFreeSafely();
        NAudioManager.Instance?.SetBgmVol(SaveManager.Instance.SettingsSave.VolumeBgm);
    }

    public static void SetMonsterBgmPlayerVolumeByPercent(float percent)
    {
        //SaveManager.Instance.SettingsSave.VolumeBgm在 [NBgmVolumeSlider] 类 的 "OnValueChanged"函数里接收的值已经/100.0了
        //VolumeLinear会把VolumeDb覆盖，所以要把修改过的Volume先记录再应用百分比
        MonsterBgmPlayer.VolumeLinear = ModifiedMonsterBgmLinear * Math.Clamp(percent, 0, 1);
    }
}