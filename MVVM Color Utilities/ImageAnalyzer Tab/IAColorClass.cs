using MVVM_Color_Utilities.Helpers;
using System.Drawing;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    internal class IAColorClass
    {
        public IAColorClass(Color color)
        {
            Color = color;
            ColorHex = ColorUtils.ColorToHex(color);
        }

        public Color Color { get; set; }
        public string ColorHex { get; set; }
    }
}