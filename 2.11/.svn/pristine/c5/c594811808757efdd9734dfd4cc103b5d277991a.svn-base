using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Exceedra.Common;
using Model.DataAccess.Generic;
using Model.Entity.Generic;

namespace Model.DataAccess
{
    public class AnalyticsAccess
    {

        public Task<XElement> GetReportTree()
        {
            string arguments = "<GetData><User_Idx>{0}</User_Idx><SalesOrg_Idx>{1}</SalesOrg_Idx></GetData>"
                                .FormatWith(User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID);

            return WebServiceProxy.CallAsync(StoredProcedure.Analytics.GetReports, XElement.Parse(arguments));

        }


        //private const string GetByUserIdTemplate = @"<GetUserReportsList>
        //                                                <User_Idx>{0}</User_Idx>                                                        
        //                                              </GetUserReportsList>";
        //private const string GetUserReportByName = @"<GetUserReport><User_Idx>{0}</User_Idx><TypeCode>{1}</TypeCode><ReportName>{2}</ReportName></GetUserReport>";
        ///// <summary>
        ///// Get all user reports for this type
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public IEnumerable<AnalyticsGroup> GetUserCubesList()
        //{

        //    //EXEC app.Procast_SP_ANALYTICS_GetUserReportsList
        //    //'
        //    //  <GetUserCubeList>
        //    //    <User_Idx>1</User_Idx>
        //    //    <TypeCode>CUBE</TypeCode>
        //    //  </GetUserCubeList>
        //    //'

        //    var arguments = GetByUserIdTemplate.FormatWith(User.CurrentUser.ID);
        //    var u = User.CurrentUser.ID;
        //    var s = new List<string>();
        //    var res = new List<AnalyticsGroup>();

        //    try
        //    {
        //        var items = WebServiceProxy.Call(StoredProcedure.Analytics.GetUserCubesList, XElement.Parse(arguments), DisplayErrors.No);

        //        if (items != null)
        //        {
        //            res.AddRange(items.Elements().Select(m => AnalyticsGroup.FromXml(m)));
        //        }
        //    }
        //    catch// (Exception ex)
        //    {

        //    }

        //    return res;
        //}

        //private static IList<AnalyticsGroup> GetCubesContinuation(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //        return new List<AnalyticsGroup>();

        //    return task.Result.Elements().Select(AnalyticsGroup.FromXml).ToList();
        //}



        //public IEnumerable<AnalyticsGroup> GetUserReportsList()
        //{

        //    //EXEC app.Procast_SP_ANALYTICS_GetUserReportsList
        //    //'
        //    //  <GetUserReportsList>
        //    //    <User_Idx>1</User_Idx>
        //    //    <TypeCode>CUBE</TypeCode>
        //    //  </GetUserReportsList>
        //    //'

        //    var arguments = GetByUserIdTemplate.FormatWith(User.CurrentUser.ID);
        //    var u = User.CurrentUser.ID;
        //    var s = new List<string>();
        //    var res = new List<AnalyticsGroup>();

        //    try
        //    {
        //        var items = WebServiceProxy.Call(StoredProcedure.Analytics.GetUserReportsList, XElement.Parse(arguments), DisplayErrors.No);

        //        if (items != null)
        //        {
        //            res.AddRange(items.Elements().Select(m => AnalyticsGroup.FromXml(m)));
        //        }
        //    }
        //    catch// (Exception ex)
        //    {

        //    }

        //    return res;
        //}

        //private static IList<AnalyticsGroup> GetReportsContinuation(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //        return new List<AnalyticsGroup>();

        //    return task.Result.Elements().Select(AnalyticsGroup.FromXml).ToList();
        //}




        //public static string SerializeToString(object obj)
        //{
        //    XmlSerializer serializer = new XmlSerializer(obj.GetType());

        //    using (StringWriter writer = new StringWriter())
        //    {
        //        serializer.Serialize(writer, obj);

        //        return writer.ToString();
        //    }
        //}

        //public string GetUserReport(string id, string type)
        //{
        //    //EXEC app.Procast_SP_ANALYTICS_GetUserReport
        //    //'
        //    //  <GetUserReport>
        //    //    <User_Idx>1</User_Idx>
        //    //    <TypeCode>CUBE</TypeCode>
        //    //    <ReportName>Test Report</ReportName>
        //    //  </GetUserReport>
        //    //'

        //    var arguments = GetUserReportByName.FormatWith(User.CurrentUser.ID, type, id);
        //    var u = User.CurrentUser.ID;

        //    var report = WebServiceProxy.Call(StoredProcedure.Analytics.GetUserReport, XElement.Parse(arguments), DisplayErrors.No);
        //    report.Elements("User_Idx").Remove();
        //    report.Elements("TypeCode").Remove();
        //    report.Elements("ReportName").Remove();
        //    report.Elements("ReportID").Remove();

        //    return report.ToString();
        //}

        //<DeleteUserReport>
        //    <User_Idx>1</User_Idx>
        //    <Report_Idx>1</Report_Idx>
        //</DeleteUserReport>

        public Task<SprocResult> DeleteUserReport(string ReportID)
        {
            const string getSalesOrgTemplate = "<DeleteUserReport><User_Idx>{0}</User_Idx><Report_Idx>{1}</Report_Idx></DeleteUserReport>";
            var arguments = getSalesOrgTemplate.FormatWith(User.CurrentUser.ID, ReportID);

            var u = User.CurrentUser.ID;
            return WebServiceProxy.CallAsync(StoredProcedure.Analytics.DeleteUserReport, XElement.Parse(arguments))
              .ContinueWith(t => ContinuationS(t));

        }

        private SprocResult ContinuationS(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = "Success";
            }
            else
            {
                result.Success = true;
                result.Message = "Nothing deleted";
            }


            return result;
        }

        public Task<XElement> SaveReport(XElement settings)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Analytics.SaveUserReportLayout, settings)
                .ContinueWith(t => ContinuationXelement(t));
        }

        private static XElement ContinuationXelement(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return null;

            return task.Result;
        }
        private static bool Continuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return false;
            return true;
        }

        public Task<IEnumerable<ComboboxItem>> GetUsers()
        {
            const string arguments = "<GetUserList></GetUserList>";

            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.Analytics.GetUserList,
                XElement.Parse(arguments));
        }

        public XElement GetUserReport(string idx)
        {
            string arguments = "<GetData><User_Idx>{0}</User_Idx><SalesOrg_Idx>{1}</SalesOrg_Idx><Report_Idx>{2}</Report_Idx></GetData>"
                                .FormatWith(User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID,idx);

            return WebServiceProxy.Call(StoredProcedure.Analytics.GetUserReport, XElement.Parse(arguments));
        }
    }
}


//EXEC app.Procast_SP_ANALYTICS_SaveUserReportLayout
//'
//  <DataServiceProvider>
//      <User_Idx>1</User_Idx>
//      <TypeCode>CUBE</TypeCode>
//      <ReportName>Test Report</ReportName>
//      <TEST>Hi</TEST>
//  </DataServiceProvider>
//'


//EXEC app.Procast_SP_ANALYTICS_GetUserReport
//'
//  <GetUserReport>
//    <User_Idx>1</User_Idx>
//    <TypeCode>CUBE</TypeCode>
//    <ReportName>Test Report</ReportName>
//  </GetUserReport>
//'

//EXEC app.Procast_SP_ANALYTICS_DeleteUserReport
//'
//<DeleteUserReport>
//    <User_Idx>1</User_Idx>
//    <Report_Idx>1</Report_Idx>
//</DeleteUserReport>

//'

