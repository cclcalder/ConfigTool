using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Exceedra.Common
{
    public static class Extensions
    {
        public static T GetValueOrDefault<T>(this XElement element, string path)
        {
            var value = element.GetValue<T>(path);

            if (value == null)
            {
                if (typeof(T).IsEquivalentTo(typeof(string)))
                {
                    value = (T)(object)"";
                    return value;
                }

                return default(T);
            }

            return value;
        }

        public static IEnumerable<T> TraversalAsEnumerable<T>(T start, Func<T, T> traversalFunc) where T : class
        {
            while (!ReferenceEquals(start, null))
            {
                yield return start;
                start = traversalFunc(start);
            }
        }

        public static IEnumerable<T> Flatten<T>(this T t, Func<T, IEnumerable<T>> mapFunc, bool includeSelf = true) where T : class
        {
            if (!ReferenceEquals(t, null))
            {
                foreach (T t2 in mapFunc(t))
                {
                    foreach (T t3 in t2.Flatten(mapFunc))
                    {
                        yield return t3;
                    }
                }
                if (includeSelf) yield return t;
            }
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T,IEnumerable<T>> map)
            where T : class
        {
            if (source == null)
                return null;

            return source.SelectMany(item => item.Flatten(map));
        }

        public static TResult Convert<T, TResult>(this T source, Converter<T, TResult> converter)
        {
            return converter(source);
        }

        public static IEnumerable<T> Run<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source.ToArray())
            {
                action(item);
                yield return item;
            }
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static T GetVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
    }
}
