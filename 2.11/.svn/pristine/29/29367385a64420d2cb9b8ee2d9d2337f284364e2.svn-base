using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Exceedra.Common.Utilities
{
    public class DateTimeHelper
    {
        public static string ConvertDateTimeToDate(string dateTimeString)
        {

            CultureInfo culture = CultureInfo.InvariantCulture;
            DateTime dt = DateTime.MinValue;

            if (DateTime.TryParse(dateTimeString, out dt))
            {
                return dt.ToShortDateString();
            }
            return dateTimeString;
        }

        public static string ToDatabaseFormat(string dateTimeString)
        {
            DateTime dateTimeValue;
            bool isValueADate = DateTime.TryParse(dateTimeString, out dateTimeValue);

            // if the value could be anyhow successfully inputarsed into a DateTime..
            if (isValueADate) dateTimeString = string.Format("{0:yyyy-MM-dd}", dateTimeValue);

            return dateTimeString;
        }
    }
}
