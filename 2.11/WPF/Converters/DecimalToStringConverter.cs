namespace WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class DecimalToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (decimal) value;
                var format = parameter != null ? parameter.ToString() : "0.0";
                return d.ToString(format, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal d;
            if (decimal.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture, out d))
            {
                return d;
            }
            else
            {
                return 0m;
            }
        }

        #endregion
    }

    public class BooleanNegationConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return !(bool) value;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }

        #endregion
    }
}