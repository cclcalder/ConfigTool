using System.Web;
using System.Web.Mvc;

namespace WebApplication2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //Exception filters. These implement IExceptionFilter and execute if there is an unhandled exception thrown during the execution of the ASP.NET MVC pipeline. 
            filters.Add(new HandleErrorAttribute());
        }
    }
}
