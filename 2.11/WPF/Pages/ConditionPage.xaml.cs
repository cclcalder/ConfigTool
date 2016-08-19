using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPF.ViewModels.Conditions;
using System;
 
namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for ConditionPage.xaml
    /// </summary>
    public partial class ConditionPage
    {
        private ConditionDetailsViewModel _viewModel;

        public ConditionPage() : this("0") { }

        public ConditionPage(string conditionId)
        {
            InitializeComponent();
            DataContext = _viewModel = new ConditionDetailsViewModel(conditionId);
            UploadFile.Load("CONDITION", _viewModel.ConditionId, App.Configuration.StorageDetails);
        }


        private void OnCustomerSelectionChanged(object sender, EventArgs e)
        {
            _viewModel.UpdateProductSelectedForCustomers();
        }
     
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = (DataGridCell)sender;

            // Perform only for Checkbox Column types
            if (cell.Column is DataGridCheckBoxColumn)
            {
                if (!cell.IsEditing)
                {
                    if (!cell.IsFocused)
                    {
                        cell.Focus();
                    }
                    var dataGrid = FindVisualParent<DataGrid>(cell);
                    if (dataGrid != null)
                    {
                        if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                        {
                            if (!cell.IsSelected)
                                cell.IsSelected = true;
                        }
                        else
                        {
                            var row = FindVisualParent<DataGridRow>(cell);
                            if (row != null && !row.IsSelected)
                            {
                                row.IsSelected = true;
                            }
                        }
                    }
                }
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

        private void ConditionMeasures_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header != null)
            {
                const string newCondition = "New Condition";
                if (e.Column.Header.Equals(newCondition))
                    ((ConditionMeasureViewModel)e.Row.Item).HasChanged = true;
            }
        }

        private void ConditionMeasureSelectAll_Clicked(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeSelected();
        }

    }
}
