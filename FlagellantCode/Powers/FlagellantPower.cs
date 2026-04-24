using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Flagellant.Extensions;

namespace Flagellant.Powers;

public abstract class FlagellantPower : CustomPowerModel
{
	public override string CustomPackedIconPath
	{
		get
		{
			var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
			return ResourceLoader.Exists(path) ? path : "relly_power.png".PowerImagePath();
		}
	}

	public override string CustomBigIconPath
	{
		get
		{
			var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
			return ResourceLoader.Exists(path) ? path : "relly_power.png".BigPowerImagePath();
		}
	}
}
