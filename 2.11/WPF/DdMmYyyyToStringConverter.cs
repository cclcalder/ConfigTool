namespace WPF
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class DdMmYyyyToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime) value).ToString("dd/MM/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", culture.DateTimeFormat, DateTimeStyles.None,
                                       out dateTime))
            {
                return dateTime;
            }
            return null;
        }

        #endregion
    }
}