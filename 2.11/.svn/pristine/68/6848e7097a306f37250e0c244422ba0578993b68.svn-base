using Model.Entity;
using ViewModels;

namespace WPF.ViewModels.Conditions
{
    public class ConditionViewModel : ViewModelBase
    {
        private Condition _condition;
        private bool _isSelected;
        private readonly IObserver _observer;

        public ConditionViewModel(Condition condition, bool isSelected, IObserver observer)
        {
            _condition = condition;
            _isSelected = isSelected;
            _observer = observer;
        }

        public Condition ConditionData
        {
            get { return _condition; }
            set
            {
                _condition = value;
                NotifyPropertyChanged(this, vm => vm.ConditionData);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged(this, vm => vm.IsSelected);

                if (_observer != null)
                    _observer.UpdateState();
            }
        }
    }
}