using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model
{
    [Serializable()]
    public class StagedMeasure
    {
    //    private string _value;
    //    private decimal _numericValue;

        public StagedMeasure(string measureName, string measureId, string format, XElement el)
        {
            StageId = el.GetValue<string>("ID");
            StageName = el.GetValue<string>("Name");
            MeasureName = measureName;
            MeasureId = measureId;
            Format = format;
            OriginalValue = el.GetValue<string>("Value");
            //Value = OriginalValue;
            UpdateNumericValue(OriginalValue);
            IsReadOnly = el.GetValueOrDefault<string>("IsEditable") == "0";
        }

        public string Format { get; set; }

        public string StageId { get; set; }

        public string StageName { get; set; }

        public string OriginalValue { get; set; }

        public string Value
        {
            get { return NumericValue.ToString(Format); }
            set { UpdateNumericValue(value); }
        }

        public decimal NumericValue { get; set; }

        public string MeasureName { get; set; }

        public string MeasureId { get; set; }

        public bool IsReadOnly { get; set; }

        private void UpdateNumericValue(string value)
        {
            decimal number;

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
            {
                NumericValue = number;
            }
        }
    }
}
