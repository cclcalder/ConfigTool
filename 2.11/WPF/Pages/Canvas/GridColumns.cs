using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace WPF.Pages.Canvas
{
    public class GridColumns : FrameworkElement
    {
        #region NoGridColumns

        public static int GetNoGridColumns(DependencyObject obj)
        {
            return (int)obj.GetValue(NoGridColumnsProperty);
        }

        public static void SetNoGridColumns(DependencyObject obj, int value)
        {
            obj.SetValue(NoGridColumnsProperty, value);
        }

        public static readonly DependencyProperty NoGridColumnsProperty =
            DependencyProperty.RegisterAttached("NoGridColumns",
            typeof(int), typeof(GridColumns), new PropertyMetadata
            (1, NoGridColumnsChanged));

        private static void NoGridColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = d as Grid;
            int newNoColumns = (int)e.NewValue;

            RecreateColumnDefinitions(grid, newNoColumns);

            RegisteredGrids.Add(grid);

            // attaching timer used for handling multiple resizing events
            GridResizeTimer.Tick += ResizeGrid;
        }

        #endregion

        #region RenderedColumnsContainer

        public static Grid GetRenderedColumnsContainer(DependencyObject obj)
        {
            return (Grid)obj.GetValue(RenderedColumnsContainerProperty);
        }

        public static void SetRenderedColumnsContainer(DependencyObject obj, Grid value)
        {
            obj.SetValue(RenderedColumnsContainerProperty, value);
        }

        /// <summary>
        /// An indirect parent of the grid with columns
        /// that is rendered before them
        /// so it's possible to set the width for every cell beforehand
        /// </summary>
        public static readonly DependencyProperty RenderedColumnsContainerProperty =
            DependencyProperty.RegisterAttached("RenderedColumnsContainer",
            typeof(Grid), typeof(GridColumns), new PropertyMetadata
            (null, RenderedColumnsContainerChanged));

        private static void RenderedColumnsContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var renderedColumnsContainer = d as FrameworkElement;

            if (renderedColumnsContainer != null)
                GetRenderedColumnsContainer(d).SizeChanged += ContainerSizeChanged;
        }

        #endregion

        /// <summary>
        /// Removes all the column definitions and creates them again
        /// </summary>
        /// <param name="grid">Grid to recreate the column definitions to</param>
        /// <param name="newNoColumns">Number of new column definitions</param>
        /// <param name="columnWidth">Width of every new column</param>
        private static void RecreateColumnDefinitions(Grid grid, int newNoColumns, double columnWidth = 0)
        {
            grid.ColumnDefinitions.Clear();

            for (int i = 0; i < newNoColumns; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(columnWidth);

                grid.ColumnDefinitions.Add(column);
            }
        }

        /// <summary>
        /// Fires up every time when the columns container size is changed.
        /// Enables the timer (so it has to reach its interval at least once) 
        /// and resets it (stops and starts) so if it reaches its interval resizes the grid.
        /// </summary>
        /// <param name="sender">Columns container</param>
        /// <param name="e">Size changed details</param>
        private static void ContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridResizeTimer.IsEnabled = true;
            GridResizeTimer.Stop();
            GridResizeTimer.Start();

            gridToResize = sender as Grid;
            newGridWidth = e.NewSize.Width;
        }

        /// <summary>
        /// Disables the timer and recreates the columns definitions
        /// with columns of width equal to 1/10 of the container width
        /// (but not less than 80 pixels)
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private static void ResizeGrid(object sender, EventArgs e)
        {
            GridResizeTimer.IsEnabled = false;

            var singleCellWidth = (newGridWidth - 15) / 10;
            if (singleCellWidth < 80) singleCellWidth = 80;

            // finding every grid that is a child of the columns container and recreating its columns definitions
            var childGrids = RegisteredGrids.Where(cg => cg.GetParents().Contains(gridToResize));
            foreach (var childGrid in childGrids)
                RecreateColumnDefinitions(childGrid, childGrid.ColumnDefinitions.Count, singleCellWidth);
        }

        #region fields

        /// <summary>
        /// Indirect children grids of the columns container.
        /// Stored to be found easily, because in handlers
        /// (when resizing the grids and calculating a single column width)
        /// we have access only to the columns container
        /// </summary>
        private static readonly List<Grid> RegisteredGrids = new List<Grid>();

        /// <summary>
        /// When the columns container is being resized (its size changed event occurs)
        /// it waits 0.3s and if no other size changed event occurs
        /// eventually resizes the grid.
        /// Used to prevent from multiple grid resizing.
        /// </summary>
        private static readonly DispatcherTimer GridResizeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 300), IsEnabled = false };

        // Just to remember the args sent from the ContainerSizeChanged method
        // to the ResizeGrid method
        private static Grid gridToResize;
        private static double newGridWidth;

        #endregion
    }
}
