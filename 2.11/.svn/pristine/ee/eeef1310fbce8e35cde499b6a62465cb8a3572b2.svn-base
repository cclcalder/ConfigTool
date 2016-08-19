using System.Windows;
using System.Windows.Controls;
using Exceedra.Controls.DynamicRow.Models;

namespace Exceedra.DynamicGrid.Converters
{
    class RowTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CheckboxTemplate { get; set; }
        public DataTemplate SingleSelectComboboxTemplate { get; set; }
        public DataTemplate TextboxTemplate { get; set; }
        public DataTemplate DatePickerTemplate { get; set; }
        public DataTemplate FileSelectorTemplate { get; set; }
        public DataTemplate MultiSelectComboboxTemplate { get; set; }
        public DataTemplate LabelTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cf = (RowProperty)item ?? new RowProperty();
            switch (cf.ControlType.ToLower())
            {
                case "checkbox": return CheckboxTemplate;
                case "dropdown": return SingleSelectComboboxTemplate;
                case "textbox": return TextboxTemplate;
                case "datepicker": return DatePickerTemplate;
                case "folderpicker":
                    return FileSelectorTemplate;
                case "multiselectdropdown":
                    return MultiSelectComboboxTemplate;
                case "hyperlink": return LabelTemplate;
                default:
                    return LabelTemplate;
            }
        }
    }
}
