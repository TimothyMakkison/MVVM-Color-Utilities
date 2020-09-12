using MVVM_Color_Utilities.Helpers.Extensions;
using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVVM_Color_Utilities.Converters
{
    [ValueConversion(typeof(Color?), typeof(string))]
    public class ColorToHexConverter : IValueConverter
    {
        private readonly Regex hexColorReg = new Regex("^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color? ?? Color.White;
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || !(value is string))
                return null;

            var s = value as string;
            if (!hexColorReg.IsMatch(s))
                return null;

            var c = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(s);
            return c.ToDrawingColor();
        }
    }
}