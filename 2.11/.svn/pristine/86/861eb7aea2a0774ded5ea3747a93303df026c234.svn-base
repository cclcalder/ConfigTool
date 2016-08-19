using System.Windows.Automation;

namespace WPF.PromoTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Model;
    using Telerik.Windows.Controls;
    using ViewModels;
    using Telerik.Windows.Data;

    /// <summary>
    /// Interaction logic for Volumes.xaml
    /// </summary>
    public partial class Volumes : Page
    {
        private static readonly HashSet<string> PostPromoColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Post Promo Vol", "New Cons Vol"
        };
        public Volumes()
            : this(null)
        {
        }


        private static bool showRowTotal { get; set; }

        private PromotionWizardViewModelBase _viewModel;

        public Volumes(PromotionWizardViewModelBase viewModel)  
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
            Loaded += Page_Loaded;
            refresh = false;
            
           _viewModel.PropertyChanged += ViewModelPropertyChanged;
           dynamicVolumes.Visibility = System.Windows.Visibility.Collapsed;
           displayGrid.Visibility = System.Windows.Visibility.Collapsed;
           dynamicSteals.Visibility = System.Windows.Visibility.Collapsed;


           dynamicVolumes.CanShowResults = Visibility.Visible;
           dynamicDisplay.CanShowResults = Visibility.Hidden;
           dynamicSteals.CanShowResults = Visibility.Visible;

           dynamicVolumes.Header = "Product Volumes";
           dynamicDisplay.Header = "Parent Volumes";
           dynamicSteals.Header = "Cannibalisation Volumes";
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VolumesRVM")
            { 
                dynamicVolumes.IsReadOnly = _viewModel.IsReadOnlyPromo;
                 
                dynamicVolumes.Visibility = (_viewModel.VolumesRVM != null &&  _viewModel.VolumesRVM.Records != null && _viewModel.VolumesRVM.Records.Count() > 0 ? Visibility.Visible : Visibility.Collapsed);
            }

            if (e.PropertyName == "ShowUnitGrid")
            {                
                dynamicDisplay.IsReadOnly = _viewModel.IsReadOnlyPromo;
                
                displayGrid.Visibility = (_viewModel.ShowUnitGrid == true ? Visibility.Visible : Visibility.Collapsed);
            }

            if (e.PropertyName == "StealVolumesRVM")
            {
                dynamicSteals.IsReadOnly = _viewModel.IsReadOnlyPromo;

                dynamicSteals.Visibility = ((App.Configuration.IsCannibalisationActive && _viewModel.StealVolumesRVM != null &&
                    _viewModel.StealVolumesRVM.Records != null && _viewModel.StealVolumesRVM.Records.Count() > 0)) ? System.Windows.Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            dynamicVolumes.Visibility = (_viewModel.VolumesRVM != null && _viewModel.VolumesRVM.Records != null && _viewModel.VolumesRVM.Records.Count() > 0 ? Visibility.Visible : Visibility.Collapsed);
            displayGrid.Visibility = (_viewModel.ShowUnitGrid == true ? Visibility.Visible : Visibility.Collapsed);
            if (App.Configuration.IsCannibalisationActive && _viewModel.StealVolumesRVM != null && _viewModel.StealVolumesRVM.Records != null && _viewModel.StealVolumesRVM.Records.Count() > 0)
            {
                dynamicSteals.Visibility = System.Windows.Visibility.Visible;
            }      

        }

    

        private void dgProducts_Loaded(object sender, RoutedEventArgs e)
        {
           // this.dgProducts.CalculateAggregates();
        }

        private bool refresh;
   
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateAllowedDisplayunits();
            _viewModel.VolumesPage.State = ToggleState.Off;
        }

 
         

    }
}