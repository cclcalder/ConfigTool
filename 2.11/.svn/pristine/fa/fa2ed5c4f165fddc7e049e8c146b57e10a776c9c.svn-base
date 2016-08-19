using WPF.Navigation;
using WPF.ViewModels.MyActionsDashboard;

namespace WPF.Pages.MyActionsDashboard
{
    public partial class MyActionsDashboard
    {
        public MyActionsDashboard(string idx)
        {
            ViewModel = new MyActionsDashboardViewModel(idx);
            InitializeComponent();
        }

        public MyActionsDashboard()
        {
            ViewModel = new MyActionsDashboardViewModel();
            InitializeComponent();
        }

        public MyActionsDashboardViewModel ViewModel { get; set; }

        public void NavigationlinkClicked(string type, string idx, bool pop)
        {
            RedirectMe.Goto(type, idx, "", "", "", pop);
        }
    }
}
