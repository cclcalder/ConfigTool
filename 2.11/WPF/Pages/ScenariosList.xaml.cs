using WPF.ViewModels.Scenarios;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for ScenariosList.xaml
    /// </summary>
    public partial class ScenariosList
    {
        public ScenariosList()
        {
            InitializeComponent();

            DataContext = ScenarioMainViewModel.New();
        }        
    } 
}
