using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.NPD
{
    public class NPDDate
    {
        public DateTime StartDate;
        public DateTime EndDate;

        public NPDDate(List<XElement> xml)
        {
            StartDate = xml.FirstOrDefault().GetValue<DateTime>("Start_Date");
            EndDate = xml.FirstOrDefault().GetValue<DateTime>("End_Date");
        }

        public NPDDate(XElement xml)
        {
            xml = (XElement) xml.FirstNode;
            StartDate = Convert.ToDateTime(xml.Element("Start_Date").MaybeValue());
            EndDate = Convert.ToDateTime(xml.Element("End_Date").MaybeValue());
        }

        public NPDDate()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
        }

        public NPDDate(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }
    }
}