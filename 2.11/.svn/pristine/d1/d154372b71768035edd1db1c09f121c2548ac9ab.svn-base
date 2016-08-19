using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WPF.Converters
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateConverter : IValueConverter
    {
        private const string _format = "dd/MM/yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;

            return date.ToString(_format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.ParseExact((string)value, _format, culture);
        }

    }

    [ValueConversion(typeof(DateTime), typeof(String))]
    public class CurrentCultureDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;

            return date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.ParseExact((string)value, CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, culture);
        }

    }
}
