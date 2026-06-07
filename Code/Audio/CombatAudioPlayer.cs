using BaseLib.Abstracts;
using Godot;

namespace Flagellant.Audio
{
    public class CombatAudioPlayer() : CustomSingletonModel(HookType.Run)
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
    }
}
