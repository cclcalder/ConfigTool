using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPF.Navigation;
using WPF.ViewModels.Funds;

namespace WPF.Pages
{
    using Exceedra.Controls.DynamicGrid.Models;

    /// <summary>
    /// Interaction logic for EventsPage.xaml
    /// </summary>
    public partial class FundsList
    {
        private const string FilterWatermark = "Filter...";
        private readonly FundListViewModel _viewModel;

        public FundsList()
        {
            InitializeComponent();
            DataContext = _viewModel = FundListViewModel.New();

            //FundsgrGridControl.HyperLinkHandler = GenericLinkHandler;
            //ParentFundsgrGridControl.HyperLinkHandler = GenericLinkHandler;
            ProductDataModalPresenter.Visibility = Visibility.Hidden;
            
        }


        public void GenericLinkHandler(object sender, RoutedEventArgs e)
        {
            Record obj = ((FrameworkElement)sender).DataContext as Record;

            var path = obj.Item_Type.ToLower();
            var pathID = obj.Item_Idx;

            var p = false;
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                p = true;
            }

            RedirectMe.Goto(path, pathID, "", "", "", p);
        }

        public void GenericLinkHandlerChild(object sender, RoutedEventArgs e)
        {
            Record obj = ((FrameworkElement)sender).DataContext as Record;

            var path = obj.Item_Type.ToLower();
            try
            {
                var p = false;
                if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                {
                    p = true;
                }

                var pathID = obj.Properties.Single(t => t.ColumnCode == "Fund_Idx").Value;
                RedirectMe.Goto(path, pathID, "", "", "", p);
            }
            catch { }

        }



        private List<string> _initallyExpandedDeatilsList = new List<string>();
        private void ParentFundsDetailGrid_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.ContainsAll("DetailsVisibility,"))
            {
                var idx = e.PropertyName.Split(',').Last();

                if (_initallyExpandedDeatilsList == null) _initallyExpandedDeatilsList = new List<string>();

                if (!_initallyExpandedDeatilsList.Contains(idx))
                {
                    _initallyExpandedDeatilsList.Add(idx);
                }
                else
                {
                    _initallyExpandedDeatilsList.Remove(idx);
                }

                _viewModel.InitallyExpandedDeatilsList = _initallyExpandedDeatilsList;
            }
        }

    }
}
