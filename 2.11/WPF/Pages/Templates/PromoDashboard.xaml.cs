using System.Linq;
using System.Windows;

namespace WPF.Wizard
{
    using System;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class PromoDashboard : Page
    {
        private PromotionWizardViewModelBase _viewModel;
        public PromoDashboard(PromotionWizardViewModelBase viewModel)
        {
            App.LogError(Environment.NewLine + "PromoDashboard start load @" + DateTime.Now);
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = viewModel;
            Loaded += Page_Loaded;
            _viewModel = viewModel;
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
            
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DashboardRVM")
            {
                DashboardData.IsReadOnly = _viewModel.IsReadOnlyPromo;
                DashboardData.Visibility = (_viewModel.DashboardRVM != null && _viewModel.DashboardRVM.Records.Any()
                    ? Visibility.Visible
                    : Visibility.Hidden);
            }
 
        }


        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            App.LogError("PromoDashboard loaded @" + DateTime.Now);
        }
    }
}