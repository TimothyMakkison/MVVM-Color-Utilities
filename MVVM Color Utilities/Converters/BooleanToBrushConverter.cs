using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MVVM_Color_Utilities.Converters
{
    // Source: https://stackoverflow.com/a/42007113/11522147

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BooleanToBrushConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; }

        public Brush FalseBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool? ?? false;
            // Inverts color
            if (parameter != null && parameter.ToString() == "!")
            {
                boolValue = !boolValue;
            }

            return boolValue ? TrueBrush : FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}