using System.Windows;
using System.Windows.Controls;
using Exceedra.Controls.DynamicGrid.Models;

namespace Exceedra.DynamicGrid.Converters
{
    public class RowDetailsTemplateSelector : DataTemplateSelector
    {

        public DataTemplate TabbedViewTemplate { get; set; }
        public DataTemplate DynamicGridTemplate { get; set; }
        public DataTemplate ChartTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var rec = (Record)item;
            if (rec == null || rec.DetailsViewModel == null) return null;

            var x = rec.DetailsViewModel.GetType();

            switch (x.Name)
            {
                case "TabbedViewModel":
                    return TabbedViewTemplate;

                case "RecordViewModel":
                    return x.FullName.Contains("Grid") ? DynamicGridTemplate : ChartTemplate;
                default:
                    return null;
            }
        }

    }
}