using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity
{
    public class ConditionDetail
    {
        public string CustomerLevel1Id { get; set; }
        public IList<string> CustomerLevel2Ids { get; set; }
        public string ProductLevel1Id { get; set; }
        public IList<string> ProductLevel2Ids { get; set; }
        public string ConditionTypeId { get; set; }
        public string ConditionTypeIndicator { get; set; }
        
        public bool ShowChildSelection { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PercentChange { get; set; }
        public string AbsoluteChange { get; set; }
        public string ChangeFixed { get; set; }
        public string ScenarioId { get; set; }
        public string ConditionReasonId { get; set; }
        public string ConditionStatusId { get; set; }
        public string SalesOrg_Idx { get; set; }
        public string Name { get; set; }
        
        public bool isEditable { get; set; }

        

        public static ConditionDetail FromXml(XElement node)
        {
            const string customerLevel1Id = "CustomerLevel1Id";
            const string customerLevel2Id = "CustomerLevel2Id";
            const string productLevel1Id = "ProductLevel1Id";
            const string productLevel2Id = "ProductLevel2Id";
            const string customerProductMeasureId = "CustomerProductMeasureId";
            const string conditionTypeIndicator = "ConditionTypeIndicator";
            const string startDateId = "StartDateId";
            const string endDateId = "EndDateId";
            const string changePercentage = "ChangePercentage";
            const string changeDelta = "ChangeDelta";
            const string changeFixed = "ChangeFixed";
            const string format = "yyyyMMdd";
            const string statusId = "Cond_Status_Idx";
            const string custIdx = "Cust_Idx";
            const string prodIdx = "Prod_Idx";
            const string scenIdx = "Scen_Idx";
            const string iseditable = "IsEditable";
            //const string scenarioIds = "ScenarioIDs";
            const string condReasonIdx = "Cond_Reason_Idx";
            const string showChildSelection = "ShowChildSelections";
            const string salesOrg_Idx = "SalesOrg_Idx";
            const string conditionName = "Cond_Name";

            var reasonElement = node.MaybeElement(condReasonIdx);
            return new ConditionDetail
            {
                CustomerLevel1Id = node.GetValue<string>(customerLevel1Id),
                CustomerLevel2Ids = node.MaybeElements(customerLevel2Id).Select(e => e.GetValue<string>(custIdx)).ToList(),
                ProductLevel1Id = node.GetValue<string>(productLevel1Id),
                ProductLevel2Ids = node.MaybeElements(productLevel2Id).Select(e => e.GetValue<string>(prodIdx)).ToList(),
                ConditionTypeId = node.GetValue<string>(customerProductMeasureId),
                ConditionTypeIndicator = node.GetValue<string>(conditionTypeIndicator),
                StartDate = DateTime.ParseExact(node.GetValue<string>(startDateId), format, null),
                EndDate = DateTime.ParseExact(node.GetValue<string>(endDateId), format, null),
                PercentChange = node.GetValue<string>(changePercentage),
                AbsoluteChange = node.GetValue<string>(changeDelta),
                ChangeFixed = node.GetValue<string>(changeFixed),
                ConditionReasonId = reasonElement == null ? null : reasonElement.Value,
                ConditionStatusId = node.GetValue<string>(statusId),
                ScenarioId = node.GetValue<string>(scenIdx),
                ShowChildSelection = node.GetValue<int>(showChildSelection) == 1 ? true : false,
                isEditable = node.GetValue<int>(iseditable) == 1 ? true : false,
                SalesOrg_Idx = node.GetValue<string>(salesOrg_Idx),
                Name = node.GetValue<string>(conditionName)
                
                
            };
        }
    }
}
