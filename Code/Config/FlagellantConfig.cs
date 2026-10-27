using BaseLib.Config;

namespace Flagellant.Code.Config;

[ConfigHoverTipsByDefault]
internal class FlagellantConfig : SimpleModConfig
{
    public enum SkinTypeEnum
    {
        Default,
        Exanimate,
        Maniac,
        Scourge,
        Infernal,
        Origin
    }
    // Should likely be at the top, as an easy and obvious opt-out
    public static bool ShouldUseRemadeCardImage { get; set; } = true;

    [ConfigDropdownOverrideLocalization("SKIN_TYPE")]
    public static SkinTypeEnum SkinType { get; set; } = SkinTypeEnum.Default;

    [ConfigSection("AnimSection")]
    public static bool ShouldPlayCardAnimAndSound { get; set; } = true;
    public static bool ShouldMuteSeparately { get; set; } = false;

    [ConfigSlider(-40, 20, 1, Format = "{0:0} dB")]
    public static int FlagellantAudioSoundVolume { get; set; } = 0;
    public static bool ShouldShowCardAnimInMultiplayerMode { get; set; } = true;
    public static bool ShouldUseDeathDoorIdle { get; set; } = true;
}