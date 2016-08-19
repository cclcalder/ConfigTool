using System.Windows;
using Exceedra.Controls.DynamicRow.Controls;
using Exceedra.Controls.DynamicRow.Models;

namespace WPF.PromoTemplates
{
    using System;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Attributes.xaml
    /// </summary>
    public partial class TemplateAttributes : Page
    {
        public TemplateAttributes()
            : this(null)
        {
        }

        public TemplateAttributes(PromotionTemplateViewModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = viewModel;


           
        }

        //private void btnNext_Click(object sender, RoutedEventArgs e)
        //{
        //    var page = new Volumes(DataContext as PromotionWizardViewModel);
        //    NavigationService.Navigate(page);
        //}
        private void Pattern1verticalControl_OnSelectedChange(object sender, RoutedEventArgs e)
        {
            var r = sender as DynamicRowControl;

            //r.ItemDataSource.Records[0].LoadDependentDrops();
        }
    }
}