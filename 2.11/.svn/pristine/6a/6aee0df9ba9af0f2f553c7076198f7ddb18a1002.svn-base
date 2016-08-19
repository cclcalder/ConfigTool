using Model.Entity.Diagnostics;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Exceedra.Controls.Caret
{
    public static class FilterCaret
    {
        public static void SetFilterIconAndSize(bool isOpen, RowDefinition rowFilter, int closeSize, int openSize)
        {
            if (isOpen)
            {
                OpenFilters(rowFilter, openSize);
            }
            else
            {
                CloseFilters(rowFilter,closeSize);
            }
        }

        private static void OpenFilters(RowDefinition rowFilter, int openSize)
        {
            rowFilter.Height = new GridLength(openSize);
        }

        private static void CloseFilters(RowDefinition rowFilter, int closeSize)
        {
            rowFilter.Height = new GridLength(closeSize);
        }

        private static string imageUrl = new SiteData().BaseURL;

        public static BitmapImage GetUpImage()
        {
            return new BitmapImage(new Uri(imageUrl + "/Images/caret/up.gif"));
        }

        public static BitmapImage GetDownImage()
        {
            return new BitmapImage(new Uri(imageUrl + "/Images/caret/down.gif"));
        }



    }
}
