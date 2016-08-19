namespace WPF.Wizard
{
    using System;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : Page
    {
        public Customer(PromotionWizardViewModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}