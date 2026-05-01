using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Flagellant.Code.Character;
using Flagellant.Code.Core;
using Flagellant.Code.Extensions;
using Flagellant.Code.ResoluteOrMeltdown;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Logging;

namespace Flagellant.Code.Abstract;

[Pool(typeof(FlagellantCardPool))]
public abstract class FlagellantCardModel(
    int canonicalEnergyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true) :
    MyConstructedCardModel(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary)
{
    public String _cardSelectAnimName = "DoNothing";
    public String _cardPlayAnimName = "DoNothing";
    public String CardSelectAnimName => _cardSelectAnimName;
    public String CardPlayAnimName => _cardPlayAnimName;

    protected FlagellantCardModel WithRMTip<T>() where T : ResoluteOrMeltdownModel
    {
        WithTip(new TooltipSource(_ => FlagellantHoverTipFactory.FromResoluteOrMeltdown<T>()));
        return this;
    }
    protected FlagellantCardModel WithAnimName(String AnimName)
    {
        _cardSelectAnimName = AnimName;
		_cardPlayAnimName = AnimName;
        return this;
    }
    protected async Task PlaySkillAnim()
    {
        if(CardPlayAnimName == null || CardPlayAnimName == "" || CardPlayAnimName == "DoNothing")
		{
			return;
        }
        await CreatureCmd.TriggerAnim(Owner.Creature, CardPlayAnimName, 0.0f);
    }
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath
	{
		get
		{
			var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
			Log.Info(">>>[FlagellantMod]CardPath=" + path, 2);
			return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
		}
	}

	//Smaller variants of card images for efficiency:
	//Smaller variant of fullart: 250x350
	//Smaller variant of normalart: 250x190

	//Uses card_portraits/card_name.png as image path. These should be smaller images.
	public override string PortraitPath
	{
		get
		{
			var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
			Log.Info(">>>[FlagellantMod]CardPath=" + path, 2);
			return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
		}
	}

	//Optional and I'm not sure it's functional yet.
	public override string BetaPortraitPath
	{
		get
		{
			var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
			Log.Info(">>>[FlagellantMod]CardPath=" + path, 2);
			return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
		}
	}
}
