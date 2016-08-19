using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Exceedra.Common
{
    public static class DataGridExtensions
    {
        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null && column > 0)
            {
                var presenter = Extensions.GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = Extensions.GetVisualChild<DataGridCellsPresenter>(row);
                }

                var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public static IEnumerable<DataGridRow> GetRows(this DataGrid grid)
        {
            if (grid == null || grid.Items == null) yield break;
            int count = grid.ItemsSource == null
                            ? grid.Items.Count
                            : grid.ItemsSource.Cast<object>().Count();

            for (int i = 0; i < count; i++)
            {
                yield return grid.GetRow(i);
            }
        }

        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            if (grid == null) return null;
            if (index < 0) return null;

            if (grid.Items.Count == 0) return null;

            var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
    }
}
