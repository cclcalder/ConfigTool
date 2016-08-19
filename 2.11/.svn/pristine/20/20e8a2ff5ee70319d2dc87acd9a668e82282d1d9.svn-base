namespace WPF.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using ViewModels;

    /// <summary>
    /// Interaction logic for ManagePhasing.xaml
    /// </summary>
    public partial class ManagePhasing : UserControl
    {
        private ManagePhasingViewModel _viewModel;

        public ManagePhasing()
        {
            InitializeComponent();

            SetViewModel();
            InitializeGrid();

            DataContextChanged += OnDataContextChanged;
        }

        private void SetViewModel()
        {
            if (_viewModel != null)
            {
                _viewModel.SelectedProfileChanged -= ViewModelOnSelectedProfileChanged;
            }
            _viewModel = DataContext as ManagePhasingViewModel;
            if (_viewModel != null)
            {
                _viewModel.SelectedProfileChanged += ViewModelOnSelectedProfileChanged;
            }
        }

        private void ViewModelOnSelectedProfileChanged(object sender, EventArgs eventArgs)
        {
            if (_viewModel.SelectedProfile != null)
            {
                _viewModel.SelectedProfile.PropertyChanged -= SelectedProfileOnPropertyChanged;
                _viewModel.SelectedProfile.PropertyChanged += SelectedProfileOnPropertyChanged;
            }
            InitializeGrid();
        }

        private void SelectedProfileOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals("Size"))
            {
                InitializeGrid();
            }
        }

        private void OnDataContextChanged(object sender,
                                          DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            SetViewModel();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            if (_viewModel == null) return;

            if (ValueGrid.Columns.Count > 1)
            {
                ResetGrid();
            }

            if (_viewModel.SelectedProfile == null) return;

            string prefix = _viewModel.IsDaySelected ? "D" : "W";
            for (int i = 0; i < _viewModel.SelectedProfile.Size; i++)
            {
                var column = new DataGridTextColumn
                    {
                        Header = prefix + (i + 1),
                        Binding =
                            new Binding(string.Format("Values[{0}]", i))
                                {
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                    Converter = FindResource("DecimalToString") as IValueConverter
                                },
                        IsReadOnly = false,
                    };
                ValueGrid.Columns.Add(column);
            }
        }

        private void ResetGrid()
        {
            List<DataGridColumn> dynamicColumns = ValueGrid.Columns.Skip(1).ToList();
            foreach (DataGridColumn column in dynamicColumns)
            {
                ValueGrid.Columns.Remove(column);
            }
        }
    }
}