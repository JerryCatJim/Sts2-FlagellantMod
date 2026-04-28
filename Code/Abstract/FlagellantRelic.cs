using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using Flagellant.Code.Character;
using Flagellant.Code.Extensions;

namespace Flagellant.Code.Abstract;

[Pool(typeof(FlagellantRelicPool))]

public abstract class FlagellantRelic : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "default_relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            /*var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "default_relic_outline.png".RelicImagePath();*/
            return PackedIconPath;
        }
    }

    protected override string BigIconPath
    {
        get
        {
            /*var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "manbo_toy.png".BigRelicImagePath();*/
            return PackedIconPath;
        }
    }
}