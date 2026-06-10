using Flagellant.Code.Config;
using Flagellant.Code.Monster;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using MegaCrit.Sts2.Core.Saves;

namespace Flagellant.Code.Audio;

internal static class AudioManager
{
    public static void PlayCombatSfx(string AudioName, bool bIsTemp = false, bool bIsFullPathName = false, float VolumeDb = -10.0f) //声音素材音量太大了，减小一点
    {
        if (NonInteractiveMode.IsActive) return;

        AudioStream stream;
        String path = bIsFullPathName ? AudioName : AudioCfg.GetFlagellantPath(AudioName);

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
        var audioPlayer = bIsTemp ? new AudioStreamPlayer() : CombatAudioPlayer.PlayerAudioPlayer;

        audioPlayer.VolumeDb = 0.0f;
        audioPlayer.VolumeDb += VolumeDb + FlagellantConfig.FlagellantAudioSoundVolume;
        audioPlayer.Stream = stream;
        audioPlayer.Bus = "SFX";
        if(bIsTemp)
        {
            audioPlayer.Finished += () => audioPlayer.QueueFreeSafely();
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
            audioPlayer.QueueFreeSafely();
        }
    }

    public static void PlayMonsterSfx(string AudioName, bool bIsTemp = false, bool bIsFullPathName = false, float VolumeDb = -4f)
    {
        AudioStream stream;
        String path = bIsFullPathName ? AudioName : AudioCfg.GetDeathPath(AudioName);

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
        var audioPlayer = bIsTemp ? new AudioStreamPlayer() : CombatAudioPlayer.MonsterAudioPlayer;

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
        String path = "res://Flagellant/Monster_Death/Bgm/DD2_TheMountain_BGM.mp3";
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
        var audioPlayer = CombatAudioPlayer.MonsterBgmPlayer;

        audioPlayer.VolumeDb = 0.0f;
        audioPlayer.VolumeDb += VolumeDb;
        audioPlayer.Stream = stream;
        audioPlayer.Bus = "SFX";

        CombatAudioPlayer.ModifiedMonsterBgmLinear = audioPlayer.VolumeLinear;

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
        var audioPlayer = CombatAudioPlayer.MonsterBgmPlayer;
        audioPlayer.Stop();
        audioPlayer.QueueFreeSafely();
        NAudioManager.Instance?.SetBgmVol(SaveManager.Instance.SettingsSave.VolumeBgm);
    }

    public static void SetMonsterBgmPlayerVolumeByPercent(float percent)
    {
        //SaveManager.Instance.SettingsSave.VolumeBgm在 [NBgmVolumeSlider] 类 的 "OnValueChanged"函数里接收的值已经/100.0了
        //VolumeLinear会把VolumeDb覆盖，所以要把修改过的Volume先记录再应用百分比
        CombatAudioPlayer.MonsterBgmPlayer.VolumeLinear = CombatAudioPlayer.ModifiedMonsterBgmLinear * Math.Clamp(percent, 0, 1);
    }
}

[HarmonyPatch(typeof(NBgmVolumeSlider), "OnValueChanged")]
public static class CreatureCmdHealPatch
{
    public static void Postfix(double value)
    {
        //从死神的战斗中退回到主界面会导致BGM音量为0，调一下滑动条就恢复正常了，没找到退回到主界面事件，懒得修了
        if(CombatManager.Instance.IsInProgress && DeathListenForRunStateSingleton.IsDeathExistingInCombat == true)
        {
            NAudioManager.Instance?.SetBgmVol(0);
            AudioManager.SetMonsterBgmPlayerVolumeByPercent((float)(value / 100.0));
        }
    }
}