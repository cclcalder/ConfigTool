using System.ComponentModel;
using System.Runtime.CompilerServices;
using Exceedra.Common.Utilities;
using Model.Annotations;
using WPF.ViewModels.Demand;

namespace WPF.Pages.Supersession
{
    /// <summary>
    /// Interaction logic for Supersession.xaml
    /// </summary>
    public partial class Supersession : INotifyPropertyChanged
    {
        public SupersessionViewModel ViewModel { get; set; }
        public Supersession()
        {
            InitializeComponent();

            FilterCaretBtn.CaretSource = DataSelection;
            ViewModel = new SupersessionViewModel();
            PropertyChanged.Raise(this, "ViewModel");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged != null)
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
