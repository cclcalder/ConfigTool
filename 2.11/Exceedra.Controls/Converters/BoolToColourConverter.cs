using System;
using System.Globalization;
using System.Windows.Data;

namespace Exceedra.Converters
{
    public class BoolToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSelected = (bool)value;
            if (isSelected) return "#FFACACAC";
            return "#FFFFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
