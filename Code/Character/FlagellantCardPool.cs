using BaseLib.Abstracts;
using Godot;
using Flagellant.Code.Extensions;

namespace Flagellant.Code.Character;

public class FlagellantCardPool : CustomCardPoolModel
{
    public override string Title => Flagellant.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "Charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "Charui/text_energy.png".ImagePath();

    /* These HSV values will determine the color of your card back.
	They are applied as a shader onto an already colored image,
	so it may take some experimentation to find a color you like.
	Generally they should be values between 0 and 1. */
    /*public override float H => 0.95f;
    public override float S => 0.98f;
    public override float V => 0.7f;*/

    public override float H => 0.33f; //Hue; changes the color. //green
    public override float S => 0.7f; //Saturation
    public override float V => 0.7f; //Brightness

    //Alternatively, leave these values at 1 and provide a custom frame image.
    /*public override Texture2D CustomFrame(CustomCardModel card)
	{
		//This will attempt to load Oddmelt/images/cards/frame.png
		return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
	}*/

    //Color of small card icons
    public override Color DeckEntryCardColor => new("008000"); //green
    public override Color EnergyOutlineColor => new("454545"); //dark_gray

    public override bool IsColorless => false;
}
