using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace Flagellant.Code.Formatters;

public class ComboIconsFormatter : IFormatter
{
    public string Name
    {
        get
        {
            return "comboIcons";
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        int result = 1;
        string text3 = "[img=28x28 center]res://Flagellant/Images/FormatterIcons/combo_small.png[/img]";
        string text4 = ((result > 0 && result < 4) ? string.Concat(Enumerable.Repeat(text3, result)) : ((!(formattingInfo.CurrentValue is DynamicVar dynamicVar)) ? $"{result}{text3}" : (dynamicVar.ToHighlightedString(inverse: false) + text3)));
        formattingInfo.Write(text4);
        return true;
    }
}
