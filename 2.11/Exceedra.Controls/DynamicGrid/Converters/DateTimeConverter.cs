using System;
using System.Globalization;
using System.Windows.Data;

namespace Exceedra.DynamicGrid.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        // Converting the date from the apps culture to the db format
        // When the string is being parsed from the Value property to a datetime picker
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        // Converting the date from the apps culture to the db format
        // When the string is parsed from a datetime picker to the database
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        private string Convert(object date)
        {
            if (date == null || string.IsNullOrEmpty(date.ToString()))
                return string.Empty;

            DateTime dateInAppsCulture;
            bool isDateParseable = DateTime.TryParse(date.ToString(), out dateInAppsCulture);

            if (isDateParseable)
            {
                string dateInDbFormat = dateInAppsCulture.ToString("yyyy-MM-dd");
                return dateInDbFormat; // This is the value the datepicker can read properly.
            }

            return "Select Date";
        }
    }
}
