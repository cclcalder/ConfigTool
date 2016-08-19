using System; 
using System.Linq; 
using System.Windows.Automation;
using System.Windows.Data;
 

namespace WPF.Converters
{

    public class BoolToToggleStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolValue = (bool?)value;

            if (value == null)
            {
                return ToggleState.Indeterminate; 
            }

            switch (boolValue.Value)
            {
                case true:
                    return ToggleState.On;
                case false:
                    return ToggleState.Off;
                default:
                    return ToggleState.Indeterminate;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ToggleState toggleState = (ToggleState)value;
            switch (toggleState)
            {
                case ToggleState.On:
                    return true;
                case ToggleState.Off:
                    return false;
                default:
                    return null;
            }
        }
    }


   // [ValueConversion(typeof(ToggleState), typeof(string))]
    public class ToggleStateToIconFilenameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var d = App.Current.Resources.MergedDictionaries.ToList();
            switch ((ToggleState)value)
            {
                case ToggleState.Indeterminate:
                    return "";
                case ToggleState.Off:
                    return "";
                case ToggleState.On:
                    return "";
                default:
                    return null;
            }

            // or
           // return Enum.GetName(typeof(ToggleState), value) + ".jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

 
}
