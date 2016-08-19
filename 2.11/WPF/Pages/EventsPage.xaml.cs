using System.Collections.Generic;
using System.Linq;

namespace WPF.Pages
{
    using global::ViewModels;

    /// <summary>
    /// Interaction logic for EventsPage.xaml
    /// </summary>
    public partial class EventsPage
    {
        public EventsPage()
            : this("0")
        {
        }

        public EventsPage(string appTypeId, List<string> initallyExpandedDeatilsList = null)
        {
            AppTypeID = appTypeId;
            //if (senderTab != null) _initalTab = senderTab;

            //if (initallyExpandedDeatilsList != null) _initallyExpandedDeatilsList = initallyExpandedDeatilsList;

            InitializeComponent();
            
            var screen = App.Configuration.GetScreens().FirstOrDefault(rs => rs.RobAppType == appTypeId) ;
            //MainPage.CurrentScreen = screen;
            string pageTitle = screen != null ? screen.Label : "ROB";

            DataContext = new EventsViewModel(appTypeId, pageTitle);
            //Loaded += PageLoaded;

            //if (!App.Configuration.IsPopupWindowsActive)
            //dynamicDisplay.HyperLinkHandler = GenericLinkHandler;

            //ListingsControl.DataContext = _viewModel.ListingsVM;
            //ListingsControl.ViewModel = ListingsControl.DataContext as ListingsViewModel;

        }

        //public void GenericLinkHandler(object sender, RoutedEventArgs e)
        //{
        //    Record rob = ((FrameworkElement)sender).DataContext as Record;
        //    if (rob == null) return;

        //    // take rob or rob group id property (whichever exists)
        //    var robIdProperty = rob.Properties.FirstOrDefault(prop => 
        //        prop.ColumnCode.ToLowerInvariant().Equals("rob_idx") 
        //        || prop.ColumnCode.ToLowerInvariant().Equals("robgroup_idx"));
        //    if (robIdProperty == null) return;

        //    // get the id value
        //    var robIdx = robIdProperty.Value;

        //    switch (MainPage.CurrentScreen.ShowAsROBGroup)
        //    {
        //        case true:
        //            RedirectMe.EntryPoint("groups", robIdx, _appTypeID);
        //            break;

        //        default:
        //            if(robIdProperty.ColumnCode.ToLowerInvariant().Equals("rob_idx"))
        //                _viewModel.EditRob(robIdx);
        //            else if (robIdProperty.ColumnCode.ToLowerInvariant().Equals("robgroup_idx"))
        //                _viewModel.EditContract(robIdx);
        //            break;
        //    }


        //}

        
        //public bool IsFiltered { get; set; }

        //public bool IsFilteredProducts { get; set; }

        //private RadTabItem _initalTab;
        //private List<string> _initallyExpandedDeatilsList = new List<string>();

        //private void SkuDetailGrid_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName.ContainsAll("DetailsVisibility,"))
        //    {
        //        var idx = e.PropertyName.Split(',').Last();

        //        if (_initallyExpandedDeatilsList == null) _initallyExpandedDeatilsList = new List<string>();

        //        if (!_initallyExpandedDeatilsList.Contains(idx))
        //        {
        //            _initallyExpandedDeatilsList.Add(idx);
        //        }
        //        else
        //        {
        //            _initallyExpandedDeatilsList.Remove(idx);
        //        }

        //        _viewModel.InitallyExpandedDeatilsList = _initallyExpandedDeatilsList;
        //    }
        //}

        public string AppTypeID { get; set; }
    }
}
