using BaseLib.Config;

namespace Flagellant.Code.Config;

[ConfigHoverTipsByDefault]
internal class FlagellantConfig : SimpleModConfig
{
    // Should likely be at the top, as an easy and obvious opt-out

    [ConfigSection("AnimSection")]
    public static bool ShouldPlayCardAnimAndSound { get; set; } = true;
    public static bool ShouldMuteSeparately { get; set; } = false;

    [ConfigSlider(-40, 20, 1, Format = "{0:0} dB")]
    public static int FlagellantAudioSoundVolume { get; set; } = 0;

    public static bool ShouldPlayCardAnimNetBroadcast { get; set; } = true;
}