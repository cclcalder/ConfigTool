using System.Windows;
using System.Windows.Controls;
using Exceedra.Common.Xaml;

namespace Exceedra.CellsGrid
{
    public class GridColumns : FrameworkElement
    {
        #region NoColumns

        public static int GetNoColumns(DependencyObject obj)
        {
            return (int)obj.GetValue(NoColumnsProperty);
        }

        public static void SetNoColumns(DependencyObject obj, int value)
        {
            obj.SetValue(NoColumnsProperty, value);
        }

        public static readonly DependencyProperty NoColumnsProperty =
            DependencyProperty.RegisterAttached("NoColumns",
            typeof(int), typeof(GridColumns), new PropertyMetadata
            (1, NoColumnsChanged));

        private static void NoColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cellsGridPanel = d as Grid;
            if (cellsGridPanel == null) return;

            var cellsContainer = XamlHelper.FindParent<Grid>(cellsGridPanel, "CellsContainer");
            if (cellsContainer == null) return;

            int newNoColumns = (int)e.NewValue;

            if (cellsGridPanel.ActualWidth != 0)                                                 // grid is rendered already
            {
                if (cellsGridPanel.ColumnDefinitions.Count == newNoColumns) return;
                RecreateColumnDefinitions(cellsContainer, cellsGridPanel, newNoColumns);
            }
            else cellsContainer.Loaded += (sender, args) =>                                     // grid is not rendered yet
            {
                if (cellsGridPanel.ColumnDefinitions.Count == newNoColumns) return;
                RecreateColumnDefinitions(cellsContainer, cellsGridPanel, newNoColumns);
            };
        }

        #endregion

        #region CellsContainer

        public static Grid GetCellsContainer(DependencyObject obj)
        {
            return (Grid)obj.GetValue(CellsContainerProperty);
        }

        public static void SetCellsContainer(DependencyObject obj, Grid value)
        {
            obj.SetValue(CellsContainerProperty, value);
        }

        /// <summary>
        /// An indirect parent of the grid with columns
        /// that is rendered before them
        /// so it's possible to set the width for every cell beforehand
        /// </summary>
        public static readonly DependencyProperty CellsContainerProperty =
            DependencyProperty.RegisterAttached("CellsContainer",
            typeof(Grid), typeof(GridColumns), new PropertyMetadata
            (null, CellsContainerChanged));

        private static void CellsContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var renderedColumnsContainer = d as FrameworkElement;

            if (renderedColumnsContainer != null)
                GetCellsContainer(d).SizeChanged += CellsContainerSizeChanged;
        }

        #endregion

        #region methods

        private static double lastWidth;

        /// <summary>
        /// Fires up every time when the columns container size is changed.
        /// Enables the timer (so it has to reach its interval at least once) 
        /// and resets it (stops and starts) so if it reaches its interval resizes the grid.
        /// </summary>
        /// <param name="sender">Columns container</param>
        /// <param name="e">Size changed details</param>
        private static void CellsContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var cellsContainer = sender as Grid;
            if (cellsContainer == null) return;

            if (cellsContainer.ActualWidth == lastWidth)
                return;

            lastWidth = cellsContainer.ActualWidth;

            var cellsGridItemsControl = XamlHelper.FindChild<ItemsControl>(cellsContainer, "CellsGridItemsControl");
            if (cellsGridItemsControl == null) return;

            var cellsGridPanel = XamlHelper.FindChild<Grid>(cellsGridItemsControl, string.Empty);
            if (cellsGridPanel == null) return;

            RecreateColumnDefinitions(cellsContainer, cellsGridPanel, cellsGridPanel.ColumnDefinitions.Count);
        }

        /// <summary>
        /// Removes all the column definitions and creates them again
        /// </summary>
        /// <param name="cellsContainer">Grid to recreate the column definitions to</param>
        /// <param name="newNoColumns">Number of new column definitions</param>
        /// <param name="columnWidth">Width of every new column</param>
        private static void RecreateColumnDefinitions(Grid cellsContainer, Grid cellsGridPanel, int newNoColumns)
        {
            var singleCellWidth = (cellsContainer.ActualWidth - 15) / 10;
            if (singleCellWidth < 80) singleCellWidth = 80;

            cellsGridPanel.ColumnDefinitions.Clear();

            for (int i = 0; i < newNoColumns; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(singleCellWidth);

                cellsGridPanel.ColumnDefinitions.Add(column);
            }
        }

        #endregion
    }
}
