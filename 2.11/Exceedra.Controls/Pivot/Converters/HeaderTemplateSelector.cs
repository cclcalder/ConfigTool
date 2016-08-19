using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.Pivot;

namespace Exceedra.Pivot.Converters
{
    public class HeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HiddenHeaderTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null)
            {
                GroupData data = element.DataContext as GroupData;
                if (data != null)
                {
                    var headerName = data.Data.ToString().ToLower();
                    if (headerName.StartsWith("hidden"))
                        return HiddenHeaderTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
