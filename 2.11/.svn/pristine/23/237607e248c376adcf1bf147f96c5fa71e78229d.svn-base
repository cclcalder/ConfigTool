using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Exceedra.Web.Areas.app.Models;

namespace Exceedra.Web.Areas.app.Controllers
{
    public class PromotionController : Controller
    {
        // GET: app/Promotion
        public ActionResult Index()
        {
            var dm = new PromotionsListModel();

            return View(dm);
        }

        // GET: app/Promotion/Details/5
        public ActionResult Details(int id)
        {
            var dm = new PromotionDetailsModel(id.ToString());

            return View(dm);
        }

        // GET: app/Promotion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: app/Promotion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: app/Promotion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: app/Promotion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: app/Promotion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: app/Promotion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
