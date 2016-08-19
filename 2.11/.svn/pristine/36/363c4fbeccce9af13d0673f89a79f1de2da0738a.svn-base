using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Model.Entity.Listings;
using Telerik.Windows.Controls;

namespace WPF.UserControls.Trees
{
    public static class Extensions
    {
        public static List<RadTreeViewItem> FindTreeViewItems(this Visual @this)
        {
            if (@this == null)
                return null;

            var result = new List<RadTreeViewItem>();

            var frameworkElement = @this as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.ApplyTemplate();
            }

            Visual child = null;
            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(@this); i < count; i++)
            {
                child = VisualTreeHelper.GetChild(@this, i) as Visual;

                var treeViewItem = child as RadTreeViewItem;
                if (treeViewItem != null)
                {
                    result.Add(treeViewItem);
                    if (!treeViewItem.IsExpanded)
                    {
                        treeViewItem.IsExpanded = true;
                        treeViewItem.UpdateLayout();
                    }
                }
                foreach (var childTreeViewItem in FindTreeViewItems(child))
                {
                    result.Add(childTreeViewItem);
                }
            }
            return result;
        }

      
    }
}
