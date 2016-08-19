using System.Windows;
using System.Windows.Controls;
using Telerik.Pivot.Core;

namespace Exceedra.Pivot.Converters
{
    public class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RedTemplate { get; set; }
        public DataTemplate HiddenCellTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cellAggregate = item as CellAggregateValue;
            if (cellAggregate != null)
            {
                var description = cellAggregate.Description as PropertyAggregateDescription;
                if (description != null && description.CustomName != null && description.CustomName.ToLower().StartsWith("hidden"))
                    return HiddenCellTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}