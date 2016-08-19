using System.Windows;
using WPF.ViewModels.NPD;

namespace WPF.Pages.NPD
{
    /// <summary>
    /// Interaction logic for NPDList.xaml
    /// </summary>
    public partial class NPDList
    {
        private readonly NPDListViewModel _npdListViewModel;

        public NPDList()
        {
            InitializeComponent();
            DataContext = _npdListViewModel = new NPDListViewModel();
        }

        private void StartDatePicker_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.Text == "")
            {
                StartDatePicker.Text = _npdListViewModel.StartDate.ToString();
            }
        }

        private void EndDatePicker_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (EndDatePicker.Text == "")
            {
                EndDatePicker.Text = _npdListViewModel.EndDate.ToString();
            }
        }

    }
}
