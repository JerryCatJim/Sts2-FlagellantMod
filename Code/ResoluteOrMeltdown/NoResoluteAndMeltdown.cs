using Flagellant.Code.ResoluteOrMeltdown.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Flagellant.Code.ResoluteOrMeltdown;

public class NoResoluteAndMeltdown : ResoluteOrMeltdownModel
{
    public override bool ShouldReceiveCombatHooks => false;
    public override ResoluteOrMeltdownType RMType => ResoluteOrMeltdownType.None;
    protected override VfxConfig RMVfxConfig => new(
        EnterSfxPath: "",
        ScreenShakeStrength: ShakeStrength.None
    );
}