using System.Windows;
using System.Windows.Controls;
using Exceedra.Controls.DynamicGrid.Models;

namespace Exceedra.Controls.DynamicTab.Controls
{

    public class TabControlTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HorizontalGridTemplate { get; set; }
        public DataTemplate VerticalGridTemplate { get; set; }
        public DataTemplate ScheduleGridTemplate { get; set; }
        public DataTemplate ChartTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cf = (Property)item;
            switch (cf.ControlType)
            {
                case "HorizontalGrid": return HorizontalGridTemplate;
                case "VerticalGrid": return VerticalGridTemplate;
                case "ScheduleGrid": return ScheduleGridTemplate;
                case "Chart": return ChartTemplate;
                default: return DefaultTemplate;
            }


            //return null;
        }

    }
}