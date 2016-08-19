using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Exceedra.Controls.Helpers.Controls
{
 public class ProportionallyStretchingPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
                child.Measure(availableSize);
            return availableSize;
        }

        protected override Size ArrangeOverride(Size availableSize)
        {
            double widthSum = 0.0;
            foreach (UIElement child in InternalChildren)
            {
                widthSum += child.DesiredSize.Width;
            }
            double x = 0.0;
            foreach (UIElement child in InternalChildren)
            {
                double proportionalWidth = child.DesiredSize.Width / widthSum * availableSize.Width;
                child.Arrange(
                    new Rect(
                        new Point(x, 0.0),
                        new Point(x + proportionalWidth, availableSize.Height)));
                x += proportionalWidth;
            }
            return availableSize;
        }
    }

}
