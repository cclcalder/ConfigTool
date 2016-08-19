using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Exceedra.Common.Utilities
{
    public static class ObservableCollectionHelper
    {
    //    public static void Sort<T>(this ObservableCollection<T> collection,
    //IComparable<T> comparable)
    //    {
    //        if (comparable == null)
    //        {
    //            throw new ArgumentNullException("comparable is null");
    //        }
    //        Array.Sort<T>(collection, comparable);
    //    }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            ObservableCollection<T> collection = new ObservableCollection<T>();
            foreach (T item in source)
            {
                collection.Add(item);
            }
            return collection;
        }

        public static void SortBy<TSource, TKey>(this ObservableCollection<TSource> collection, Func<TSource, TKey> keySelector)
        {
            var sorted = collection.OrderBy(keySelector).ToList();

            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }
}
