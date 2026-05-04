using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Flagellant.Code.Powers;

public sealed class ComboPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;
}