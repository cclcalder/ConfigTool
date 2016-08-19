using System;
using System.Collections.Generic;
using System.Linq;
using Model.Entity;
using System.Xml.Linq;

namespace Model.DataAccess.Converters
{
    public static class ListingConverter
    {
        //<Summary>
        //Returns the listings pair from an existing cust-sku selection
        //</Summary>
        public static IEnumerable<Tuple<string, string>> ToListingPairs(HashSet<string> custs, HashSet<string> skus, Listing listings)
        {
            var hashedCustList = listings.TheseListings.ToLookup(l => l.Key.Split('@')[0]);

            /* Remove non-selected custs */
            var hashedSelectedCustList = hashedCustList.Where(c => custs.Contains(c.Key)).SelectMany(k => hashedCustList[k.Key]).ToLookup(l => l.Key.Split('@')[0]);

            /* Get the new set of listings that only contain the selected customers */
            var applicableListings = hashedSelectedCustList.Select(c => c.Key).SelectMany(k => hashedSelectedCustList[k]).ToList();

            var hashedSkuList = applicableListings.ToLookup(l => l.Key.Split('@')[1]);

            /* Get the new set of listings that only contain the selected skus */
            //applicableListings = applicableListings.Where(c => skus.Contains(c.Value.ProductID)).SelectMany(k => hashedSkuList[k.Value.ProductID]).ToList();

            return applicableListings.Where(c => skus.Contains(c.Value.ProductID)).Select(listing => listing.Key.Split('@')).Select(s => new Tuple<string, string>(s[0], s[1]));
        }

        public static XElement ToListingXml(HashSet<string> custs, HashSet<string> skus, Listing listings, string rootTag = "Listings")
        {
            var activeListings = ToListingPairs(custs, skus, listings);

            var listingsXml = new XElement(rootTag);
            foreach (var listing in activeListings)
            {
                listingsXml.Add(new XElement("Listing", new XAttribute("Cust_Idx", listing.Item1),
                    new XAttribute("Sku_Idx", listing.Item2)));
            }

            return listingsXml;
        }
    }
}