using Model;
using ViewModels;

namespace WPF.ViewModels.Conditions
{
    public class ConditionStatusViewModel : ViewModelBase
    {
        public ConditionStatus Data { get; private set; }

        public ConditionStatusViewModel(ConditionStatus data)
        {
            Data = data;
        }

        public bool IsSelected
        {
            get { return Data.IsSelected; }
            set
            {
                Data.IsSelected = value;
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }
    }
}