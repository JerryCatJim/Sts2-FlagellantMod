using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.HoverTips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Code.Core;

public class FlagellantHoverTipFactory
{
    public static IHoverTip FromResoluteOrMeltdown<T>() where T : ResoluteOrMeltdownModel
    {
        return FromResoluteOrMeltdown(FlagellantModelDb.ResoluteOrMeltdown<T>());
    }

    public static IHoverTip FromResoluteOrMeltdown(ResoluteOrMeltdownModel model) => model.DumbHoverTip;

}
