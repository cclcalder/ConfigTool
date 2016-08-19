namespace WPF.PromoTemplates
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Finish.xaml
    /// </summary>
    public partial class Finish : Page
    {
        public Finish() : this(null)
        {
        }

        public Finish(PromotionWizardViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = viewModel;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}