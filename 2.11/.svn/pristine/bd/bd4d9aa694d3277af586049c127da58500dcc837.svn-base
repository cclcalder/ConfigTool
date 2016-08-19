using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Converters
{
    public class ColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var canView = (bool)parameter;
            return canView ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}