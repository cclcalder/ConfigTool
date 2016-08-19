using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Exceedra.Converters
{
    public class GridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
                return new GridLength(1, GridUnitType.Star);

            if (value is GridLength)
                return value;

            int val = (int)value;
            return new GridLength(val); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength val = (GridLength)value;
            
            return val.Value;
        }
    }
}
