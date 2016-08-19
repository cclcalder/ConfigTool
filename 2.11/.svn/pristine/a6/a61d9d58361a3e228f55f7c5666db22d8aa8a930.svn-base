using Exceedra.Common.Utilities;
using System.ComponentModel;
using WPF.ViewModels.Demand.Seasonals;

namespace WPF.Pages.Demand.Seasonals
{
    /// <summary>
    /// Interaction logic for SeasonalsConfiguration.xaml
    /// </summary>
    public partial class SeasonalsConfiguration : INotifyPropertyChanged
    {
        public SeasonalConfigViewModel ViewModel { get; set; }
        public SeasonalsConfiguration()
        {
            ViewModel = new SeasonalConfigViewModel();
            PropertyChanged.Raise(this, "ViewModel");

            InitializeComponent();

        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
