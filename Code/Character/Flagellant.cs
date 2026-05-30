using BaseLib.Abstracts;
using Flagellant.Code.Cards.Basic;
using Flagellant.Code.Cards.Common;
using Flagellant.Code.Cards.Uncommon;
using Flagellant.Code.Cards.Rare;
using Flagellant.Code.Extensions;
using Flagellant.Code.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Character;

public class Flagellant : PlaceholderCharacterModel
{
    public override float DeathAnimTime => 0f;

    public const string CharacterId = "Flagellant";
    public override string PlaceholderID => "silent";

    public static readonly Color Color = new Color("808080"); //gray
    // 角色名称颜色
    public override Color NameColor => Color;
    // 能量图标轮廓颜色
    //public override Color EnergyLabelOutlineColor => new Color("0000000D");
    // 地图绘制颜色
    public override Color MapDrawingColor => new Color("008000"); //green

    // 人物性别（男女中立）
    public override CharacterGender Gender => CharacterGender.Masculine;

    // 初始血量
    public override int StartingHp => 80;

    // 人物模型tscn路径。要自定义见下。
    public override string CustomVisualPath => "res://Flagellant/Scenes/combat_flagellant.tscn";
    // 卡牌拖尾场景。
    // public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    // 悬浮于继续游戏时的预览的人物头像路径。
    public override string CustomIconTexturePath => "flagellant_select.png".CharacterUiPath();
    // 左上角人物头像,注意要是tscn。
    public override string CustomIconPath => "res://Flagellant/Scenes/flagellant_icon.tscn";
    // 能量表盘tscn路径。要自定义见下。
    //public override string CustomEnergyCounterPath => "res://test/scenes/test_energy_counter.tscn";
    // 篝火休息场景。
    public override string CustomRestSiteAnimPath => "res://Flagellant/Scenes/campfire_flagellant.tscn";
    // 商店人物场景。
    public override string CustomMerchantAnimPath => "res://Flagellant/Scenes/shop_flagellant.tscn";
    // 多人模式-手指。
    // public override string CustomArmPointingTexturePath => null;
    // 多人模式剪刀石头布-石头。
    // public override string CustomArmRockTexturePath => null;
    // 多人模式剪刀石头布-布。
    // public override string CustomArmPaperTexturePath => null;
    // 多人模式剪刀石头布-剪刀。
    // public override string CustomArmScissorsTexturePath => null;

    // 人物选择背景。
    public override string CustomCharacterSelectBg => "res://Flagellant/Scenes/flagellant_bg.tscn";
    // 人物选择图标。
    public override string CustomCharacterSelectIconPath => "flagellant_select.png".CharacterUiPath();
    // 人物选择图标-锁定状态。
    public override string CustomCharacterSelectLockedIconPath => "flagellant_select_locked.png".CharacterUiPath();
    // 人物选择过渡动画。
    // public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    public override string CustomMapMarkerPath => "flagellant_map_maker.png".CharacterUiPath();
    // 攻击音效
    // public override string CustomAttackSfx => null;
    // 施法音效
    // public override string CustomCastSfx => null;
    // 死亡音效
    // public override string CustomDeathSfx => null;
    // 角色选择音效
    // public override string CharacterSelectSfx => null;
    // 过渡音效。这个不能删。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

	public override IEnumerable<CardModel> StartingDeck => [
		ModelDb.Card<FlagellantStrike>(),
        ModelDb.Card<FlagellantStrike>(),
        ModelDb.Card<FlagellantStrike>(),
        ModelDb.Card<FlagellantStrike>(),
        ModelDb.Card<FlagellantDefend>(),
        ModelDb.Card<FlagellantDefend>(),
        ModelDb.Card<FlagellantDefend>(),
        ModelDb.Card<FlagellantDefend>(),
        ModelDb.Card<Punish>(),
		ModelDb.Card<Fester>(),
    ];

	public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<PainBox>()];

	public override CardPoolModel CardPool => ModelDb.CardPool<FlagellantCardPool>();
	public override RelicPoolModel RelicPool => ModelDb.RelicPool<FlagellantRelicPool>();
	public override PotionPoolModel PotionPool => ModelDb.PotionPool<FlagellantPotionPool>();
}
