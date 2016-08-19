using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.UserControls.Listings;
using WPF.ViewModels.PromotionPowerEditor;


namespace WPF.Pages.PromoPowerEditor
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1
    {
        public Page1() : this(null)
        {
            App.Navigator.EnableNavigation(false);
            InitializeComponent();
        }

        private readonly PromotionPowerEditorViewModel _viewModel;

        public Page1(PromotionPowerEditorViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            App.Navigator.EnableNavigation(false);
            InitializeComponent();
            ListingsUserControl.CustomerSingleSelect = true;
            //ListingsUserControl.CustomersTree.IsTriStateMode = false;
            //ListingsUserControl.CustomersTree.SelectionMode = SelectionMode.Single;
            //ListingsUserControl.CustomersTree.IsOptionElementsEnabled = true;
            //ListingsUserControl.CustomersTree.IsOptionElementsEnabled = false;
            //ListingsUserControl.CustomersTree.UpdateLayout();

            DataContext = _viewModel = viewModel;

            //_viewModel.PropertyChanged += ViewModelPropertyChanged;
        }

        private void PromoPowerPage2TabItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedPageIdx == 1) return;

            if(_viewModel.GoToPage2Command.CanExecute(null))
                _viewModel.GoToPage2Command.Execute(null);
        }

        //private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "PAndLRecordVM")
        //    {
        //        _viewModel.PAndLRowSum();
        //    }
        //}

        private void PromoPowerPage1TabItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.SelectedPageIdx = 0;
        }

        //private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{
        //    //ListingsUserControl = new ListingsUserControl();

        //    //ListingsUserControl.DataContext = _viewModel.ListingsVM;
        //    //ListingsUserControl.InitializeComponent();

        //    //ListingsUserControl.CustomersTree.IsTriStateMode = false;
        //    //ListingsUserControl.CustomersTree.SelectionMode = SelectionMode.Single;

        //    //ListingsUserControl.CustomersIsSingleSelection = true;

        //    //ListingsUserControl.ReLoad();
        //}
    }
}
