using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.DataAccess.Schedule
{
    public class ScheduleAccess
    {
        private const string IndividualTemplate =
            @"<{0}> 
                    <User_Idx>{1}</User_Idx>
                    <SalesOrg_Idx>{2}</SalesOrg_Idx>
                    <Item_Type_Name>{3}</Item_Type_Name>    
                    <Item_Idx>{4}</Item_Idx>
            </{0}>";

        public Task<XElement> GetSingleTimelineItem(string idx, string type, string flag)
        {
            var arguments = XElement.Parse(IndividualTemplate.FormatWith("GetGrid", User.CurrentUser.ID,
                User.CurrentUser.SalesOrganisationID, type, idx));
            arguments.Add(new XElement("Operation", flag));

            return WebServiceProxy.CallAsync(StoredProcedure.Schedule.GetSingleItem, arguments)
                .ContinueWith(t => GetSingleItemContinuation(t));
        }

        private XElement GetSingleItemContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            return task.Result;
        }

        public XElement SaveSingleTimelineItem(XElement xml)
        {
            var arguments = xml;
            return WebServiceProxy.Call(StoredProcedure.Schedule.SaveSingleItem, arguments);
        }

        public XElement CopySingleTimelineItem(XElement xml)
        {
            var arguments = xml;
            return WebServiceProxy.Call(StoredProcedure.Schedule.CopySingleItem, arguments);
        }

        public XElement DeleteSingleTimelineItem(XElement xml)
        {
            var arguments = xml;
            return WebServiceProxy.Call(StoredProcedure.Schedule.DeleteSingleItem, arguments);
        }
    }
}
