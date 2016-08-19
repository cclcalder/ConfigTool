using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;


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

    /// <summary>
    /// Interaction logic for Financials.xaml
    /// </summary>
    public partial class Financials : Page
    {
        public Financials()
            : this(null)
        {
        }

        private PromotionWizardViewModelBase _viewModel;

        public Financials(PromotionWizardViewModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");

            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;

            _viewModel.PropertyChanged += ViewModelPropertyChanged;

            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

           
            //dynamicPromoMeasures.Header = App.CurrentLang.GetValue("PromoFinancials_PromotionMeasuresGrid",
            //  "Promotion Measures");

            dynamicParentProductMeasures.Header = App.CurrentLang.GetValue("PromoFinancials_ParentProductGrid",
                "Parent Products");

            dynamicProductMeasures.Header = App.CurrentLang.GetValue("PromoFinancials_PlanningProductGrid",
                "Planning Products");

            dynamicPromoMeasures.Visibility = (_viewModel.G1PromoFinancialMeasures != null && _viewModel.G1PromoFinancialMeasures.Records != null && _viewModel.G1PromoFinancialMeasures.Records.Count() > 0 ? Visibility.Visible : Visibility.Collapsed);
            dynamicParentProductMeasures.Visibility = (_viewModel.G2ParentProductFinancialMeasures != null && _viewModel.G2ParentProductFinancialMeasures.Records != null && _viewModel.G2ParentProductFinancialMeasures.Records.Count() > 0 ? Visibility.Visible : Visibility.Collapsed);
            dynamicProductMeasures.Visibility = (_viewModel.G3FinancialScreenPlanningSkuMeasure != null && _viewModel.G3FinancialScreenPlanningSkuMeasure.Records != null && _viewModel.G3FinancialScreenPlanningSkuMeasure.Records.Count() > 0 ? Visibility.Visible : Visibility.Collapsed);

        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "G1PromoFinancialMeasures")
            {
                //dynamicPromoMeasures.IsReadOnlyMode = _viewModel.IsReadOnlyPromo;

                dynamicPromoMeasures.Visibility = (_viewModel.G1PromoFinancialMeasures != null && _viewModel.G1PromoFinancialMeasures.Records != null && _viewModel.G1PromoFinancialMeasures.Records.Count() > 0 ? Visibility.Visible : Visibility.Collapsed);
            }

            if (e.PropertyName == "G2ParentProductFinancialMeasures")
            {
                dynamicParentProductMeasures.IsReadOnly = _viewModel.IsReadOnlyPromo;

                dynamicParentProductMeasures.Visibility = (_viewModel.G2ParentProductFinancialMeasures != null && _viewModel.G2ParentProductFinancialMeasures.Records != null && _viewModel.G2ParentProductFinancialMeasures.Records.Count() > 0 ? Visibility.Visible : Visibility.Collapsed);
            }

            if (e.PropertyName == "G3FinancialScreenPlanningSkuMeasure")
            {
                dynamicProductMeasures.IsReadOnly = _viewModel.IsReadOnlyPromo;

                dynamicProductMeasures.Visibility = (_viewModel.G3FinancialScreenPlanningSkuMeasure != null && _viewModel.G3FinancialScreenPlanningSkuMeasure.Records != null && _viewModel.G3FinancialScreenPlanningSkuMeasure.Records.Count() > 0 ? Visibility.Visible : Visibility.Collapsed);
            }
        }

      

        //private void dgProducts_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DataGrid dg = sender as DataGrid;
        //    Border border = VisualTreeHelper.GetChild(dg, 0) as Border;
        //    ScrollViewer scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
        //    Grid grid = VisualTreeHelper.GetChild(scrollViewer, 0) as Grid;
        //    Button button = VisualTreeHelper.GetChild(grid, 0) as Button;

        //    if (button != null && button.Command != null && button.Command == DataGrid.SelectAllCommand)
        //    {
        //        button.Click += new RoutedEventHandler(ProductHeaderSort_Click);
        //    }

          


        //}

        //private void ProductHeaderSort_Click(object sender, RoutedEventArgs e)
        //{
        //    //var indexOfLowestHeaderColumn = dgProducts.Columns.First(ind => ind.Header.Equals(dgProducts.Columns.Min(x => x.Header.ToString()))).DisplayIndex;
        //    //var indexOfHighestHeaderColumn = dgProducts.Columns.First(ind => ind.Header.Equals(dgProducts.Columns.Max(x => x.Header.ToString()))).DisplayIndex;
        //    //List<PromotionMeasure> orderedMeasures;
        //    //if (indexOfLowestHeaderColumn == 1 && indexOfHighestHeaderColumn == dgProducts.Columns.Count - 1) //already order ascending
        //    //{
        //    //    orderedMeasures = new List<PromotionMeasure>((dgProducts.ItemsSource as ObservableCollection<PromotionProductPrice>).First().Measures.ToList().OrderByDescending("Name"));
        //    //}
        //    //else
        //    //{
        //    //    orderedMeasures = new List<PromotionMeasure>((dgProducts.ItemsSource as ObservableCollection<PromotionProductPrice>).First().Measures.ToList().OrderBy("Name"));
        //    //}
        //    //int intOrder = 1;
        //    //foreach (var promotionMeasure in orderedMeasures)
        //    //{
        //    //    dgProducts.Columns.First(x => x.Header.Equals(promotionMeasure.Name)).DisplayIndex = intOrder;
        //    //    intOrder++;
        //    //}
        //}

    }
}