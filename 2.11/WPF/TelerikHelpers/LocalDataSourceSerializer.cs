using Model.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Model.Entity.Listings;
using Telerik.Pivot.Adomd;


namespace WPF.TelerikHelpers
{
    public static class LinqExtensions
    {
        //public static IEnumerable<T> FlattenMe<T>(
        //              this IEnumerable<T> e,
        //              Func<T, IEnumerable<T>> f)
        //{
        //    return e.SelectMany(c => f(c).FlattenMe(f)).Concat(e);
        //}


        public static IEnumerable<TreeViewHierarchy> FlattenTree(this IEnumerable<TreeViewHierarchy> e)
        {
            return e.SelectMany(c => c.Children.FlattenTree()).Concat(e);
        }
    }
        public class AdomdProviderSerializer : DataProviderSerializer
        {
            public override IEnumerable<Type> KnownTypes
            {
                get
                {
                    return AdomdPivotSerializationHelper.KnownTypes;
                }
            }
        }
 


}
