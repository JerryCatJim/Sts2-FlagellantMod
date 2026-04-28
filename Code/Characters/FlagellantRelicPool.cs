using BaseLib.Abstracts;
using Godot;
using Flagellant.Code.Character;
using System;

namespace Flagellant.Code.Character;

public partial class FlagellantRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Flagellant.CharacterId;

    public override Color LabOutlineColor => Flagellant.Color;
}
