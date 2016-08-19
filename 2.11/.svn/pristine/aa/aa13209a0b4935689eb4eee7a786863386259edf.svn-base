using System.Windows;
using System.Windows.Controls;

namespace WPF.UserControls.Tabs.Models
{
    public class TabTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HorizontalGridTemplate { get; set; }
        public DataTemplate VerticalGridTemplate { get; set; }
        public DataTemplate ScheduleTemplate { get; set; }
        public DataTemplate ChartTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cf = (Tab)item;
            switch (cf.TabType)
            {
                case "HorizontalGrid": return HorizontalGridTemplate;
                case "VerticalGrid": return VerticalGridTemplate;
                case "ScheduleGrid": return ScheduleTemplate;
                case "Chart": return ChartTemplate;
                default: return DefaultTemplate;
            }
        } 
    }
}