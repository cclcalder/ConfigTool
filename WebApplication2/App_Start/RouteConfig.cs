using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            //defult route home home -- soon should be setup page
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "ngView", id = UrlParameter.Optional }
                //defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
            );
            routes.AppendTrailingSlash = true;
        }
    }
}
