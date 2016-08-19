using System.Web.Mvc;
using SalesPlannerWeb.Helpers;
using SalesPlannerWeb.Models;

namespace SalesPlannerWeb.Controllers
{
    [ExceedraAuthorize]
    public class MessageController : Controller
    {
        public PartialViewResult Show(Message message)
        {
            return PartialView("_Message", message);
        }
    }
}