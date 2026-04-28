using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
