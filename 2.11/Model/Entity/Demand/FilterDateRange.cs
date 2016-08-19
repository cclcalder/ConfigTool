using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.Demand
{
    [Serializable]
    public class FilterDateRange
    {
        public DateTime StartDate;
        public DateTime EndDate;

        public FilterDateRange(XElement xml)
        {
            try
            {
                StartDate =
                    Convert.ToDateTime(xml.Descendants("Start_Date").FirstOrDefault().MaybeValue() ??
                                       xml.Descendants("Start").FirstOrDefault().MaybeValue());
                EndDate =
                    Convert.ToDateTime(xml.Descendants("End_Date").FirstOrDefault().MaybeValue() ??
                                       xml.Descendants("End").FirstOrDefault().MaybeValue());
            }
            catch //A very annoying special case for the scenarios screen. If it the xml ever gets refactored we can delete this.
            {
                StartDate = DateTime.ParseExact(xml.Descendants("Start_Date").FirstOrDefault().MaybeValue() ??
                    xml.Descendants("Start").FirstOrDefault().MaybeValue(), "yyyyMMdd", CultureInfo.CurrentCulture);
                EndDate = DateTime.ParseExact(xml.Descendants("End_Date").FirstOrDefault().MaybeValue() ?? 
                    xml.Descendants("End").FirstOrDefault().MaybeValue(), "yyyyMMdd", CultureInfo.CurrentCulture);
            }
        }

        public FilterDateRange()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
        }

        public FilterDateRange(DateTime? start, DateTime? end)
        {
            StartDate = start ?? DateTime.Today;
            EndDate = end ?? DateTime.Today;
        } 
    }
}