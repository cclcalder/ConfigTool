using System;
using System.Windows;
using System.Windows.Controls; 
using WPF.ViewModels;

namespace WPF.Wizard
{
    /// <summary>
    /// Interaction logic for WizardFrame.xaml
    /// </summary>
    public partial class WizardFrame : Page
    { 
        //private static PromotionWizardViewModel _viewModel { get; set; }

        public WizardFrame()
        {
            InitializeComponent();
            App.WizardNavigator = this;
        }

        private static string _param;
         public WizardFrame(string parameter)
        {
            InitializeComponent();
            App.WizardNavigator = this;
            Loaded += Page_Loaded;

             _param = parameter;
        }

         private void Page_Loaded(object sender, RoutedEventArgs e)
         {
             if (_param != "")
             {
                 // _viewModel = new PromotionWizardViewModel(null, parameter);
                 //   DataContext = _viewModel;
                 //frmMain.Navigate();

                 var p = new PromotionWizardViewModel(null, _param);


             } 
         }

        public void NavigateTo(Page view)
        {
            frmMain.Navigate(view);
        }

        public void NavigateTo(Page view, Action navigated)
        {
            frmMain.Navigate(view, navigated);
        }



      
    }
}
