using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using Flagellant.Code.Character;
using Flagellant.Code.Extensions;
using Flagellant.Code.ResoluteOrMeltdown.Vfx;

namespace Flagellant.Code.ResoluteOrMeltdown;

public enum ResoluteOrMeltdownType
{
    None,
    Resolute,
    Meltdown,
    Toxic //You can add more types.
}

public abstract class ResoluteOrMeltdownModel : AbstractModel
{
    private Player? _player;
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    public abstract ResoluteOrMeltdownType RMType {get;}

    public ResoluteOrMeltdownModel ToMutable(Player player)
    {
        var mutable = (ResoluteOrMeltdownModel)MutableClone();
        mutable._player = player;
        return mutable;
    }

    private LocString Title => new("powers", $"{MainFile.ModId.ToUpperInvariant()}-{Id.Entry}.title");
    private LocString Description => new("powers", $"{MainFile.ModId.ToUpperInvariant()}-{Id.Entry}.description");
    private string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    private Texture2D Icon => ResourceLoader.Load<Texture2D>(PackedIconPath);
    public HoverTip DumbHoverTip
    {
        get
        {
            var description = Description;
            AddDumbVariablesToDescription(description);
            return new HoverTip(Title, description.GetFormattedText(), Icon);
        }
    }



    private void AddDumbVariablesToDescription(LocString description)
    {
        description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
        var pool = IsMutable ? Owner.Character.CardPool : ModelDb.CardPool<FlagellantCardPool>();
        description.Add("energyPrefix", EnergyIconHelper.GetPrefix(pool));
    }

    protected virtual VfxConfig FlagellantVfxConfig => new (
        EnterSfxPath: GetEnterSfxPath(),
        ScreenShakeStrength: ShakeStrength.Strong
    );

    private VfxController? _vfx;

    //public IEnumerable<string> AssetPaths => VfxConfig.AssetPaths;

    public virtual async Task OnEnterResoluteOrMeltdown(PlayerChoiceContext ctx, Player owner, CardModel? source)
    {
        _vfx = new VfxController(FlagellantVfxConfig);
        await _vfx.OnEnter(owner.Creature);

    }

    public virtual async Task OnExitResoluteOrMeltdown(PlayerChoiceContext ctx, Player owner, CardModel? source)
    {
        if (_vfx != null)
            await _vfx.OnExit(owner.Creature);
        _vfx = null;
    }

    private String GetEnterSfxPath()
    {
        String Path = "";
        switch(RMType)
        {
            case ResoluteOrMeltdownType.Resolute:
                Path = "res://Flagellant/Sounds/Resolute/sfx_battle_status_resolute.wav";
                break;
            case ResoluteOrMeltdownType.Meltdown:
            case ResoluteOrMeltdownType.Toxic:
                Path = "res://Flagellant/Sounds/Meltdown/sfx_battle_status_meltdown.wav";
                break;
            default:
                break;
        }
        return Path;
    }
}