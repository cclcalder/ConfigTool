using System.ComponentModel;
using Exceedra.Common.Utilities;
using Model.Annotations;
using WPF.ViewModels.Demand;

namespace WPF.Pages.Demand.DPFcMgmt
{
    /// <summary>
    /// Interaction logic for DPFcMgmtEditor.xaml
    /// </summary>
    public partial class DPFcMgmtEditor : INotifyPropertyChanged
    {
        public DPFcMgmtEditorViewModel ViewModel { get; set; }

        public DPFcMgmtEditor(string idx)
        {
            InitializeComponent();
            ViewModel = new DPFcMgmtEditorViewModel(idx);
            PropertyChanged.Raise(this, "ViewModel");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
