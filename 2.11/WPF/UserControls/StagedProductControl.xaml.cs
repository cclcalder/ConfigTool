using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Model;
using Telerik.Windows.Controls;
using WPF.ViewModels;

namespace WPF.UserControls
{
    /// <summary>
    /// Interaction logic for WeeklyVolumeControl.xaml
    /// </summary>
    public partial class StagedProductControl : UserControl
    {
        private StagedProductViewModel _viewModel;

        public StagedProductControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as StagedProductViewModel;

            if (_viewModel != null)
            {
                InitializeGrid();
            }
        }

        private void InitializeGrid()
        {
            if (_viewModel == null) return;

            if (StagedGrid.Columns.Count > 1)
            {
                ResetGrid();
            }

            for (var index = 0; index < _viewModel.Stages.Count; index++)
            {
                var column = new GridViewDataColumn
                {
                    Header = _viewModel.Stages[index],
                    Width = 85,
                    DataMemberBinding =
                        new Binding(string.Format("Values[{0}].Value", index))
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                            //,Converter = FindResource("DecimalToString") as IValueConverter
                        },
                    IsReadOnlyBinding = new Binding(string.Format("Values[{0}]." + nameof(StagedMeasure.IsReadOnly), index))
                };

                StagedGrid.Columns.Add(column);
            }
        }

        private void ResetGrid()
        {
            for (int index = 1; index < StagedGrid.Columns.Count; index++)
            {
                StagedGrid.Columns.RemoveAt(index);
            }
        }
    }
}
