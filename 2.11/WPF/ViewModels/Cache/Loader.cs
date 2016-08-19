using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Model.Annotations;
using Model.DataAccess;
using Model.DataAccess.Listings;
using Model.Entity;
using WPF.UserControls.Listings;
using WPF.ViewModels.Scenarios;

namespace WPF.ViewModels.Cache
{
    /// <summary>
    /// Static calls to load each cache in App.Cache
    /// </summary>
    public static class Loader
    {

        internal static void LoadAll(List<Screen> Screens)
        {
            //default we need them         
            SalesOrgCache();

            //everyone needs listings
            ListingsCache();             
        }
         
        public static void SalesOrgCache()
        {

            try
            {
                var ps = App.AppCache.GetItem("SalesOrganisations");

                if (ps == null)
                {
                    var res = LoginAccess.GetSalesOrgs();
                    var salesOrgs = res.OrderBy(s => s.SortIndex).ToList();
                    var salesOrgViewModels = salesOrgs.Select(s => new SalesOrgDataViewModel(s, s.SortIndex == 0)).ToList();


                    App.AppCache.Upsert("SalesOrganisations", salesOrgViewModels);
                }

            }
            catch (Exception)
            {


            }
        }
 
        public static void ListingsCache()
        {
            var listingsAccess = new ListingsAccess();
            listingsAccess.ResetListingsData();
        }
         
        /// <summary>
        /// Add any cache items that need to be cleared when the sales Org is changed by the user
        /// </summary>
        public static void ClearSalesOrgRelatedCache()
        {
            App.CachedListingsViewModels.Clear();
            App.AppCache.Remove(string.Format("{0}_`", "Schedule"));

            foreach (var thisScreen in App.Configuration.ROBScreens)
            {
                App.AppCache.Remove(string.Format("ROBs_{0}_Customers", thisScreen.RobAppType));
            }


        }
         
    }
     
}
