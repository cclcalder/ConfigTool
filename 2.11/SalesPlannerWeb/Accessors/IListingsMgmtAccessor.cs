using System.Xml.Linq;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Model.Entity.Listings;
using SalesPlannerWeb.Models;

namespace SalesPlannerWeb.Accessors
{
    public interface IListingsMgmtAccessor
    {
        RecordViewModel GetListings();
        RecordViewModel GetListings(XElement xFilters);

        RowViewModel GetListingDetails(string listingIdx);
        SingleSelectViewModel GetListingCustomerDropdown(string listingIdx);
        TreeViewHierarchy GetListingProductsRootNode(string listingIdx);

        Message RemoveListing(string listingIdx);

        Message SaveListing(Listing listing);
    }
}
