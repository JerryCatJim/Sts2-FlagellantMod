using BaseLib.Config;

namespace Flagellant.Code.Config;

[ConfigHoverTipsByDefault]
internal class FlagellantConfig : SimpleModConfig
{
    // Should likely be at the top, as an easy and obvious opt-out
    public static bool ShouldUseRemadeCardImage { get; set; } = true;

    [ConfigSection("DD2Config")]
    public static bool ShouldPlayDeathDoorVfxIfDoomed { get; set; } = true;
    public static bool ShouldPlayDeathDoorVfxIfPoisoned { get; set; } = true;
    public static bool ShouldPlayDeathDoorVfxIfLowHealth { get; set; } = true;
    [ConfigSlider(0, 100, 1, Format = "{0:0} %")]
    public static int PlayerShowDeathDoorVfxHpPercent { get; set; } = 25;
    [ConfigSlider(0, 100, 1, Format = "{0:0} %")]
    public static int MonsterShowDeathDoorVfxHpPercent { get; set; } = 10;
    public static bool ShouldPlayerShowDeathDoorVfx { get; set; } = true;
    public static bool ShouldMonsterShowDeathDoorVfx { get; set; } = true;
    public static bool ShouldPlayerShowDeathBlowVfx { get; set; } = true;
    public static bool ShouldMonsterShowDeathBlowVfx { get; set; } = true;

    [ConfigSection("AnimSection")]
    public static bool ShouldPlayCardAnimAndSound { get; set; } = true;
    public static bool ShouldMuteSeparately { get; set; } = false;

    [ConfigSlider(-40, 20, 1, Format = "{0:0} dB")]
    public static int FlagellantAudioSoundVolume { get; set; } = 0;
    public static bool ShouldShowCardAnimInMultiplayerMode { get; set; } = true;
    public static bool ShouldUseDeathDoorIdle { get; set; } = true;

    [ConfigSection("DeathVfxAndBgm")]
    public static bool ShouldShowDeathEncounterVfx { get; set; } = true;
    public static bool ShouldPlayDeathEncounterBgm { get; set; } = true;
    [ConfigSection("DeathEncounter")]
    public static bool ShouldMultiplayerUseDefaultCondition { get; set; } = true;
    public static bool ShouldDeathOnlyHuntFlagellant { get; set; } = true;

    [ConfigSlider(0, 100, 1, Format = "{0:0}%")]
    public static int DeathEncounterChance { get; set; } = 6;

    [ConfigSlider(1, 99, 1)]
    public static int DeathAppearMaxTime { get; set; } = 1;
    public static bool ShouldEnhanceDeathAfterDefeat { get; set; } = true;
    public static bool PredictWhetherDeathWillAppear { get; set; } = true;
    public static bool ShouldDeathAppearInMonsterRoom { get; set; } = true;
    public static bool ShouldDeathAppearInEliteRoom { get; set; } = false;
    public static bool ShouldDeathAppearInBossRoom { get; set; } = false;
}