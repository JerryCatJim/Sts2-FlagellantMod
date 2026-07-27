using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Flagellant.Code.Extensions;

namespace Flagellant.Code.Abstract;

public abstract class FlagellantPowerModel : CustomPowerModel
{
    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
			return ResourceLoader.Exists(path) ? path : "default_power.png".PowerImagePath();
        }
    }

    public override string CustomPackedIconPath
	{
		get
		{
            /*var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
			return ResourceLoader.Exists(path) ? path : "default_power.png".PowerImagePath();*/
            return CustomBigIconPath;
		}
	}
	
    protected decimal GetHealingPercentHp(decimal overridePercent)
    {
        decimal Percent = Math.Clamp(overridePercent, 0, 100);
        decimal Healing = Math.Round(base.Owner.MaxHp * Percent / 100m, MidpointRounding.AwayFromZero);
        if (Percent > 0 && Healing < 1m)
        {
            return 1m;
        }
        return Healing;
    }
}
