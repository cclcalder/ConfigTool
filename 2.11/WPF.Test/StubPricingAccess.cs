namespace WPF.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using Model.DataAccess;
    using Model.Entity;

    class StubPricingAccess : IPricingAccess
    {
        private static readonly List<ItemCustomer> ItemCustomers = new List<ItemCustomer>
                                                               {
                                                                   new ItemCustomer("1", "Tesco"),
                                                                   new ItemCustomer("2", "Sainsbury"),
                                                                   new ItemCustomer("3", "ASDA")
                                                               };

        private static readonly List<Item> Items = new List<Item>
                                                       {
                                                           new Item("1", "Teapot"),
                                                           new Item("2", "Towel"),
                                                           new Item("42", "The HitchHiker's Guide to the Galaxy")
                                                       };

        private static readonly List<Scenario> Scenarios = new List<Scenario>
                                                               {
                                                                   new Scenario("1", "One"),
                                                                   new Scenario("2", "Two"),
                                                                   new Scenario("3", "Three")
                                                               };

        private static readonly Dictionary<string, List<PricingProduct>> Products =
            new Dictionary<string, List<PricingProduct>>
                {
                    {
                        "1", new List<PricingProduct>
                                 {
                                     new PricingProduct(null) { ID = "1", DisplayName = "Product 1.1" },
                                     new PricingProduct(null) { ID = "2", DisplayName = "Product 1.2" },
                                     new PricingProduct(null) { ID = "3", DisplayName = "Product 1.3" }
                                 }
                    },
                    {
                        "2", new List<PricingProduct>
                                 {
                                     new PricingProduct(null) { ID = "4", DisplayName = "Product 2.1" },
                                     new PricingProduct(null) { ID = "5", DisplayName = "Product 2.2" },
                                     new PricingProduct(null) { ID = "6", DisplayName = "Product 2.3" }
                                 }
                    },
                    {
                        "3", new List<PricingProduct>
                                 {
                                     new PricingProduct(null) { ID = "7", DisplayName = "Product 3.1" },
                                     new PricingProduct(null) { ID = "8", DisplayName = "Product 3.2" },
                                     new PricingProduct(null) { ID = "9", DisplayName = "Product 3.3" }
                                 }
                    },
                };

        public IEnumerable<ItemCustomer> GetCustomers()
        {
            return GetItemCustomersImpl();
        }

        public IEnumerable<Item> GetItems()
        {
            return Items.AsEnumerable();
        }

        public IEnumerable<Item> GetItemsProduct()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Scenario> GetScenarios()
        {
            return Scenarios.AsEnumerable();
        }

        public IEnumerable<PricingProduct> GetProducts(ItemCustomer itemCustomer)
        {
            if (itemCustomer != null)
                return Products[itemCustomer.ID].AsEnumerable();
            return Products.SelectMany(kvp => kvp.Value);
        }

        public IEnumerable<PricingProduct> GetProducts(string customerId)
        {
            if (!string.IsNullOrWhiteSpace(customerId))
                return Products[customerId].AsEnumerable();
            return Products.SelectMany(kvp => kvp.Value);
        }

        public IEnumerable<ItemDetail> GetItemDetails(ItemCustomer selectedItemCustomer, Scenario selectedScenario, Item selectedItem, IEnumerable<string> productIds)
        {
            return productIds.Select(pid => new ItemDetail(pid, pid, pid, DateTime.Now.Date, DateTime.Now.AddMonths(1).Date, 1m, "http://www.google.co.uk/"));
        }

        public bool ValidateItemDetailsWasCalled { get; private set; }
        public WebServiceResult ValidateItemDetails(IEnumerable<ItemDetail> itemDetails, ItemCustomer customer, Scenario scenario, Item item)
        {
            ValidateItemDetailsWasCalled = true;
            return new WebServiceSuccess("Success");
        }

        public bool SaveItemDetailsWasCalled { get; private set; }
        public WebServiceResult SaveItemDetails(IEnumerable<ItemDetail> itemDetails, ItemCustomer customer, Scenario scenario, Item item)
        {
            SaveItemDetailsWasCalled = true;
            return new WebServiceSuccess("Success");
        }

        private static IEnumerable<ItemCustomer> GetItemCustomersImpl()
        {
            return ItemCustomers.AsEnumerable();
        }
    }
}