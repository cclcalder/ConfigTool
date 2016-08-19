using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace WPF.Pages.Canvas
{
    public class GridRows : FrameworkElement
    {
        #region NoGridRows

        public static int GetNoGridRows(DependencyObject obj)
        {
            return (int)obj.GetValue(NoGridRowsProperty);
        }

        public static void SetNoGridRows(DependencyObject obj, int value)
        {
            obj.SetValue(NoGridRowsProperty, value);
        }

        public static readonly DependencyProperty NoGridRowsProperty =
            DependencyProperty.RegisterAttached("NoGridRows",
            typeof(int), typeof(GridRows), new PropertyMetadata
            (1, NoGridRowsChanged));

        private static void NoGridRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = d as Grid;
            int newNoRows = (int)e.NewValue;

            RecreateRowDefinitions(grid, newNoRows);

            RegisteredGrids.Add(grid);

            // attaching timer used for handling multiple resizing events
            GridResizeTimer.Tick += ResizeGrid;
        }

        #endregion

        #region RenderedRowsContainer

        public static Grid GetRenderedRowsContainer(DependencyObject obj)
        {
            return (Grid)obj.GetValue(RenderedRowsContainerProperty);
        }

        public static void SetRenderedRowsContainer(DependencyObject obj, Grid value)
        {
            obj.SetValue(RenderedRowsContainerProperty, value);
        }

        /// <summary>
        /// An indirect parent of the grid with rows
        /// that is rendered before them
        /// so it's possible to set the height for every cell beforehand
        /// </summary>
        public static readonly DependencyProperty RenderedRowsContainerProperty =
            DependencyProperty.RegisterAttached("RenderedRowsContainer",
            typeof(Grid), typeof(GridRows), new PropertyMetadata
            (null, RenderedRowsContainerChanged));

        private static void RenderedRowsContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var renderedRowsContainer = d as FrameworkElement;

            if (renderedRowsContainer != null)
                GetRenderedRowsContainer(d).SizeChanged += ContainerSizeChanged;
        }

        #endregion

        /// <summary>
        /// Removes all the row definitions and creates them again
        /// </summary>
        /// <param name="grid">Grid to recreate the row definitions to</param>
        /// <param name="newNoRows">Number of new row definitions</param>
        /// <param name="rowHeight">Height of every new row</param>
        private static void RecreateRowDefinitions(Grid grid, int newNoRows, double rowHeight = 0)
        {
            grid.RowDefinitions.Clear();

            for (int i = 0; i < newNoRows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(rowHeight);

                grid.RowDefinitions.Add(row);
            }
        }

        /// <summary>
        /// Fires up every time when the rows container size is changed.
        /// Enables the timer (so it has to reach its interval at least once) 
        /// and resets it (stops and starts) so if it reaches its interval resizes the grid.
        /// </summary>
        /// <param name="sender">Rows container</param>
        /// <param name="e">Size changed details</param>
        private static void ContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridResizeTimer.IsEnabled = true;
            GridResizeTimer.Stop();
            GridResizeTimer.Start();

            gridToResize = sender as Grid;
            newGridHeight = e.NewSize.Height;
        }

        /// <summary>
        /// Disables the timer and recreates the rows definitions
        /// with rows of height equal to 1/10 of the container height
        /// (but not less than 50 pixels)
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private static void ResizeGrid(object sender, EventArgs s)
        {
            GridResizeTimer.IsEnabled = false;

            var singleCellHeight = (newGridHeight - 15) / 10;
            if (singleCellHeight < 50) singleCellHeight = 50;

            // finding every grid that is a child of the rows container and recreating its rows definitions
            var childGrids = RegisteredGrids.Where(cg => cg.GetParents().Contains(gridToResize));
            foreach (var childGrid in childGrids)
                RecreateRowDefinitions(childGrid, childGrid.RowDefinitions.Count, singleCellHeight);
        }

        #region fields

        /// <summary>
        /// Indirect children grids of the rows container.
        /// Stored to be found easily, because in handlers
        /// (when resizing the grids and calculating a single row height)
        /// we have access only to the rows container
        /// </summary>
        private static readonly List<Grid> RegisteredGrids = new List<Grid>();

        /// <summary>
        /// When the rows container is being resized (its size changed event occurs)
        /// it waits 0.3s and if no other size changed event occurs
        /// eventually resizes the grid.
        /// Used to prevent from multiple grid resizing.
        /// </summary>
        private static readonly DispatcherTimer GridResizeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 300), IsEnabled = false };

        // Just to remember the args sent from the ContainerSizeChanged method
        // to the ResizeGrid method
        private static Grid gridToResize;
        private static double newGridHeight;

        #endregion
    }
}
