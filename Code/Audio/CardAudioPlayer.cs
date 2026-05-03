using BaseLib.Abstracts;
using Godot;

namespace Flagellant.Audio
{
    public class CombatCardAudioPlayer() : CustomSingletonModel(true, false)
    {
        private static AudioStreamPlayer? _audioPlayer;
        public static AudioStreamPlayer audioPlayer
        {
            get
            {
                // IsInstanceValid 能同时检测 null 和已被 QueueFree 的对象
                if (!GodotObject.IsInstanceValid(_audioPlayer))
                {
                    _audioPlayer = new AudioStreamPlayer();
                }
                return _audioPlayer;
            }
        }
    }
}
