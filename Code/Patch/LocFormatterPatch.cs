using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SmartFormat.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFormat;
using Flagellant.Code.Formatters;

namespace Flagellant.Code.Patch;

[HarmonyPatch(typeof(LocManager), "LoadLocFormatters")]
public static class LocFormatterPatch
{
    public static void Postfix(LocManager __instance)
    {
        ListFormatter listFormatter = new ListFormatter();
        SmartFormatter TempSF = Traverse.Create(typeof(LocManager)).Field("_smartFormatter").GetValue<SmartFormatter>();
        TempSF.AddExtensions(listFormatter, new StressIconsFormatter());
        Smart.Default = TempSF;
    }
}