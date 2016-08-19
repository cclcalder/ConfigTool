using WPF.ViewModels;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for Phasing.xaml
    /// </summary>
    public partial class Phasing
    {
        public Phasing(PhasingViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}