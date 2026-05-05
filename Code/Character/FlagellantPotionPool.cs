using BaseLib.Abstracts;
using Godot;

namespace Flagellant.Code.Character;

public class FlagellantPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => Flagellant.CharacterId;
    public override Color LabOutlineColor => Flagellant.Color;
}