using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model
{
    public class StagedProduct
    {
        public StagedProduct(XElement el)
        {
            Measures = new List<StagedMeasure>();
            ProductId = el.GetValue<string>("ID");
            ProductName = el.Element("Sku_Name").MaybeValue();

            var measures = el.Element("Measures");

            if (measures != null)
            {
                foreach (var measure in measures.Elements())
                {
                    var times = measure.Element("Times");

                    if (times != null)
                    {
                        foreach (var time in times.Elements())
                        {
                            Measures.Add(new StagedMeasure(measure.GetValue<string>("Name"), measure.GetValue<string>("ID"), measure.GetValue<string>("Format"), time));
                        }
                    }
                }
            }
        }

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public List<StagedMeasure> Measures { get; set; }

        public XElement CreateSaveArgument()
        {
            var productArg = new XElement("Product");
            productArg.Add(new XElement("ID", ProductId));

            var measuresArg = new XElement("Measures");
            var measureNames = Measures.Select(arg => arg.MeasureName).Distinct().ToList();

            foreach (var measure in measureNames)
            {
                var measureNode = new XElement("Measure");
                var measureItems = Measures.Where(arg => arg.MeasureName == measure).ToList();
                measureNode.Add(new XElement("ID", measureItems.First().MeasureId));
                measureNode.Add(new XElement("Name", measureItems.First().MeasureName));

                var timesArg = new XElement("Times");

                foreach (var item in measureItems)
                {
                    var timeNode = new XElement("Time");
                    timeNode.Add(new XElement("ID", item.StageId));
                    timeNode.Add(new XElement("Name", item.StageName));
                    timeNode.Add(new XElement("Value", item.NumericValue.ToString(CultureInfo.InvariantCulture)));
                    timeNode.Add(new XElement("IsEditable", item.IsReadOnly ? 0 : 1));
                    timesArg.Add(timeNode);
                }

                measureNode.Add(timesArg);
                measuresArg.Add(measureNode);
            }

            productArg.Add(measuresArg);

            return productArg;
        }
    }
}
