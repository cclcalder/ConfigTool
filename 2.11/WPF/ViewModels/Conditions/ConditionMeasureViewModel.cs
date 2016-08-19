using System;
using System.Globalization;
using Model;
using ViewModels;

namespace WPF.ViewModels.Conditions
{
    public class ConditionMeasureViewModel : ViewModelBase
    {
        private ConditionMeasure _conditionMeasure;
        private bool _isSelected;
        private bool _isMarkedForDeletion;
        private readonly IObserver _observer;

        private bool _rowError;

        public ConditionMeasureViewModel(ConditionMeasure conditionMeasure, bool isSelected, IObserver observer)
        {
            _conditionMeasure = conditionMeasure;
            _isSelected = isSelected;
            _observer = observer;
            _isMarkedForDeletion = conditionMeasure.MarkedForDeletion;
        }

        public ConditionMeasure Data
        {
            get { return _conditionMeasure; }
            set
            {
                _conditionMeasure = value;
                NotifyPropertyChanged(this, vm => vm.Data);
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

        public bool RowError
        {
            get { return _rowError; }
            set
            {
                _rowError = value;
                NotifyPropertyChanged(this, vm => vm.RowError);

                if (_observer != null)
                    _observer.UpdateState();
            }
        }

        public bool IsMarkedForDeletion
        {
            get { return _isMarkedForDeletion; }
            set
            {
                _isMarkedForDeletion = value;
                this.Data.MarkedForDeletion = value;
                NotifyPropertyChanged(this, vm => vm.IsMarkedForDeletion);
            }
        }

        public bool HasChanged { get; set; }

        public void SetNewValue(decimal newValue, string format)
        {
            format = String.IsNullOrWhiteSpace(format) ? "C2" : format;

            var formatted = newValue.ToString(format);
            Data.NewValue = decimal.Parse(formatted, NumberStyles.Currency); ;
            NotifyPropertyChanged(this, vm => vm.Data);
            HasChanged = false;

            if (_observer != null)
                _observer.UpdateState();
        }
    }
}