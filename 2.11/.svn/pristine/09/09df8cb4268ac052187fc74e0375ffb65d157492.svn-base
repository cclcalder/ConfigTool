using System.Linq;
using System.Xml.Linq;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess.Generic;
using Model.Entity.Listings;
using SalesPlannerWeb.Models;

namespace SalesPlannerWeb.Accessors
{
    public class ListingsMgmtAccessor : IListingsMgmtAccessor
    {
        public RecordViewModel GetListings()
        {
            var proc = StoredProcedure.ListingMgmnt.GetListings;

            XElement xmlIn = CommonXml.GetBaseArguments("GetListings");
            xmlIn.Add(new XElement("LoadFromDefaults", "1"));
            xmlIn.Add(new XElement("Screen_Code", "LISTINGSMGMT"));

            var dbResponse = MvcWebServiceProxy.ParseXml(proc, xmlIn);

            var listingsList = new RecordViewModel(dbResponse);
            return listingsList;
        }

        public RecordViewModel GetListings(XElement xFilters)
        {
            var proc = StoredProcedure.ListingMgmnt.GetListings;

            var dbResponse = MvcWebServiceProxy.ParseXml(proc, xFilters);

            var listingsList = new RecordViewModel(dbResponse);
            return listingsList;
        }

        public RowViewModel GetListingDetails(string listingIdx)
        {
            var proc = StoredProcedure.ListingMgmnt.GetListingDetails;

            XElement xmlIn = CommonXml.GetBaseArguments("GetList");
            xmlIn.Add(new XElement("Listing_Idx", listingIdx));

            var dbResponse = MvcWebServiceProxy.ParseXml(proc, xmlIn);

            var listingDetails = new RowViewModel(dbResponse);

            foreach (var record in listingDetails.Records)
                foreach (var dropdownProperty in record.Properties.Where(prop => prop.ControlType.ToLower() == "dropdown"))
                    record.InitialDropdownLoad(dropdownProperty);

            return listingDetails;
        }

        public SingleSelectViewModel GetListingCustomerDropdown(string listingIdx)
        {
            var proc = StoredProcedure.ListingMgmnt.GetListingCustomers;

            XElement xmlIn = CommonXml.GetBaseArguments("DataSourceInput");
            xmlIn.Add(new XElement("Listing_Idx", listingIdx));

            var dbResponse = MvcWebServiceProxy.ParseXml(proc, xmlIn);

            var listingCustomerDropdown = new SingleSelectViewModel(dbResponse, true);
            return listingCustomerDropdown;
        }

        public TreeViewHierarchy GetListingProductsRootNode(string listingIdx)
        {
            var proc = StoredProcedure.ListingMgmnt.GetListingProducts;

            XElement xmlIn = CommonXml.GetBaseArguments("Products");
            xmlIn.Add(new XElement("Listing_Idx", listingIdx));
            xmlIn.Add(new XElement("Screen_Code", "LISTINGSMGMT"));

            var dbResponse = MvcWebServiceProxy.ParseXml(proc, xmlIn);

            var productsRootNode = new TreeViewHierarchy(dbResponse);
            return productsRootNode;
        }

        public Message RemoveListing(string listingIdx)
        {
            var proc = StoredProcedure.ListingMgmnt.DeleteListing;

            XElement xmlIn = CommonXml.GetBaseArguments("DeleteListing");
            xmlIn.Add(
                new XElement("Listings",
                    new XElement("Listing_Idx", listingIdx)
                    ));

            var dbResponse = MvcWebServiceProxy.ParseXml(proc, xmlIn);

            var message = Message.New(dbResponse);
            return message;
        }

        public Message SaveListing(Listing listing)
        {
            var proc = StoredProcedure.ListingMgmnt.SaveListing;

            XElement xmlIn = CommonXml.GetBaseArguments("SaveData");
            xmlIn.Add(new XElement("Cust_Idx", listing.Customers.SelectedItem.Idx));

            var selectedProduct = TreeViewHierarchy.GetFlatTree(listing.ProductsRoot).Single(node => node.IsSelectedBool == true);
            if (selectedProduct != null)
                xmlIn.Add(new XElement("Sku_Idx", selectedProduct.Idx));

            xmlIn.Add(new XElement("Details", listing.Details.ToCoreXml().Root));

            var dbResponse = MvcWebServiceProxy.ParseXml(proc, xmlIn);

            var message = Message.New(dbResponse);
            return message;
        }
    }
}