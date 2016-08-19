using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Exceedra.TreeGrid.Extensions
{
    public static class TreeGridExtensions
    {
        #region ScrollOnDragProperty

        public static readonly DependencyProperty ScrollOnDragProperty =
            DependencyProperty.RegisterAttached("ScrollOnDrag",
                typeof(bool),
                typeof(TreeGridExtensions),
                new PropertyMetadata(false, HandleScrollOnDragChanged));

        public static bool GetScrollOnDrag(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(ScrollOnDragProperty);
        }

        public static void SetScrollOnDrag(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ScrollOnDragProperty, value);
        }

        private static void HandleScrollOnDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement container = d as FrameworkElement;

            if (d == null)
            {
                Debug.Fail("Invalid type!");
                return;
            }

            Unsubscribe(container);

            if (true.Equals(e.NewValue))
            {
                Subscribe(container);
            }
        }

        private static void Subscribe(FrameworkElement container)
        {
            container.PreviewMouseMove += Container_MouseLeave;
        }


        private static void Container_MouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement container = sender as FrameworkElement;
            bool mouseIsDown = Mouse.LeftButton == MouseButtonState.Pressed;

            if (mouseIsDown)
            {
                ScrollViewer scrollViewer = GetFirstVisualChild<ScrollViewer>(container);

                if (scrollViewer == null)
                {
                    return;
                }

                double tolerance = 0;
                double horizontalPos = e.GetPosition(container).X;
                double offset = 10;

                if (horizontalPos < tolerance) //Left of TreeList? 
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - offset); //Scroll Left. 
                }
                else if (horizontalPos > container.ActualWidth - tolerance) //Right of TreeList? 
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + offset); //Scroll Right.     
                }


                double verticalPos = e.GetPosition(container).Y;

                if (verticalPos < tolerance) // Top of TreeList? 
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset); //Scroll up. 
                }
                else if (verticalPos > container.ActualHeight - tolerance) //Bottom of TreeList? 
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset); //Scroll down.     
                }
            }
        }

        private static void Unsubscribe(FrameworkElement container)
        {
            container.PreviewMouseMove -= Container_MouseLeave;
        }

        public static T GetFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = GetFirstVisualChild<T>(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
