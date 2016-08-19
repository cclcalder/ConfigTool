using Model.Entity.Diagnostics;
using System;
using System.Windows.Data;

namespace Exceedra.Caret
{

        public class ImageUrlConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter,
               System.Globalization.CultureInfo culture)
            {
                return string.Format("{0}/images/{1}", new SiteData().BaseURL, parameter);
            }

            public object ConvertBack(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    
}
