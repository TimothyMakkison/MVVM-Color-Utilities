using MVVM_Color_Utilities.Helpers.Extensions;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_Color_Utilities.Converters
{
    [ValueConversion(typeof(Color), typeof(System.Windows.Media.SolidColorBrush))]
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color))
                return new System.Windows.Media.SolidColorBrush();
            var color = value as Color? ?? Color.Transparent;
            return color.ToBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}