using BaseLib.Abstracts;
using BaseLib.Extensions;
using Flagellant.Code.Extensions;

namespace Flagellant.Code.Abstract;

public abstract class FlagellantPotionModel : CustomPotionModel
{
    public override string CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();

    public override string CustomPackedOutlinePath =>
        //$"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".PotionImagePath();
        CustomPackedImagePath;
}