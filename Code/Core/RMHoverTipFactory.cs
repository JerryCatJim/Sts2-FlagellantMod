using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.HoverTips;

namespace Flagellant.Code.Core;

public class RMHoverTipFactory
{
    public static IHoverTip FromResoluteOrMeltdown<T>() where T : ResoluteOrMeltdownModel
    {
        return FromResoluteOrMeltdown(RMModelDb.ResoluteOrMeltdown<T>());
    }

    public static IHoverTip FromResoluteOrMeltdown(ResoluteOrMeltdownModel model) => model.DumbHoverTip;

}
