using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.DataAccess;

namespace Model.Entity.Canvas
{
    public class CanvasAccessor : ICanvasAccessor
    {
        private const string GetInsightsString = @"<{0}><User_Idx>{1}</User_Idx></{0}>";
        private const string GetInsightFiltersString = @"<{0}><User_Idx>{1}</User_Idx><Canvas_Report_Idx>{2}</Canvas_Report_Idx></{0}>";
        private const string SaveInsightDefaultsString = @"<{0}><User_Idx>{1}</User_Idx><SalesOrg_Idx>{2}</SalesOrg_Idx><MenuItem_Idx>{3}</MenuItem_Idx>{4}</{0}>";

        public Task<IList<Insight>> GetInsights()
        {
            const string getMenuItems = "GetMenuItems";
            string arguments = GetInsightsString.FormatWith(getMenuItems, User.CurrentUser.ID);

            return WebServiceProxy.CallAsync(StoredProcedure.Insights.GetMenuItemList, XElement.Parse(arguments))
                .ContinueWith(t => GetInsightsContinuation(t));
        }

        private IList<Insight> GetInsightsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<Insight>();

            var insights =
                (
                    from insight in task.Result.Elements("Item")
                    select new Insight(insight)
                ).ToList();

            return insights;
        }

        public Task<XElement> GetFilters(string insightId)
        {
            const string getFilters = "GetFilters";
            string arguments = GetInsightFiltersString.FormatWith(getFilters, User.CurrentUser.ID, insightId);

            return WebServiceProxy.CallAsync(StoredProcedure.Insights.GetFilters, XElement.Parse(arguments));
        }

        private object GetFiltersContinuation(Task<XElement> task)
        {
            //TODO: when dividing filters into columns will be moved to the dynamic row control we'll use this method to pass here an xml returned from the db, use it as a new-control-view-model constructor argument and we'll return this new-control-view-model back to GetFilters method
            return null;
        }

        public Task<string> SaveDefaults(string insightId, XElement filters)
        {
            const string getGrid = "SaveDefaults";
            string arguments = SaveInsightDefaultsString.FormatWith(getGrid, User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID, insightId, filters);

            return WebServiceProxy.CallAsync(StoredProcedure.Insights.SaveDefaults, XElement.Parse(arguments))
                .ContinueWith(t => SaveDefaultsContinuation(t));
        }

        private string SaveDefaultsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return task.Exception != null ? task.Exception.Message : "An error occurred.";

            var result = task.Result;
            var message = string.Empty;

            var xSuccessMessage = result.Element("SuccessMessage");
            if (xSuccessMessage != null) message = xSuccessMessage.Value;

            return message;
        }
    }
}