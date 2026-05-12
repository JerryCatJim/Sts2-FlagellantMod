using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Flagellant.Code.Powers;

public class StableReconstructionPower : FlagellantPowerModel
{
    //Just for displaying......
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
