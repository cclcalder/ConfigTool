using Exceedra.Common.Utilities;
using System.ComponentModel;
using WPF.ViewModels.Phasing;

namespace WPF.Pages.Phasings
{
    /// <summary>
    /// Interaction logic for PhasingProfile.xaml
    /// </summary>
    /// 
    public partial class PhasingProfile : INotifyPropertyChanged
    {
        public PhasingProfilesViewModel ViewModel { get; set; }

        public PhasingProfile()
        {
            ViewModel = new PhasingProfilesViewModel();
            PropertyChanged.Raise(this, "ViewModel");

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
