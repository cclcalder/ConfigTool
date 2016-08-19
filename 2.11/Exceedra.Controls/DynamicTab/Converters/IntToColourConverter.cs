using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Exceedra.DynamicTab.Converters
{
    public class IntToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var c = Int16.Parse(value.ToString());
            switch (c)
            {
                case 1:
                    return "#FFD700";
                case 2:
                    return "#FF6347";
                default:
                    return "#ffffff";
            }

             
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    public class HasChangesCanEditBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var c = Int16.Parse(value.ToString());
            switch (c)
            {
                case 1:
                    return false;
                case 2:
                    return  false;
                default:
                    return true;
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            if (value is Boolean)
            {
                return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
