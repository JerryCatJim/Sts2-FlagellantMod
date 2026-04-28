using Flagellant.Code.Powers;
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
        if (!CombatManager.Instance.IsInProgress)
        {
            return false;
        }
        if (power is StressPower || power is ResoluteOrMeltdownPowerModel)
        {
            Log.Info("OnPowerIncreased Amount:" + amount);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(NCreature), "OnPowerRemoved")]
public static class StressDecreaseAnimPatch
{
    public static bool Prefix(NCreature __instance, PowerModel power)
    {
        if (power is StressPower || power is ResoluteOrMeltdownPowerModel)
        {
            return false;
        }
        return true;
    }
}