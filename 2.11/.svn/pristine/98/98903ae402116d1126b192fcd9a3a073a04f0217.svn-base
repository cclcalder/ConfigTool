using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Exceedra.Controls.DynamicGrid.Models;
using Telerik.Windows.Controls;
using ViewModels;
using WPF.Navigation;

namespace WPF.Pages
{
   
    public partial class FundPage
    {
        private readonly FundViewModel _viewModel;

        public FundPage()
        {
            InitializeComponent();
            DataContext = _viewModel = new FundViewModel("");
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
            Loaded += OnLoaded;

            TransferGrid.HyperLinkHandler = GenericLinkHandler;            
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _viewModel.IsLoading = false;
            UploadFile.Load("FUND", _viewModel.FundIdx, App.Configuration.StorageDetails);
        }

        public FundPage(string id)
        {
            InitializeComponent();
            DataContext = _viewModel = new FundViewModel(id, false);
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
            Loaded += OnLoaded;

            TransferGrid.HyperLinkHandler = GenericLinkHandler;
        }

        public FundPage(string id, bool isParentFund = false)
        {
            InitializeComponent();
            DataContext = _viewModel = new FundViewModel(id, isParentFund);
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
            Loaded += OnLoaded;

            TransferGrid.HyperLinkHandler = GenericLinkHandler;
        }

        private void GenericLinkHandler(object sender, RoutedEventArgs e)
        {
            // find the control that has been clicked
            var control = e.OriginalSource as Button;

            // we also need the record (row) that the control sits in
            Record obj = ((FrameworkElement)sender).DataContext as Record;

            // we also need the current column the control is in - we need the column header to use as the filter filter 
            if (control == null) return;
            var selectedColumn = control.Tag.ToString();

            if (obj == null) return;

            var path = obj.Item_Type.ToLower();
            var column = obj.Properties.SingleOrDefault(a => a.ColumnCode == selectedColumn);

            var p = false;
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                p = true;
            }
           


            switch (selectedColumn)
             {
                case "To_Fund_Name":
                    //To_Fund_Idx

                    try
                    {
                        var pathID = obj.Properties.SingleOrDefault(t => t.ColumnCode == "To_Fund_Idx").Value;
                        RedirectMe.Goto(path, pathID,"","","",p);
                    }
                    catch { }
                    
                    break;
                case "From_Fund_Name":
                    //From_Fund_Idx

                    try
                    {
                        var pathID = obj.Properties.SingleOrDefault(t => t.ColumnCode == "From_Fund_Idx").Value;
                        RedirectMe.Goto(path, pathID, "", "", "", p);
                    }
                    catch { }

                    break;
               }

         
            

        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LinkedEvents" || e.PropertyName == "Values")
            {
                _viewModel.CheckAndUpdateSummary();
            }

            // to reload FundIdx
            if (e.PropertyName == nameof(_viewModel.FundIdx))
                UploadFile.Load("FUND", _viewModel.FundIdx, App.Configuration.StorageDetails);
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (lstComments.SelectedItem != null)
            //    MessageBox.Show((lstComments.SelectedItem as PromotionComment).Value, "Comment details");
        }

        // private void OnComboWithCheckboxesLostFocus(object sender, RoutedEventArgs e)
        // {
        //   // _viewModel.UpdateSaveSelection();
        //     //Do stuff
        // }


        //private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (!((RadComboBoxItem) sender).IsEnabled)
        //        e.Handled = true;
        //}
    }
}
