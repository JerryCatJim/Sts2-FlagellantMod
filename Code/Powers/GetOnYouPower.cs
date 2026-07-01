using BaseLib.Abstracts;
using BaseLib.Extensions;
using Flagellant.Code.Cards.Multiplayer;
using Flagellant.Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public class GetOnYouPower : TemporaryStrengthPower, ICustomPower
{
    public virtual string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "default_power.png".PowerImagePath();
        }
    }
    public virtual string CustomPackedIconPath => CustomBigIconPath;
    public virtual string CustomBigBetaIconPath => CustomBigIconPath;

    public override AbstractModel OriginModel => ModelDb.Card<GetOnYou>();

    protected override bool IsPositive => true;
}