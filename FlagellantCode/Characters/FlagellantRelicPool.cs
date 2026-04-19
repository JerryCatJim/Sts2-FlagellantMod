using BaseLib.Abstracts;
using Godot;
using Flagellant.Character;
using System;

namespace Flagellant.Character;

public partial class FlagellantRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Flagellant.CharacterId;

    public override Color LabOutlineColor => Flagellant.Color;
}
