using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Flagellant.Extensions;
using Flagellant.Cards;
using System;
using Flagellant.Relics;

namespace Flagellant.Character;

public class Flagellant : PlaceholderCharacterModel
{
	public const string CharacterId = "Flagellant";

	public override string PlaceholderID => "necrobinder";

	public static readonly Color Color = new Color("c4278a");

	public override Color NameColor => Color;
	public override CharacterGender Gender => CharacterGender.Feminine;
	public override int StartingHp => 70;

	public override IEnumerable<CardModel> StartingDeck => [
		ModelDb.Card<FlagellantAttack>(),
		ModelDb.Card<FlagellantAttack>(),
		ModelDb.Card<FlagellantBlock>(),
		ModelDb.Card<FlagellantRelly>(),
		ModelDb.Card<FlagellantPowerUp>()
	];

	public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<FlagellantToy>()];

	public override CardPoolModel CardPool => ModelDb.CardPool<FlagellantCardPool>();
	public override RelicPoolModel RelicPool => ModelDb.RelicPool<FlagellantRelicPool>();
	public override PotionPoolModel PotionPool => ModelDb.PotionPool<SharedPotionPool>();

	/*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
		override all the other methods that define those assets.
		These are just some of the simplest assets, given some placeholders to differentiate your character with.
		You don't have to, but you're suggested to rename these images. */
	public override string CustomVisualPath => "res://Flagellant/Scenes/flagellant.tscn";
	public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
	public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
	public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
	public override string CustomCharacterSelectBg => "res://Flagellant/Scenes/flagellant_bg.tscn";
	public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
