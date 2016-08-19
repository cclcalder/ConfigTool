using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Website.Webservices;

//using Website.Model.DataAccess;

namespace Website
{
    public class Global : System.Web.HttpApplication
    {

        void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var parsedQuery = HttpUtility.ParseQueryString(HttpContext.Current.Request.Url.Query);

            if (parsedQuery["v"].HasValue())
            {
                var q = parsedQuery["v"];
                StaticWS.version = q;
            }
            

        }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
             
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            //var user = LoginAccess.GetUser("tu", "tu");
            //if (user != null)
            //{
            //    Model.User.CurrentUser = user;
            //    HttpContext.Current.Session["user"] = user;
            //}
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
