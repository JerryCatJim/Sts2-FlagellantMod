using Flagellant.Code.Monster;
using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace Flagellant.Code.Patches;

[HarmonyPatch(typeof(RunManager), "OnEnded")]
public static class OnGameEndedPatch
{
    public static void Postfix()
    {
        DeathListenForRunStateSingleton.ResetValue();
        MonsterAudioManager.StopMonsterBgm();
    }
}