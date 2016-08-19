using System;
using Model.Entity;
using ViewModels;

namespace WPF.ViewModels.Scenarios
{
    public class ScenarioDataViewModel : ViewModelBase
    {
        private readonly IObserver Observer;
        private bool _IsSelected;
        private ScenarioData _scenarioData;

        public ScenarioDataViewModel(ScenarioData scenarioData, bool isSelected)
            : this(scenarioData, isSelected, null) { }

        public ScenarioDataViewModel(ScenarioData scenarioData, bool isSelected, IObserver observer)
        {
            ScenarioData = scenarioData;
            IsSelected = isSelected;
            Observer = observer;
        }

        public ScenarioData ScenarioData
        {
            get { return _scenarioData; }
            set
            {
                _scenarioData = value;
                NotifyPropertyChanged(this, vm => vm.ScenarioData);
            }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                NotifyPropertyChanged(this, vm => vm.IsSelected);

                if (Observer != null)
                    Observer.UpdateState();
            }
        }


        private bool _activeBudget;
        public bool ActiveBudget
        {
            get { return _activeBudget; }
            set
            {
                _activeBudget = value;
                NotifyPropertyChanged(this, vm => vm.ActiveBudget);

                if (Observer != null)
                    Observer.UpdateState();
            }
        }

        public event EventHandler Saved;

        public void OnSaved(EventArgs e = null)
        {
            EventHandler handler = Saved;
            if (handler != null) handler(this, e ?? EventArgs.Empty);
        }
    }
}