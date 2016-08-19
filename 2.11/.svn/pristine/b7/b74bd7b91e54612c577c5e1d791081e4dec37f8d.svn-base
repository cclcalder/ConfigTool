using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Model.Entity.CellsGrid;

namespace Exceedra.CellsGrid
{
    public class CellsGridTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ChartTemplate { get; set; }
        public DataTemplate GridTemplate { get; set; }
        public DataTemplate LabelTemplate { get; set; }
        public DataTemplate NavigationTemplate { get; set; }
        public DataTemplate PivotTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var insightControl = item as InsightControl;
            if (insightControl == null)
                return base.SelectTemplate(item, container);

            // Every time when the control type changes the template will be reassigned
            PropertyChangedEventHandler lambda = null;
            lambda = (o, args) =>
            {
                if (args.PropertyName == "ControlType")
                {
                    insightControl.PropertyChanged -= lambda;
                    var cp = (ContentPresenter)container;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        cp.ContentTemplateSelector = null;
                        cp.ContentTemplateSelector = this;
                    });
                }
            };
            insightControl.PropertyChanged += lambda;

            if (insightControl.ControlType != null)
            {
                if (insightControl.ControlType.ToLower() == "chart") return ChartTemplate;
                if (insightControl.ControlType.ToLower() == "dynamicgrid") return GridTemplate;
                if (insightControl.ControlType.ToLower() == "label") return LabelTemplate;
                if (insightControl.ControlType.ToLower() == "pivotgrid") return PivotTemplate;
                if (insightControl.ControlType.ToLower() == "navigation") return NavigationTemplate;
            }

            return null;
        }
    }
}
