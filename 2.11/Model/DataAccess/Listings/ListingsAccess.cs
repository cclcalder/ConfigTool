using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.Entity.Listings;

namespace Model.DataAccess.Listings
{
    public class ListingsAccess
    {
        public ListingsAccess(string appTypeId)
        {
            AppTypeId = appTypeId;
        }

        public ListingsAccess(string appTypeId, string robGroupId)
            : this(appTypeId)
        {
            RobGroupId = robGroupId;
        }

        public ListingsAccess()
        { }

        public string AppTypeId;
        public string RobGroupId;

        public void ResetListingsData()
        {
            PopulateListingsCache();
            PopulateGeneric();
        }

        #region Listings

        public void PopulateListingsCache()
        {
            GetListings(true);
        }

        public static Listing GetListings(bool resetCache = false)
        {
            var args = CommonXml.GetBaseArguments("Listings");

            return DynamicDataAccess.GetGenericItem<Listing>(StoredProcedure.Shared.GetListings, args, resetCache);
        }

        #endregion

        //#region Generic

        public void PopulateGeneric()
        {
            GetFilterProducts(true);
            GetFilterCustomers(true);
        }

        /* Testing new cache approach */
        /* StandAloneReload - When you want to load products but not effect the FlatProductList. 
         * This is needed in the rare case that you need two sku trees so cannot share the same model.
         */
        public static Task<TreeViewHierarchy> GetFilterProducts(bool forceReload = false, bool standAloneReload = false, Screen screen = null)
        {
            /* Do not use another proc here */
            var proc = StoredProcedure.Shared.GetFilterProducts;

            XElement args;
            string cacheItem;

            if (screen != null && screen.UseKeyToLoadData)
            {
                args = CommonXml.GetBaseArguments("Products", null, screen.Key);

                XElement xScreenKey = new XElement("ScreenKey", screen.Key);
                cacheItem = DynamicDataAccess.GetCacheString(proc, xScreenKey);
            }
            else
            {
                args = CommonXml.GetBaseArguments("Products");
                cacheItem = DynamicDataAccess.GetCacheString(proc);
            }

            if (XmlCache.Contains(cacheItem) && !forceReload && FlatProductList != null && !standAloneReload)
            {
                FlatProductList.Do(p => p.IsSelectedBool = false);
                return Task.FromResult(TreeViewHierarchy.ConvertListToTree(FlatProductList));
            }

            return DynamicDataAccess.GetGenericItemAsync<TreeViewHierarchy>(proc, args, forceReload, cacheItem).ContinueWith(t =>
            {
                if(!standAloneReload)
                    FlatProductList = t.Result.FlatTree.ToList();
                return t.Result;
            });
        }

        public static Task<TreeViewHierarchy> GetFilterCustomers(bool forceReload = false, bool standAloneReload = false, Screen screen = null)
        {
            /* Do not use another proc here */
            var proc = StoredProcedure.Shared.GetFilterCustomers;

            XElement args;
            string cacheItem;

            if (screen != null && screen.UseKeyToLoadData)
            {
                args = CommonXml.GetBaseArguments("GetCustomerTree", null, screen.Key);

                XElement xScreenKey = new XElement("ScreenKey", screen.Key);
                cacheItem = DynamicDataAccess.GetCacheString(proc, xScreenKey);
            }
            else
            {
                args = CommonXml.GetBaseArguments("GetCustomerTree");
                cacheItem = DynamicDataAccess.GetCacheString(proc);
            }

            if (XmlCache.Contains(cacheItem) && !forceReload && FlatCustomerList != null && !standAloneReload)
            {
                FlatCustomerList.Do(p => p.IsSelectedBool = false);
                return Task.FromResult(TreeViewHierarchy.ConvertListToTree(FlatCustomerList));
            }

            return DynamicDataAccess.GetGenericItemAsync<TreeViewHierarchy>(proc, args, forceReload, cacheItem).ContinueWith(t =>
            { 
                if(!standAloneReload && t.Result.FlatTree != null)
                    FlatCustomerList = t.Result.FlatTree.ToList();
                return t.Result;
            });
        }

        /* Instead of getting from an xml cache, use the below flat trees to skip any need to deserialize */
        private static List<TreeViewHierarchy> FlatProductList { get; set; }
        private static List<TreeViewHierarchy> FlatCustomerList { get; set; }
         
        public static void ClearListingsCache()
        {
            FlatProductList = null;
            FlatCustomerList = null;
        }
    }

}
