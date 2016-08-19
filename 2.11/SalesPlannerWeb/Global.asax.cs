using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Model.DataAccess;
using SalesPlannerWeb.Webservices;
using StackExchange.Profiling;


namespace SalesPlannerWeb
{
    public class MvcApplication : HttpApplication
    {
        void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var parsedQuery = HttpUtility.ParseQueryString(HttpContext.Current.Request.Url.Query);

            if (parsedQuery["v"].HasValue())
            {
                var q = parsedQuery["v"];
                StaticWS.version = q;
            }
 
            if (parsedQuery["sessionID"].HasValue())
            {
                var q = parsedQuery["sessionID"];
                if (Model.User.CurrentUser == null)
                {
                    var user = LoginAccess.LoginWithSession(q);
                    Model.User.CurrentUser = user;
                    
                }
                if (Model.User.CurrentUser != null && WebConfiguration.Configuration == null)
                {
                    WebConfiguration.Configuration = new ClientConfigurationAccess().GetClientConfiguration(true);
                }
            }
             
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            StaticWS.version = System.Configuration.ConfigurationManager.AppSettings["ActiveDBVersion"];
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            }
        }
        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

    }
}
