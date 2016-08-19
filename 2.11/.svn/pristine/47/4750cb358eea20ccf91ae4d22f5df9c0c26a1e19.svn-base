using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Exceedra.Controls.DynamicGrid.ViewModels;
using SalesPlannerWeb.Accessors;
using SalesPlannerWeb.Helpers;
using SalesPlannerWeb.Models;

using Listing = SalesPlannerWeb.Models.Listing;

namespace SalesPlannerWeb.Controllers
{
    [ExceedraAuthorize]
    public class ListingsMgmtController : Controller
    {
        private readonly IListingsMgmtAccessor _accessor;

        /// <summary>
        /// Setting a default accessor for the controller to ListingsMgmtAccessor
        /// </summary>
        public ListingsMgmtController() : this(new ListingsMgmtAccessor()) { }
        public ListingsMgmtController(IListingsMgmtAccessor accessor)
        {
            _accessor = accessor;

            if (Model.User.CurrentUser != null)
                @ViewBag.UserLang = Model.User.CurrentUser.LanguageCode;
        }

        public ViewResult Index()
        {
            RecordViewModel dynamicGrid = _accessor.GetListings();

            return View(dynamicGrid);
        }

        [HttpPost]
        public ViewResult GetList([BindXml]XElement xFilters)
        {
            RecordViewModel dynamicGrid = _accessor.GetListings(xFilters);

            return View("Index", dynamicGrid);
        }

        public PartialViewResult NewOrEdit(string listingIdx)
        {
            // Listing idx is stored somewhere inside of model.Details
            // but to make life easier we'll store it in a view bag dynamic property
            @ViewBag.ListingIdx = listingIdx;

            var model = new Listing
            {
                Customers = _accessor.GetListingCustomerDropdown(listingIdx),
                Details = _accessor.GetListingDetails(listingIdx),
                ProductsRoot = _accessor.GetListingProductsRootNode(listingIdx)
            };

            return PartialView("_ListingsMgmtEditorView", model);
        }

        public JsonResult Remove(string listingIdx)
        {
            if (string.IsNullOrEmpty(listingIdx))
                throw new InvalidOperationException("Idx of listing to remove is either null or empty");

            var responseMessage = _accessor.RemoveListing(listingIdx);
            if (responseMessage.Type == MessageType.Success)
                TempData["ActionResponse"] = responseMessage;

            // JsonRequestBehavior.AllowGet - used to return JSON success message (otherwise it'd return JSON error)
            return Json(responseMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(Listing listing)
        {
            if (!ModelState.IsValid)
            {
                Message validationErrorMessage = new Message
                {
                    Type = MessageType.Warning,
                    Text = string.Join("\n", ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage))
                };

                return Json(validationErrorMessage);
            }

            var responseMessage = _accessor.SaveListing(listing);

            // We don't store responseMessage in TempData when save fails because Razor renders the listings editor before the save call 
            // so the TempData["SaveResponse"] is by then empty (therefore we send responseMessage back to the script as a JSON object)
            if (responseMessage.Type == MessageType.Success)
                TempData["ActionResponse"] = responseMessage;

            // This action is invoked by an ajax form so we just return a json object back into the script
            return Json(responseMessage);
        }
    }
}