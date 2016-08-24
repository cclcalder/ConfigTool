using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class SetupController : Controller
    {
        // GET: Setup
        public ActionResult Setup()
        {
            //speak to SetupModel and update View
            return View();
        }
    }
}