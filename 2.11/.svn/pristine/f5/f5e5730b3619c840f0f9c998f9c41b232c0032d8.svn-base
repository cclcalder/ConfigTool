using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model
{
    public class PromotionDatePeriod
    {
        public PromotionDatePeriod(XElement el)
        {
            const string id = "Idx";
            const string name = "Name";
            const string isSelected = "IsSelected";
            const string promoDates = "PromoDates";
            const string value = "Value";
            const string dateGroupType = "DateGroupType";
            const string dateGroupId = "DateGroupID";

            ID = el.GetValue<string>(id);
            Name = el.GetValue<string>(name);
            IsSelected = el.GetValue<int>(isSelected) == 1;
            PromoDates = el.GetElement(promoDates).MaybeElements()
                .Select(d => new PeriodDate
                {
                    ID = d.GetValue<string>(id),
                    Value = d.GetValue<DateTime>(value),
                    DateGroupType = (PeriodDate.PeriodDateType)Enum.Parse(typeof(PeriodDate.PeriodDateType), d.GetValue<string>(dateGroupType)),
                    DateGroupID = d.GetValue<string>(dateGroupId)
                }).ToList();
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public List<PeriodDate> PromoDates { get; set; }

        public class PeriodDate
        {
            public string ID { get; set; }

            public DateTime Value { get; set; }

            public string DateGroupID { get; set; }

            public enum PeriodDateType
            {
                Start,
                End
            }

            public PeriodDateType DateGroupType { get; set; }
        }
    }
}