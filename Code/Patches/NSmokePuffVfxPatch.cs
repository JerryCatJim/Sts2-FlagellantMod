using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using static MegaCrit.Sts2.Core.Nodes.Vfx.NSmokePuffVfx;

namespace Flagellant.Code.Patches;

[HarmonyPatch(typeof(NSmokePuffVfx), "_Ready")]
public static class NSmokePuffVfxPatch
{
    private static SmokePuffColor lastColor = SmokePuffColor.Green;
    public static void Postfix(NSmokePuffVfx __instance)
    {
        //官方材质默认为绿色，检测到紫色时，把材质改为了紫色，但如果应用过紫色后又应用绿色，会导致材质被改为紫色而仍为紫色，需加一条将材质改为绿色的逻辑
        SmokePuffColor spColor = Traverse.Create(__instance).Field("_color").GetValue<SmokePuffColor>();
        GpuParticles2D spClouds = Traverse.Create(__instance).Field("_clouds").GetValue<GpuParticles2D>();
        if (lastColor != SmokePuffColor.Green && spColor == SmokePuffColor.Green)
        {
            ParticleProcessMaterial particleProcessMaterial = (ParticleProcessMaterial)spClouds.ProcessMaterial;
            particleProcessMaterial.HueVariationMin = -0.1f;
            particleProcessMaterial.HueVariationMax = 0.1f;
            particleProcessMaterial.Color = new Color(0.78f, 1.0f, 0.61f);
        }
        lastColor = spColor;
    }
}
