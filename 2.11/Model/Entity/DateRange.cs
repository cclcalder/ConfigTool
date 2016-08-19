﻿using System;
using System.Xml.Linq;

namespace Model
{
    public class DateRange
    {
        #region StartDate
        /// <summary>
        /// Gets or sets the StartDate of this PromotionDate.
        /// </summary>
        public DateTime StartDate { get; set; }
        #endregion

        #region EndDate
        /// <summary>
        /// Gets or sets the EndDate of this PromotionDate.
        /// </summary>
        public DateTime EndDate { get; set; }
        #endregion

        public static DateRange FromXml(XElement node)
        {
            const string startElement = "Start";
            const string endElement = "End";
            const string format = "yyyyMMdd";
            return new DateRange
            {
                StartDate = DateTime.ParseExact(node.GetValue<string>(startElement), format, null),
                EndDate = DateTime.ParseExact(node.GetValue<string>(endElement), format, null)
            };
        }
    }
}