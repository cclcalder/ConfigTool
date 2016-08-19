using System.ComponentModel;
using Telerik.Windows;
using Telerik.Windows.Controls.TreeView;
using WPF.UserControls.Trees;

namespace WPF.Wizard
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Model;
    using ViewModels;
    using System.Windows.Media;
    using System.Windows.Input;
    using Telerik.Windows.Controls;
    using Exceedra.Controls;

    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : Page
    {
        private readonly int _baseColumnCount;

        public Products()
            : this(null)
        {
        }

        //private void ShowModalContent(object sender, RoutedEventArgs e)
        //{      
        //    ProductDataModalPresenter.IsModal = true;
        //}

        static bool showDisplay { get; set; }
        static bool showFOC { get; set; }
        private PromotionWizardViewModelBase _viewModel;
        public Products(PromotionWizardViewModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            _baseColumnCount = dgProducts.Columns.Count;
            DataContext = _viewModel = viewModel;
            showDisplay = viewModel.ShowDisplay;
            showFOC = viewModel.ShowFOC;
            Loaded += Products_Loaded;
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            
        }

        private void Products_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateGridColumns();
        }

        private void GenerateGridColumns()
        {
            IEnumerable items = dgProducts.ItemsSource;

            if (items == null) return;

            PromotionProductPrice prodPrice = (items as ObservableCollection<PromotionProductPrice>).FirstOrDefault();

            if (prodPrice != null && prodPrice.Measures != null && prodPrice.Measures.Any())
            {
                var measureColumns = prodPrice.Measures.ToList();
                var visibleMeasureColumns = measureColumns.Where(measure => measure.IsDisplayed).ToList();

                if (dgProducts.Columns.Count != visibleMeasureColumns.Count() + _baseColumnCount)
                {
                    var headersOfColumnsToRemove = prodPrice.Measures.Select(measure => measure.Name);
                    dgProducts.Columns.RemoveAll(o => headersOfColumnsToRemove.Contains(o.Header));

                    for (int i = 0; i < measureColumns.Count(); i++)
                    {
                        if (!measureColumns[i].IsDisplayed) continue;

                        var column = new DataGridTextColumn
                        {
                            // set index to be after Customer and Status Column.
                            DisplayIndex = i + 1,
                            Header = measureColumns[i].Name,
                            Binding = new Binding(string.Format("Measures[{0}].Value", i)),
                            MinWidth = 90,
                            IsReadOnly = true,
                        };

                        dgProducts.Columns.Add(column);
                    }
                }
            }
        }

        private void dgProducts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            IEnumerable source = dgProducts.ItemsSource;
            if (source == null)
                return;

            // comparing if the grid columns are not generated currently
            var items = (source as ObservableCollection<PromotionProductPrice>);
            if (items != null && items.Any())
            {
                if (items.First().Measures != null)
                {
                    var visibleMeasureColumns = items.First().Measures.Where(measure => measure.IsDisplayed);

                    if (dgProducts.Columns.Count != visibleMeasureColumns.Count() + _baseColumnCount)
                        GenerateGridColumns();
                }
            }
        }

        private void dgProducts_Loaded(object sender, RoutedEventArgs e)
        {
            var cols = dgProducts.Columns.Count();

            dgProducts.Columns[1].Visibility = (showFOC == false ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible);
            dgProducts.Columns[2].Visibility = (showDisplay == false ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible);
             
        }
    }
}