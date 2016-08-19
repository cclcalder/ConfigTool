using System.Windows;
using System.Windows.Controls;
using Exceedra.Common.Xaml;

namespace Exceedra.CellsGrid
{
    public class GridRows : FrameworkElement
    {
        #region NoRows

        public static int GetNoRows(DependencyObject obj)
        {
            return (int)obj.GetValue(NoRowsProperty);
        }

        public static void SetNoRows(DependencyObject obj, int value)
        {
            obj.SetValue(NoRowsProperty, value);
        }

        public static readonly DependencyProperty NoRowsProperty =
            DependencyProperty.RegisterAttached("NoRows",
            typeof(int), typeof(GridRows), new PropertyMetadata
            (1, NoRowsChanged));

        private static void NoRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cellsGridPanel = d as Grid;
            if (cellsGridPanel == null) return;

            var cellsContainer = XamlHelper.FindParent<Grid>(cellsGridPanel, "CellsContainer");
            if (cellsContainer == null) return;

            int newNoRows = (int)e.NewValue;

            if (cellsGridPanel.ActualHeight != 0)                                           // grid is rendered already
            {
                if (cellsGridPanel.RowDefinitions.Count == newNoRows) return;
                RecreateRowDefinitions(cellsContainer, cellsGridPanel, newNoRows);
            }
            else cellsGridPanel.Loaded += (sender, args) =>                                 // grid is not rendered yet
            {
                if (cellsGridPanel.RowDefinitions.Count == newNoRows) return;
                RecreateRowDefinitions(cellsContainer, cellsGridPanel, newNoRows);
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
        /// An indirect parent of the grid with rows
        /// that is rendered before them
        /// so it's possible to set the height for every cell beforehand
        /// </summary>
        public static readonly DependencyProperty CellsContainerProperty =
            DependencyProperty.RegisterAttached("CellsContainer",
            typeof(Grid), typeof(GridRows), new PropertyMetadata
            (null, CellsContainerChanged));

        private static void CellsContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var renderedRowsContainer = d as FrameworkElement;

            if (renderedRowsContainer != null)
                GetCellsContainer(d).SizeChanged += CellsContainerSizeChanged;
        }

        #endregion

        #region methods

        private static double lastHeight;

        /// <summary>
        /// Fires up every time when the rows container size is changed.
        /// Enables the timer (so it has to reach its interval at least once) 
        /// and resets it (stops and starts) so if it reaches its interval resizes the grid.
        /// </summary>
        /// <param name="sender">Rows container</param>
        /// <param name="e">Size changed details</param>
        private static void CellsContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var cellsContainer = sender as Grid;
            if (cellsContainer == null) return;

            if (cellsContainer.ActualHeight == lastHeight)
                return;

            lastHeight = cellsContainer.ActualHeight;

            var cellsGridItemsControl = XamlHelper.FindChild<ItemsControl>(cellsContainer, "CellsGridItemsControl");
            if (cellsGridItemsControl == null) return;

            var cellsGridPanel = XamlHelper.FindChild<Grid>(cellsGridItemsControl, string.Empty);
            if (cellsGridPanel == null) return;

            RecreateRowDefinitions(cellsContainer, cellsGridPanel, cellsGridPanel.RowDefinitions.Count);
        }

        /// <summary>
        /// Removes all the row definitions and creates them again
        /// </summary>
        /// <param name="cellsGridPanel">Grid to recreate the row definitions to</param>
        /// <param name="newNoRows">Number of new row definitions</param>
        /// <param name="rowHeight">Height of every new row</param>
        private static void RecreateRowDefinitions(Grid cellsContainer, Grid cellsGridPanel, int newNoRows)
        {
            var singleCellHeight = (cellsContainer.ActualHeight - 15) / 10;
            if (singleCellHeight < 50) singleCellHeight = 50;

            cellsGridPanel.RowDefinitions.Clear();

            for (int i = 0; i < newNoRows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(singleCellHeight);

                cellsGridPanel.RowDefinitions.Add(row);
            }
        }

        #endregion
    }
}
