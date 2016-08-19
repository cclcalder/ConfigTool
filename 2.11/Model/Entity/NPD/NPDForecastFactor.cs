using System.Xml.Linq;
using Exceedra.Common;
using Model.Entity.Generic;

namespace Model.Entity.NPD
{
    public class NPDForecastFactor : ComboboxItem
    {
        public NPDForecastFactor() { }

        public NPDForecastFactor(XElement xml) : base(xml)
        {
            Operator = xml.Element("Operator").MaybeValue();
        }

        public string Operator { get; set; }
    }
}