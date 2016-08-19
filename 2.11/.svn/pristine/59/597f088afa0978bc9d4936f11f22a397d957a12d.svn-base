using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Chart.Model;
using Exceedra.Common;
using Model;

namespace Exceedra.Chart.Converters
{
    public class XmlToDatapoints
    {
        public static List<LinearDatapoint> LoadLinearDatapoints(List<XElement> dpCollection)
        {
            return dpCollection.Where(e => e.Name.LocalName.ToString().ToLowerInvariant() == "datapoint").Select(datapoint => new LinearDatapoint
            {
                Name = datapoint.Element("Name").MaybeValue(),
                X = datapoint.Element("X") == null ? 0 : Decimal.Parse(datapoint.Element("X").MaybeValue()),
                Y = datapoint.Element("Y") == null ? 0 : Decimal.Parse(datapoint.Element("Y").MaybeValue()),
                Header1 = datapoint.Element("Tooltip_Header1").MaybeValue() ?? "Header1",
                Header2 = datapoint.Element("Tooltip_Header2").MaybeValue() ?? "Header2",
                Header3 = datapoint.Element("Tooltip_Header3").MaybeValue() ?? "Header3",
                Color = datapoint.Element("Tooltip_Color").MaybeValue() ?? "#000000",
                ID = datapoint.Element("Idx").MaybeValue() ?? "-1",
                NavigationType = datapoint.Element("Navigation_Type").MaybeValue()
            }).ToList();
        }

        public static List<CategoricalDatapoint> LoadCategoricalDatapoints(List<XElement> dpCollection)
        {
            return dpCollection.Where(e => e.Name.LocalName.ToString().ToLowerInvariant() == "datapoint").Select(datapoint => new CategoricalDatapoint
            {
                X = datapoint.Element("X").MaybeValue(),
                Y = datapoint.Element("Y") == null ? (decimal?) null : Decimal.Parse(datapoint.Element("Y").MaybeValue(), NumberStyles.Any),
                Header1 = datapoint.Element("Tooltip_Header1").MaybeValue() ?? "Header1",
                Header2 = datapoint.Element("Tooltip_Header2").MaybeValue() ?? "Header2",
                Header3 = datapoint.Element("Tooltip_Header3").MaybeValue() ?? "Header3",
                Color = datapoint.Element("Tooltip_Color").MaybeValue() ?? "#000000",
                ID = datapoint.Element("Idx").MaybeValue() ?? "-1",
                NavigationType = datapoint.Element("Navigation_Type").MaybeValue()
            }).ToList();
        }

        public static List<RangeDatapoint> LoadRangeDataPoints(List<XElement> dpCollection)
        {
            return dpCollection.Where(e => e.Name.LocalName.ToString().ToLowerInvariant() == "datapoint").Select(datapoint => new RangeDatapoint
            {
                //X = datapoint.Element("X") == null ? DateTime.Today : DateTime.ParseExact(datapoint.Element("X").MaybeValue(), "yyyyMMdd", null),
                X = datapoint.Element("X").MaybeValue(),
                LowRange = datapoint.Element("Y1") == null ? 0 : Double.Parse(datapoint.Element("Y1").MaybeValue()),
                HighRange = datapoint.Element("Y2") == null ? 0 : Double.Parse(datapoint.Element("Y2").MaybeValue()),
                Header1 = datapoint.Element("Tooltip_Header1").MaybeValue() ?? "Header1",
                Header2 = datapoint.Element("Tooltip_Header2").MaybeValue() ?? "Header2",
                Header3 = datapoint.Element("Tooltip_Header3").MaybeValue() ?? "Header3",
                Color = datapoint.Element("Tooltip_Color").MaybeValue() ?? "#000000",
                ID = datapoint.Element("Idx").MaybeValue() ?? "-1",
                NavigationType = datapoint.Element("Navigation_Type").MaybeValue()
            }).ToList();
        }
    }
}