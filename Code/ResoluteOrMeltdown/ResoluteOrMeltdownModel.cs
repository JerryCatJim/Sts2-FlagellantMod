using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Flagellant.Code.Character;
using Flagellant.Code.Extensions;
using Flagellant.Code;

namespace Flagellant.Code.ResoluteOrMeltdown;

public abstract class ResoluteOrMeltdownModel : AbstractModel
{
    //public override PowerType Type => PowerType.Buff;
    //public override PowerStackType StackType => PowerStackType.None;
    //protected override bool IsVisibleInternal => false;

    //public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    //public override string CustomBigIconPath => CustomPackedIconPath;

    private Player? _player;
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

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


    /*protected abstract StanceVfxConfig VfxConfig { get; }

    private StanceVfxController? _vfx;

    public IEnumerable<string> AssetPaths => VfxConfig.AssetPaths;*/

    public virtual async Task OnEnterResoluteOrMeltdown(PlayerChoiceContext ctx, Player owner, CardModel? source)
    {
        //_vfx = new StanceVfxController(VfxConfig);
        //await _vfx.OnEnter(owner.Creature);
    }

    public virtual async Task OnExitResoluteOrMeltdown(PlayerChoiceContext ctx, Player owner, CardModel? source)
    {
        /*if (_vfx != null)
            await _vfx.OnExit(owner.Creature);
        _vfx = null;*/
    }
}