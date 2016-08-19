using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.Listings;

namespace Model.DataAccess.Epos
{
   public class EposAccess
    {
        //epos.SP_MAPPING_GetUnmappedProducts
        //<GetData>
        //  <User_Idx>1</User_Idx>
        //</GetData>

        #region "linking products"
        public static Task<TreeViewHierarchy> GetUnmappedProducts(bool forceReload = false, bool standAloneReload = false)
        {
            /* Do not use another proc here */
            var proc = StoredProcedure.EPOS.Mapping.GetUnmappedProducts;
            var args = CommonXml.GetBaseArguments("GetData");

            var cacheItem = DynamicDataAccess.GetCacheString(proc);

            if (XmlCache.Contains(cacheItem) && !forceReload && FlatUnmatchedList != null && !standAloneReload)
            {
                FlatUnmatchedList.Do(p => p.IsSelectedBool = false);
                return Task.FromResult(TreeViewHierarchy.ConvertListToTree(FlatUnmatchedList));
            }

            return DynamicDataAccess.GetGenericItemAsync<TreeViewHierarchy>(proc, args, forceReload, cacheItem).ContinueWith(t =>
            {
                if (!standAloneReload)
                    FlatUnmatchedList = t.Result.FlatTree.ToList();
                return t.Result;
            });
        }
        private static List<TreeViewHierarchy> FlatUnmatchedList { get; set; }


        //epos.SP_MAPPING_GetProducts
        //Input:

        //  <GetData>
        //    <User_Idx>1</User_Idx>
        //  </GetData>

        //Output:

        //<Results>
        //  <Item>
        //    <Idx>0</Idx>
        //    <Name>ALL</Name>
        //    <IsSelected>0</IsSelected>
        //  </Item>
        //  <Item>
        //    <Idx>1</Idx>
        //    <Name>Mixers</Name>
        //    <IsSelected>0</IsSelected>
        //    <ParentIdx>0</ParentIdx>
        //  </Item>
        //  <Item>
        //    <Idx>2</Idx>
        //    <Name>Super Fizz</Name>
        //    <IsSelected>0</IsSelected>
        //    <ParentIdx>0</ParentIdx>
        //  </Item>

        //  etc…

        //</Results>

        public static Task<TreeViewHierarchy> GetSpProducts(bool forceReload = false, bool standAloneReload = false)
        {
            /* Do not use another proc here */
            var proc = StoredProcedure.EPOS.Mapping.GetProducts;
            var args = CommonXml.GetBaseArguments("GetData");

            var cacheItem = DynamicDataAccess.GetCacheString(proc);

            if (XmlCache.Contains(cacheItem) && !forceReload && FlatProductList != null && !standAloneReload)
            {
                FlatProductList.Do(p => p.IsSelectedBool = false);
                return Task.FromResult(TreeViewHierarchy.ConvertListToTree(FlatProductList));
            }

            return DynamicDataAccess.GetGenericItemAsync<TreeViewHierarchy>(proc, args, forceReload, cacheItem).ContinueWith(t =>
            {
                if (!standAloneReload)
                    FlatProductList = t.Result.FlatTree.ToList();
                return t.Result;
            });
        }
        private static List<TreeViewHierarchy> FlatProductList { get; set; }


        //epos.SP_MAPPING_CreateMapping
        //Input:

        //  <SaveData>
        //    <UserIdx>1</UserIdx>
        //    <UnMappedIdx>5</UnMappedIdx>
        //    <SpProductIdx>60000</SpProductIdx>
        //  </SaveData>

        //Output:

        //<Results>
        //  <SuccessMessage>Mapping successful</SuccessMessage>
        //</Results>

       public static bool CreateMapping(string eposIDx, string ssIDx)
       {
           var proc = StoredProcedure.EPOS.Mapping.CreateMapping;
            var args = CommonXml.GetBaseArguments("SaveData");

            args.Add(new XElement("UnMapped_Idx", eposIDx));
            args.Add(new XElement("SpProduct_Idx", ssIDx));

            return  MessageConverter.DisplayMessage(WebServiceProxy.Call(proc, args, DisplayErrors.No));
       }

        #endregion

       public static Task<TreeViewHierarchy> GetPossibleMatched(string idx, bool forceReload = false, bool standAloneReload = false)
       {

            /* Do not use another proc here */
            var proc = StoredProcedure.EPOS.Mapping.GetMappingSuggestions;
            var args = CommonXml.GetBaseArguments("GetData");
            args.Add(new XElement("UnMapped_Idx", idx));

            var cacheItem = DynamicDataAccess.GetCacheString(proc);

            if (XmlCache.Contains(cacheItem) && !forceReload && FlatUnmatchedList != null && !standAloneReload)
            {
                FlatUnmatchedList.Do(p => p.IsSelectedBool = false);
                return Task.FromResult(TreeViewHierarchy.ConvertListToTree(FlatUnmatchedList));
            }

            return DynamicDataAccess.GetGenericItemAsync<TreeViewHierarchy>(proc, args, forceReload, cacheItem).ContinueWith(t =>
            {
                if (!standAloneReload)
                    FlatUnmatchedList = t.Result.FlatTree.ToList();
                return t.Result;
            });
        }
    }
}

