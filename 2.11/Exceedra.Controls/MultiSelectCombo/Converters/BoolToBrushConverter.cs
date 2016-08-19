using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Exceedra.MultiSelectCombo.Converters
{
        public class BoolToBrushConverter : IValueConverter
        {
            public object Convert(object value, Type targetType,
                object parameter, CultureInfo culture)
            {
                if ((bool)value)
                    return Brushes.Black;

                return Brushes.Gray;
            }

            public object ConvertBack(object value, Type targetType,
                object parameter, CultureInfo culture)
            {
                return true;
            }
        }
    
}
