using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.MultiSelectCombo.Controls;
using Telerik.Windows.Controls;

namespace Exceedra.Controls.DynamicRow.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DynamicRowControl : INotifyPropertyChanged
    {
        /* You don't ever need to put extra code in getters and setters for depen props! 
         * Dependency Props bound will bypass the getters and setters - they are just required for compiling - EW */
        public RowViewModel ItemDataSource
        {
            get { return (RowViewModel)GetValue(ItemDataSourceProperty); }
            set { SetValue(ItemDataSourceProperty, value); }            
        }
        
        public DynamicRowControl()
        {
            InitializeComponent();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty ItemDataSourceProperty =
            DependencyProperty.Register("ItemDataSource", typeof(RowViewModel),
                                           typeof(DynamicRowControl),
                                            new FrameworkPropertyMetadata()
                                            {
                                                BindsTwoWayByDefault = true
                                            });

        private void LoadDependentDropdowns(RowProperty row)
        {
            if (row != null)
            {
                ItemDataSource.Records[0].LoadDependentDrops(row);
            }
        }

        // Commented out because we listen for changes in RowRecord in ControlChanged
        // (so it would be basically doing the same thing twice)
        //private void OnSingleComboboxChange(object sender, RoutedEventArgs e)
        //{
        //    var t = VisualTreeHelper.GetParent((RadComboBox)sender);
        //    while (t.GetType() != typeof(ContentControl))
        //    {
        //        t = VisualTreeHelper.GetParent(t);
        //    }
        //    if (((ContentControl) t).Content.GetType() != typeof (RowProperty))
        //        return;

        //    LoadDependentDropdowns((RowProperty)((ContentControl)t).Content);
        //}

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox cb = (TextBox)sender;
                var obj = ((FrameworkElement)sender).DataContext as RowProperty;

                obj.Value = cb.Text;

                var drg = FindVisualParent<DynamicRowControl>(this);

                if (drg != null)
                {
                    var items = drg.ItemDataSource;
                    items.CalulateRecordColumns();
                }
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox cb = (TextBox)sender;
            var obj = ((FrameworkElement)sender).DataContext as RowProperty;

            obj.Value = cb.Text;

            var drg = FindVisualParent<DynamicRowControl>(this);

            if (drg != null)
            {
                drg.ItemDataSource.CalulateRecordColumns();
            }
        }

        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        private int _callCount = 0;
        private void MultiSelectComboBox_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var t = VisualTreeHelper.GetParent((MultiSelectComboBox)sender);
            while (t.GetType() != typeof(ContentControl))
            {
                t = VisualTreeHelper.GetParent(t);
            }
            var row = (RowProperty) ((ContentControl) t).Content;
            if (row != null && (row.Values.Any() || row.Value != null))
            {
                if (((RowProperty)((ContentControl)t).Content).ControlType.ToLower().Contains("multi"))
                {
                    //if (_callCount == 0)
                    LoadDependentDropdowns((RowProperty)((ContentControl)t).Content);

                    //_callCount = _callCount + 1;

                    //if (_callCount == row.SelectedItems.Count)
                    //{
                    //    _callCount = 0;
                    //}
                }
                else
                {
                    _callCount = 0;
                }
            }
        }

        private void dynamicgrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            AlignLabelsForEachRow(sender);
        }

        private static void AlignLabelsForEachRow(object sender)
        {
            var itemsControl = sender as ItemsControl;
            if (itemsControl == null) return;

            var labelsGrids = itemsControl.ChildrenOfType<Grid>().Where(grid => grid.Name == "HeaderTextGrid").ToList();
            if (labelsGrids.Any())
            {
                var widestLabelGrid = labelsGrids.Max(labelGrid => labelGrid.ActualWidth);

                foreach (var labelGrid in labelsGrids)
                    labelGrid.Width = widestLabelGrid;
            }
        }
    }
}  