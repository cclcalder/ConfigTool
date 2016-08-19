namespace Coder.WPF.UI
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;

    public static class UIElementExtensions
    {
        private static readonly ConcurrentDictionary<Type, Func<UIElement, object>> GetContentFuncs = new ConcurrentDictionary<Type, Func<UIElement, object>>();

        public static IEnumerable<UIElement> GetDescendants(this UIElement top, Type ignoreType)
        {
            var content = GetContentPropertyFunc(top.GetType())(top);
            if (content != null)
            {
                var contentList = (content as IEnumerable ?? Enumerable.Empty<object>()).OfType<UIElement>().Where(c => !ignoreType.IsInstanceOfType(c)).ToList();
                if (contentList.Count > 0)
                {
                    return contentList
                        .Concat(contentList.SelectMany(c => c.GetDescendants(ignoreType)));
                }

                var contentElement = content as UIElement;
                if (contentElement != null && !ignoreType.IsInstanceOfType(contentElement))
                {
                    return Return(contentElement).Concat(contentElement.GetDescendants(ignoreType));
                }
            }

            return Enumerable.Empty<UIElement>();
        }

        private static Func<UIElement, object> GetContentPropertyFunc(Type type)
        {
            return GetContentFuncs.GetOrAdd(type, GetContentPropertyFuncImpl);
        }

        private static Func<UIElement, object> GetContentPropertyFuncImpl(Type type)
        {
            //if (type == typeof(DataGrid))
            //{
            //    return element => ((DataGrid) element).Columns;
            //}
            var contentPropertyAttribute = Attribute.GetCustomAttribute(type, typeof(ContentPropertyAttribute),
                                                                        true) as ContentPropertyAttribute;
            if (contentPropertyAttribute == null)
            {
                return _ => null;
            }

            var contentPropertyInfo = type.GetProperty(contentPropertyAttribute.Name);

            if (contentPropertyInfo == null)
            {
                return _ => null;
            }

            return element => contentPropertyInfo.GetValue(element, null);
        }

        private static IEnumerable<T> Return<T>(T item)
        {
            yield return item;
        }
    }
}
