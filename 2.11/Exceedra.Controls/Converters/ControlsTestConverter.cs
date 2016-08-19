using System;
using System.Windows;
using System.Windows.Controls;

namespace Exceedra.Converters
{
    public class ControlsTestConverter : DataTemplateSelector
    {
        public DataTemplate DynamicGridTemplate { get; set; }
        public DataTemplate SlimGridTemplate { get; set; }
        public DataTemplate VerticalGridTemplate { get; set; }
        public DataTemplate ChartTemplate { get; set; }
        public DataTemplate PivotTemplate { get; set; }
        public DataTemplate TreeGridTemplate { get; set; }
        public DataTemplate LabelTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var i = (ContentObject)item ?? new ContentObject();
            switch (i.Type)
            {
                case "DynamicGrid": return DynamicGridTemplate;
                case "SlimGrid": return SlimGridTemplate;
                case "VerticalGrid": return VerticalGridTemplate;
                case "Chart": return ChartTemplate;
                case "Pivot": return PivotTemplate;
                case "TreeGrid": return TreeGridTemplate;
                default: return LabelTemplate;
            }
        }
    }

    public class ContentObject
    {
        public string Type { get; set; }
        public object ViewModel { get; set; }
    }
}
