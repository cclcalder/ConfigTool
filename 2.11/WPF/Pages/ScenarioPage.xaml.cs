using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPF.ViewModels.Scenarios;
 

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for Scenarios.xaml
    /// </summary>
    public partial class ScenarioPage
    {

        private readonly ScenarioDetailsViewModel _scenarioDetails;

        public ScenarioPage() : this("0") { }

        public ScenarioPage(string scenarioIndex)
        {
            InitializeComponent();
            CurrentScenarioIndex = scenarioIndex;
            DataContext = _scenarioDetails = new ScenarioDetailsViewModel(CurrentScenarioIndex);
            _scenarioDetails.PropertyChanged += ViewModelPropertyChanged;
            //UCCustomers.Selected += new RoutedEventHandler(OnCustomerSelectionChanged);
            UploadFile.Load("SCENARIO", _scenarioDetails.ScenarioId.ToString(), App.Configuration.StorageDetails);
        }

        public string CurrentScenarioIndex { get; set; }

        private void OnCustomerSelectionChanged(object sender, EventArgs e)
        {
            _scenarioDetails.UpdateProductSelectedForCustomers(); 
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;

            // Perform only for Checkbox Column types
            if (cell.Column is DataGridCheckBoxColumn)
            {
                if (cell != null &&
                    !cell.IsEditing)
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

        private void dgFundingData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            _scenarioDetails.HasChanged = true;
            
        }


        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // to reload ScenarioId
            if (e.PropertyName == nameof(_scenarioDetails.ScenarioId))
                UploadFile.Load("SCENARIO", _scenarioDetails.ScenarioId.ToString(), App.Configuration.StorageDetails);
        }
    }

}
