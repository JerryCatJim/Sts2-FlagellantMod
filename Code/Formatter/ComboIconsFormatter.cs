using System;
using System.Linq;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Runs;
using SmartFormat.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using Flagellant.Code.Powers;

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
        /*object currentValue = formattingInfo.CurrentValue;
        int result = 0;
        if (!(currentValue is PowerVar<ComboPower> comboVar))
        {
            if (!(currentValue is CalculatedVar calculatedVar))
            {
                if (!(currentValue is decimal num))
                {
                    if (!(currentValue is int num2))
                    {
                        if (!(currentValue is string text2))
                        {
                            throw new LocException($"Unknown value='{formattingInfo.CurrentValue}' type={formattingInfo.CurrentValue?.GetType()}");
                        }
                        if (!int.TryParse(formattingInfo.FormatterOptions, out result))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        result = num2;
                    }
                }
                else
                {
                    result = (int)num;
                }
            }
            else
            {
                result = Convert.ToInt32(calculatedVar.Calculate(null));
            }
        }
        else
        {
            result = Convert.ToInt32(comboVar.PreviewValue);
        }*/
        int result = 1;
        string text3 = "[img=28x28 center]res://Flagellant/Images/FormatterIcons/combo_small.png[/img]";
        string text4 = ((result > 0 && result < 4) ? string.Concat(Enumerable.Repeat(text3, result)) : ((!(formattingInfo.CurrentValue is DynamicVar dynamicVar)) ? $"{result}{text3}" : (dynamicVar.ToHighlightedString(inverse: false) + text3)));
        formattingInfo.Write(text4);
        return true;
    }
}
