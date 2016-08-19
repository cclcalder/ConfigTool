using System.Net;

namespace WPF.PromoTemplates
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for PLReview.xaml
    /// </summary>
    public partial class TemplatePLReview : Page
    {
        public TemplatePLReview()
            : this(null)        
        {
        }
         private PromotionTemplateViewModelBase _viewModel;
         public TemplatePLReview(PromotionTemplateViewModelBase viewModel)
        {
          if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = viewModel;
            Loaded += Page_Loaded;
            _viewModel = viewModel;
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // App.LogError("PromoDashboard loaded @" + DateTime.Now);
            UploadFile.Load("PROMOTION$TEMPLATE", _viewModel.CurrentTemplate.Id, App.Configuration.StorageDetails);

            DashboardData.Header = App.CurrentLang.GetValue("Label_Template_Grid1", "Grid 1");
            DashboardData2.Header = App.CurrentLang.GetValue("Label_Template_Grid2", "Grid 2");
            //DashboardData3.Header = App.CurrentLang.GetValue("Label_Dashboard_Grid3", "Grid 3");
            //DashboardData4.Header = App.CurrentLang.GetValue("Label_Dashboard_Grid4", "Grid 4");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
        }


        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "DashboardRVM")
            //{
            //    DashboardData.IsReadOnlyMode = _viewModel.IsReadOnlyPromo;
            //    DashboardData.Visibility = (_viewModel.DashboardRVM != null && _viewModel.DashboardRVM.Records.Any()
            //        ? Visibility.Visible
            //        : Visibility.Hidden);
            //}

        }
 
        private void Frame_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            string temp = e.Exception.Message;
        }

        private void Frame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var request = e.WebRequest as HttpWebRequest;

            if (request != null)
            {
                request.CookieContainer = new CookieContainer();
                request.AllowAutoRedirect = false;
            }
        }

        public Visibility IsPromotionScenarioVisible
        {
            get { return App.Configuration.IsPromotionScenarioActive ? Visibility.Visible : Visibility.Collapsed; }
        }
    }
}