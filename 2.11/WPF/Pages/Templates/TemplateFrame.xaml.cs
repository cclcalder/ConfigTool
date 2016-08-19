using System;
using System.Windows;
using System.Windows.Controls;
using WPF.ViewModels.PromoTemplates;

namespace WPF.PromoTemplates
{
    /// <summary>
    /// Interaction logic for TemplateFrame.xaml
    /// </summary>
    public partial class TemplateFrame : Page
    { 
        // private static PromotionTemplateViewModel _viewModel { get; set; }

        public TemplateFrame()
        {
            InitializeComponent();
          App.TemplateNavigator = this;
        }

        private static string _param;
         public TemplateFrame(string parameter)
        {
            InitializeComponent();
            App.TemplateNavigator = this;
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

                 var p = new PromotionTemplateViewModel(null, _param);


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
