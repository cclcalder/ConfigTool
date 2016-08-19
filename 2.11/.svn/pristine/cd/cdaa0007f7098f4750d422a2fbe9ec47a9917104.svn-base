using System;
using System.Globalization;
using System.Windows.Data;

namespace Exceedra.DynamicGrid.Converters
{
    public class ToPercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            //[1] contains the ItemsControl.ActualWidth we binded to, [0] the percentage
            //In this case, I assume the percentage is a double between 0 and 1
            return System.Convert.ToDouble(value[1]) * (double)value[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class To95PercentSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * (double)0.95;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}