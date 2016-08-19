using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SalesPlannerWeb.Helpers
{
    public class ExceedraAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (Model.User.CurrentUser != null && Model.User.CurrentUser.ID != null)
                return true;

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        controller = "Login",
                        action = "Index"
                    })
                );
        }
    }
}