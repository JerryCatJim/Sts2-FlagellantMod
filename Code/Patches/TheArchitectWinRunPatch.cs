using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Code.Patches;

/*[HarmonyPatch(typeof(TheArchitect), "WinRun")]
public static class TheArchitectWinRunPatch
{
    //遇到建筑师时人物会强制TriggerAnim("Attack")，其中必定会走到SfxCmd.Play(character.AttackSfx)
    //如果人物没有AttackSfx会导致播放出错而WinRun()中的await AnimPlayerAttackIfNecessary(Dialogue.EndAttackers)无限等待？
    //(看别人说的，没了解Godot的AudioPlayer播放找不到的音频会如何，反正把这块跳过了应该不会卡死)
    //如果人物继承了PlaceholderCharacterModel则无需担心
    public static bool Prefix(TheArchitect __instance)
    {
        if (LocalContext.IsMe(__instance.Owner))
        {
            if (__instance.Owner?.RunState.Players.Count > 1)
            {
                NCombatRoom.Instance?.SetWaitingForOtherPlayersOverlayVisible(visible: true);
            }
            RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
            return false;
        }
        return true;
    }
}*/
