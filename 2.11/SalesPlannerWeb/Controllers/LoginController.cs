using System.Web.Mvc;
using Model;
using Model.DataAccess;
using SalesPlannerWeb.Models;

namespace SalesPlannerWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUser _userAccessor;
        private readonly IClientConfigurationAccess _clientConfigurationAccessor;

        public LoginController() : this(new User(), new ClientConfigurationAccess()) { }
        public LoginController(IUser userAccessor, IClientConfigurationAccess clientConfigurationAccessor)
        {
            _userAccessor = userAccessor;
            _clientConfigurationAccessor = clientConfigurationAccessor;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var user = _userAccessor.LogIn(model.Login, model.Password);

            if (user == null || string.IsNullOrEmpty(user.ID))
            {
                TempData["ActionResponse"] = new Message
                {
                    Text = "Invalid username or password",
                    Type = MessageType.Warning
                };

                return View("Index", model);
            }

            Model.User.CurrentUser = user;
            WebConfiguration.Configuration = _clientConfigurationAccessor.GetClientConfiguration();

            return RedirectToAction("Index", "ListingsMgmt");
        }
    }
}