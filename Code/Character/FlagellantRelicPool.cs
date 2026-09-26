using BaseLib.Abstracts;
using Godot;

namespace Flagellant.Code.Character;

public partial class FlagellantRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Flagellant.CharacterId;

    public override Color LabOutlineColor => Flagellant.Color;
}
