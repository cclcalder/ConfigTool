using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exceedra.Common;

namespace Model.DataAccess
{
    using System.Diagnostics;
    using System.Xml.Linq;
    using Entity;

    public interface IPricingAccess
    {
        IEnumerable<ItemCustomer> GetCustomers();
        IEnumerable<Item> GetItems();
        IEnumerable<Item> GetItemsProduct();
        IEnumerable<Scenario> GetScenarios();
        IEnumerable<PricingProduct> GetProducts(ItemCustomer itemCustomer);
        IEnumerable<ItemDetail> GetItemDetails(ItemCustomer selectedCustomer, Scenario selectedScenario, Item selectedItem, IEnumerable<string> productIds);
        WebServiceResult ValidateItemDetails(IEnumerable<ItemDetail> itemDetails, ItemCustomer customer,Scenario scenario, Item item);
        WebServiceResult SaveItemDetails(IEnumerable<ItemDetail> itemDetails, ItemCustomer customer, Scenario scenario, Item item);
        IEnumerable<PricingProduct> GetProducts(string customerId);
    }

    public class PricingAccess : IPricingAccess
    {
        private readonly ConcurrentDictionary<string,IList<PricingProduct>> _productCache = new ConcurrentDictionary<string, IList<PricingProduct>>(); 
        public IEnumerable<ItemCustomer> GetCustomers()
        {
            string arguments = "<GetItemsCustomers><UserID>{0}</UserID></GetItemsCustomers>".FormatWith(User.CurrentUser.ID);
            return WebServiceProxy.Call(StoredProcedure.GetItemCustomers, XElement.Parse(arguments))
                .Elements()
                .Select(xml => new ItemCustomer(xml));
        }

        public IEnumerable<Item> GetItems()
        {
            string arguments = "<GetItems><UserID>{0}</UserID></GetItems>".FormatWith(User.CurrentUser.ID);
            return WebServiceProxy.Call(StoredProcedure.GetItems, XElement.Parse(arguments))
                .Elements()
                .Select(xml => new Item(xml));
        }

        public IEnumerable<Item> GetItemsProduct()
        {
            string arguments = "<GetItems><UserID>{0}</UserID></GetItems>".FormatWith(User.CurrentUser.ID);
            return WebServiceProxy.Call(StoredProcedure.GetItemsProduct, XElement.Parse(arguments))
                .Elements()
                .Select(xml => new Item(xml));
        }

        public IEnumerable<Scenario> GetScenarios()
        {
            string arguments = "<GetItemScenarios><UserID>{0}</UserID></GetItemScenarios>".FormatWith(User.CurrentUser.ID);

            var scenarioNodes = WebServiceProxy.Call(StoredProcedure.GetItemScenarios, XElement.Parse(arguments)).Elements();

            return from s in scenarioNodes.Elements("Scenario")
                   select new Scenario(s.GetValue<string>("ID"), s.GetValue<string>("Name"),s.Element("IsSelected").MaybeValue() == "1");
        }

        /// <summary>
        /// Extracts and creates product list returned from web service XML data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PricingProduct> GetProducts(ItemCustomer itemCustomer)
        {
            var customerId = itemCustomer != null ? itemCustomer.ID : string.Empty;
            return GetProducts(customerId);
        }

        /// <summary>
        /// Extracts and creates product list returned from web service XML data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PricingProduct> GetProducts(string customerId)
        {
            return _productCache.GetOrAdd(customerId, GetProductsImpl);
        }

        private IList<PricingProduct> GetProductsImpl(string customerId)
        {
            string arguments = "<GetItemProducts><UserID>{0}</UserID><CustomerID>{1}</CustomerID></GetItemProducts>"
                .FormatWith(User.CurrentUser.ID, customerId);

            try
            {
                return WebServiceProxy.Call(StoredProcedure.GetItemProducts, XElement.Parse(arguments),
                                            DisplayErrors.No)
                    .Elements()
                    .Select(xml => new PricingProduct(xml, this) { CustomerId = customerId})
                    .ToList();
            }
            catch (ExceedraDataException ex)
            {
                if (ex.Message == WebServiceProxy.RequestedActionYieldedNoResult)
                {
                    return new[] {new PricingProduct(this) {DisplayName = "[No products found]", Children = Enumerable.Empty<PricingProduct>()}};
                }
                else
                {
                    throw;
                }
            }
        }

        public IEnumerable<ItemDetail> GetItemDetails(ItemCustomer customer, Scenario scenario, Item item, IEnumerable<string> productIds)
        {
            if (scenario == null) throw new ArgumentNullException("scenario");
            if (item == null) throw new ArgumentNullException("item");

            string proc;
            string xml;

            if (customer == null)
            {
                proc = StoredProcedure.GetItemDetailsProduct;
                xml =
                    "<GetItemDetails><UserID>{0}</UserID><ScenarioID>{1}</ScenarioID><ItemTypeID>{2}</ItemTypeID></GetItemDetails>"
                        .FormatWith(User.CurrentUser.ID, scenario.ID, item.ID);
            }
            else
            {
                proc = StoredProcedure.GetItemDetails;
                xml =
                    "<GetItemDetails><UserID>{0}</UserID><CustomerID>{1}</CustomerID><ScenarioID>{2}</ScenarioID><ItemTypeID>{3}</ItemTypeID></GetItemDetails>"
                        .FormatWith(User.CurrentUser.ID, customer.ID, scenario.ID, item.ID);
            }
            var argument = XElement.Parse(xml);
            var productsNode = new XElement("Products");
            argument.Add(productsNode);
            foreach (var productId in productIds.Distinct())
            {
                productsNode.Add(new XElement("ProductID", productId));
            }

            return WebServiceProxy.Call(proc, argument)
                .MaybeElement("Items")
                .Elements("Item")
                .Select(x => new ItemDetail(x));
        }

        public WebServiceResult ValidateItemDetails(IEnumerable<ItemDetail> itemDetails, ItemCustomer customer, Scenario scenario, Item item)
        {
            string proc;
            var argument = new XElement("SavePricingData");
            if (customer != null)
            {
                proc = StoredProcedure.ValidatePricingData;
                argument.Add(new XElement("CustomerID", customer.ID));
            }
            else
            {
                proc = StoredProcedure.ValidatePricingDataProduct;
            }
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("ScenarioID", scenario.ID));
            argument.Add(new XElement("ItemID", item.ID));

            var rows = new XElement("ItemDetails");
            argument.Add(rows);

            foreach (var itemDetail in itemDetails.Where(i => i.HasChanges))
            {
                var row = new XElement("ItemDetail");
                row.Add(new XElement("ProductID", itemDetail.ProductId));
                row.Add(new XElement("OriginalFromDate", itemDetail.OriginalFromDate.ToString("yyyy-MM-dd")));
                row.Add(new XElement("FromDate", itemDetail.FromDate.ToString("yyyy-MM-dd")));
                row.Add(new XElement("OriginalToDate", itemDetail.OriginalToDate.ToString("yyyy-MM-dd")));
                row.Add(new XElement("ToDate", itemDetail.ToDate.ToString("yyyy-MM-dd")));
                row.Add(new XElement("OriginalValue", itemDetail.OriginalPrice));
                row.Add(new XElement("ModifiedValue", itemDetail.ModifiedPrice));
                rows.Add(row);
            }

            try
            {
                return WebServiceResult.FromXml(WebServiceProxy.Call(proc, argument,DisplayErrors.No));
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return new WebServiceError(ex.Message);
            }
        }

        public WebServiceResult SaveItemDetails(IEnumerable<ItemDetail> itemDetails, ItemCustomer customer, Scenario scenario, Item item)
        {
            string proc;
            var argument = new XElement("SavePricingData");
            if (customer != null)
            {
                proc = StoredProcedure.SavePricingData;
                argument.Add(new XElement("CustomerID", customer.ID));
            }
            else
            {
                proc = StoredProcedure.SavePricingDataProduct;
            }
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("ScenarioID", scenario.ID));
            argument.Add(new XElement("ItemID", item.ID));

            var rows = new XElement("ItemDetails");
            argument.Add(rows);

            foreach (var itemDetail in itemDetails.Where(i => i.HasChanges))
            {
                var row = new XElement("ItemDetail");
                row.Add(new XElement("ProductID", itemDetail.ProductId));
                row.Add(new XElement("OriginalFromDate", itemDetail.OriginalFromDate.ToString("yyyy-MM-dd")));
                row.Add(new XElement("FromDate", itemDetail.FromDate.ToString("yyyy-MM-dd")));
                row.Add(new XElement("OriginalToDate", itemDetail.OriginalToDate.ToString("yyyy-MM-dd")));
                row.Add(new XElement("ToDate", itemDetail.ToDate.ToString("yyyy-MM-dd")));
                row.Add(new XElement("OriginalValue", itemDetail.OriginalPrice));
                row.Add(new XElement("ModifiedValue", itemDetail.ModifiedPrice));
                rows.Add(row);
            }

            try
            {
                return WebServiceResult.FromXml(WebServiceProxy.Call(proc, argument, DisplayErrors.No));
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return new WebServiceError(ex.Message);
            }
        }
    }
}
