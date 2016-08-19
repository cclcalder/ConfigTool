using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Model.Entity;

namespace Model
{
    public class ConditionMeasure
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime OriginalStartDate { get; set; }
        public DateTime OriginalEndDate { get; set; }

        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }

        public bool MarkedForDeletion { get; set; }

        //#region ExtraData
        ///// <summary>
        ///// Gets or sets the Attributes of this PromotionData.
        ///// </summary>
        //private List<ConditionExtraData> _extraDataList;
        //public List<ConditionExtraData> ExtraDataList
        //{
        //    get
        //    {
        //        return _extraDataList;
        //    }
        //    set
        //    {
        //        if (_extraDataList != value)
        //        {
        //            _extraDataList = value.ToList();
        //        }
        //    }
        //}
        //#endregion
        private static DateTime FormateDateTime(string dateString)
        {
            DateTime formatedDateTime;
            string dateFormat = "yyyyMMdd";
            formatedDateTime = DateTime.ParseExact(dateString, dateFormat, CultureInfo.CurrentCulture);

            return formatedDateTime;
        }

        public static ConditionMeasure FromXml(XElement node)
        {
            const string custIdElement = "CustIdx";
            const string custNameElement = "CustName";
            const string custCodeElement = "CustCode";
            const string prodIdElement = "ProdIdx";
            const string prodCodeElement = "ProdCode";
            const string prodNameElement = "ProdName";
            const string startDateElement = "StartDate";
            const string endDateElement = "EndDate";
            const string oldValueElement = "Old_Value";
            const string newValueElement = "New_Value";
            const string orgStartDateElement = "Original_Start_Date";
            const string orgEndDateElement = "Original_End_Date";
            const string MarkedForDeletionElement = "MarkedForDeletion";

            return new ConditionMeasure
            {
                CustomerId = node.GetValue<string>(custIdElement),
                CustomerName = node.GetValue<string>(custNameElement),
                CustomerCode = node.GetValue<string>(custCodeElement),
                ProductId = node.GetValue<string>(prodIdElement),
                ProductCode = node.GetValue<string>(prodCodeElement),
                ProductName = node.GetValue<string>(prodNameElement),
                OldValue = PopulateValue(node, oldValueElement),
                NewValue = PopulateValue(node, newValueElement),
                StartDate = FormateDateTime(node.GetValue<string>(startDateElement)),
                EndDate = FormateDateTime(node.GetValue<string>(endDateElement)),
                OriginalStartDate = (node.Element(orgStartDateElement) != null) ? FormateDateTime(node.GetValue<string>(orgStartDateElement)) : DateTime.MinValue,
                OriginalEndDate = (node.Element(orgEndDateElement) != null) ? FormateDateTime(node.GetValue<string>(orgEndDateElement)) : DateTime.MinValue,
                MarkedForDeletion = (node.Element(MarkedForDeletionElement) != null) && node.GetValue<int>(MarkedForDeletionElement) == 1 ? true : false
            };
        }

        private static decimal PopulateValue(XElement node, string elementName)
        {
            decimal returnValue;

            var element = node.Element(elementName);

            if (element == null)
            {
                returnValue = 0;
            }
            else
            {
                returnValue = Decimal.Round(decimal.Parse(element.Value, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint), 2);
            }

            return returnValue;
        }
    }
}
