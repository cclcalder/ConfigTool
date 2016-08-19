using Exceedra.Controls.DynamicGrid.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Exceedra.SlimGrid.Converters
{
    public class CellToTooltipVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = (DataGridCell)value;

            if (cell == null) return Visibility.Collapsed;

            return ((Record)cell.DataContext).GetProperty((string)cell.Tag).HasComment ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
