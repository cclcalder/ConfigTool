namespace WPF.UserControls
{
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for ApplyPhasing.xaml
    /// </summary>
    public partial class ApplyPhasing : UserControl
    {
        public ApplyPhasing()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as ApplyPhasingViewModel;
            if (viewModel != null)
            {
                viewModel.RaiseCanExecuteChanged();
            }
        }
    }
}