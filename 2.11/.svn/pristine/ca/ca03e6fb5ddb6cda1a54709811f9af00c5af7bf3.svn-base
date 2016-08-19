using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Chart.Model;
using Exceedra.Common;
using Model;

namespace Exceedra.Chart.Converters
{
    public class GridToChart
    {
        public static Model.Chart Convert(XElement xml)
        {
            var loadedChart = new Model.Chart
            {
                ChartType = "Categorical",
                YAxisType = "Linear",
                XAxisType = "Categorical",
                YAxisTitle = "Volume",
                XAxisTitle = "Week",
                Series = new ObservableCollection<SingleSeries>(),
                Outliers = new List<string>()
            };

            var potentialSeries = xml.Descendants("RootItem");

            foreach (var series in potentialSeries)
            {
                if (series.Element("Item_IsDisplayed").MaybeValue() != "1") continue;

                var singleSeries = new SingleSeries
                {
                    SeriesName = series.Element("Item_Type").MaybeValue(),
                    SeriesType = "Line",
                    Datapoints = new ObservableCollection<Datapoint>()
                };

                var attributes = series.Descendants("Attribute");

                if (singleSeries.SeriesName.Contains("Outlier"))
                {
                    foreach (var attribute in attributes.Where(a => a.Element("Value").MaybeValue().ToLower() == "true"))
                    {
                        loadedChart.Outliers.Add(attribute.Element("HeaderText").MaybeValue());
                    }
                    continue;
                }

                foreach (var attribute in attributes)
                {
                    var code = attribute.Element("ColumnCode").MaybeValue();
                    var value = attribute.Element("Value").MaybeValue();

                    if (string.IsNullOrEmpty(code) || code == "Row_Name")
                    {
                        singleSeries.SeriesName = value;
                        continue;
                    }

                    decimal tempY;
                    var isNum = decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out tempY);

                    if (isNum)
                    {
                        var datapoint = new CategoricalDatapoint
                        {
                            X = code,
                            Y = tempY
                        };
                        singleSeries.Datapoints.Add(datapoint);
                    }
                    else
                    {
                        var datapoint = new CategoricalDatapoint
                        {
                            X = code                           
                        };
                        singleSeries.Datapoints.Add(datapoint);
                    }
                }

                loadedChart.Series.Add(singleSeries);

            }

            return loadedChart;
        }
    }
}