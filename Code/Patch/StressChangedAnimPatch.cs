using Flagellant.Audio;
using Flagellant.Code.Powers;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[HarmonyPatch(typeof(NCreature), "OnPowerIncreased")]
public static class StressIncreaseAnimPatch
{
    //需要把继承自PowerModel的Power里的AllowNegative改为true才能接收到amount小于0的情况，详见Creature.cs里的InvokePowerModified()
    public static bool Prefix(NCreature __instance, PowerModel power, int amount)
    {
        if (!CombatManager.Instance.IsInProgress) return false;
        if (amount == 0) return true;

        if (power is StressPower)// || power is ResoluteOrMeltdownPowerModel)
        {
            if (power is StressPower)
            {
                var visual = __instance.GetNodeOrNull<Node2D>("TestFlagellant");
                if (visual != null)
                {
                    String NodeName = amount > 0 ? "StressUp" : "StressDown";
                    Node2D StressNode = visual.GetNodeOrNull<Node2D>(NodeName);
                    if (StressNode == null)
                    {
                        Log.Info("[StressIncreasePatch] " + NodeName +" In flagellant.tscn is not found.");
                        return false;
                    }

                    var AnimPlayer = StressNode.GetNodeOrNull<AnimationPlayer>(NodeName + "AnimPlayer");
                    if (AnimPlayer != null)
                    {
                        AnimPlayer.Stop();
                        AnimPlayer.Play("Show");
                    }
                    var StressValueText = StressNode.GetNodeOrNull<Label>(NodeName + "Text");
                    if (StressValueText != null)
                    {
                        StressValueText.SetText(Math.Abs(amount).ToString());
                    }
                    if (amount > 0)
                    {
                        AudioManager.PlayCombatSfx("res://Flagellant/Sounds/Stress/sfx_battle_status_stressup.wav", true, true, -4);
                    }
                    else
                    {
                        AudioManager.PlayCombatSfx("res://Flagellant/Sounds/Stress/sfx_battle_status_stressdown.wav", true, true, -6);
                    }
                }
            }
            else if (power is ResoluteOrMeltdownPowerModel)
            {

            }
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(NCreature), "OnPowerRemoved")]
public static class StressRemovedAnimPatch
{
    public static bool Prefix(NCreature __instance, PowerModel power)
    {
        if (power is StressPower)// || power is ResoluteOrMeltdownPowerModel)
        {
            return false;
        }
        return true;
    }
}