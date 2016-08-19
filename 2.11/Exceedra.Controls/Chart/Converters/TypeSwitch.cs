using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;
using System.Windows.Media;
using Telerik.Charting;

namespace Exceedra.Chart.Converters
{
    class TypeSwitch : DependencyObject
    {
        public static readonly DependencyProperty SeriesTypeProperty = DependencyProperty.RegisterAttached("SeriesType",
                    typeof(string), typeof(TypeSwitch), new PropertyMetadata(OnSeriesTypeChanged));

        public static string GetSeriesType(DependencyObject obj)
        {
            return (string)obj.GetValue(SeriesTypeProperty);
        }

        public static void SetSeriesType(DependencyObject obj, string value)
        {
            obj.SetValue(SeriesTypeProperty, value);
        }

        private static void OnSeriesTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadCartesianChart chart = sender as RadCartesianChart;
            if (chart == null)
                return;

            string seriesType = e.NewValue as string;
            chart.Series.Clear();
            foreach (CartesianSeries series in GetSeries(chart, seriesType))
            {
                chart.Series.Add(series);
            }

            CategoricalAxis categoricalAxis = chart.HorizontalAxis as CategoricalAxis;
            if (categoricalAxis != null)
            {
                AxisPlotMode plotMode = AxisPlotMode.BetweenTicks;

                categoricalAxis.PlotMode = plotMode;
            }
        }

        private static IEnumerable<CartesianSeries> GetSeries(RadCartesianChart chart, string seriesType)
        {
            List<CartesianSeries> generatedSeries = new List<CartesianSeries>();
            for (int i = 0; i < 2; i++)
            {
                CategoricalSeries series = null;

                series = new PointSeries();
                //switch (seriesType)
                //{
                //    case "Line":
                //        series = new LineSeries();
                //        break;
                //    case "Point":
                //        series = new PointSeries();
                //        break;
                //}
                //if (seriesType == "Line")
                //{
                //    string pointTemplatePath = string.Format("PointTemplate{0}", i + 1);
                //    series.PointTemplate = chart.Resources[pointTemplatePath] as DataTemplate;
                //}
                series.CategoryBinding = new PropertyNameDataPointBinding("Month");
                series.ValueBinding = new PropertyNameDataPointBinding("Revenue");
                string itemsSourcePath = string.Format("Collection{0}", i + 1);
                series.SetBinding(CategoricalSeries.ItemsSourceProperty, new Binding(itemsSourcePath));
                series.SetBinding(CategoricalSeries.ShowLabelsProperty, new Binding("ShowLabels"));
                series.SetBinding(CategoricalSeries.CombineModeProperty, new Binding("SelectedCombineMode"));
                generatedSeries.Add(series);
            }
            return generatedSeries;
        }
    }
}

