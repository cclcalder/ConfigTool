using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Model.Enumerators;

namespace WPF.Converters
{
    public class TypeToStyleConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            switch((StyleType)value)
            {
                //Commented out for now until we decided how we want things to look. SecondaryBtn style is quick simplistic so is a good placeholder.

                //case StyleType.Primary:
                //    return Application.Current.Resources["PrimaryBtn"] as Style;
                //case StyleType.Secondary:
                //    return Application.Current.Resources["SecondaryBtn"] as Style;
                //case StyleType.Info:
                //    return Application.Current.Resources["InfoBtn"] as Style;
                //case StyleType.Danger:
                //    return Application.Current.Resources["DangerBtn"] as Style;
                //case StyleType.Success:
                //    return Application.Current.Resources["SuccessBtn"] as Style;
                //case StyleType.Warning:
                //    return Application.Current.Resources["WarningBtn"] as Style;
                default:
                    return Application.Current.Resources["SecondaryBtn"] as Style;
            }

            
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
