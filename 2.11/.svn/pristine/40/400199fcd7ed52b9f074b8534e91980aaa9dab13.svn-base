using System;
using System.Xml.Linq;
using Exceedra.Common;
using Model.Entity.Generic;

namespace Model.Entity.Funds
{
    public class FundImpact : ComboboxItem
    {
        public FundImpact(XElement xml)
            : base(xml)
        {
            Format = xml.Element("Format").MaybeValue();
            MultiplicationFactor = Convert.ToDecimal(xml.Element("MultiplicationFactor").MaybeValue());
        }

        public FundImpact()
        {

        }

        private decimal _multiplicationFactor;
        public decimal MultiplicationFactor
        {
            get
            {
                if (Format.ToLowerInvariant().Contains("p"))
                    return _multiplicationFactor / 100;
                return _multiplicationFactor;
            }
            set { _multiplicationFactor = value; }
        }

        public string Format { get; set; }
    }
}
