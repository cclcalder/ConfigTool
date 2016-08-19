using System;
using Model.Entity;
using ViewModels;

namespace WPF.ViewModels.Claims
{
    public class VisibleMatchDataViewModel : ViewModelBase
    {
        private bool _isSelected;
        private MatchVisibility _visibleMatch;

        public VisibleMatchDataViewModel(MatchVisibility data, bool isSelected)
        {
            _visibleMatch = data;
            _isSelected = isSelected;
        }

        public MatchVisibility VisibleMatch
        {
            get { return _visibleMatch; }
            set
            {
                _visibleMatch = value;
                NotifyPropertyChanged(this, vm => vm.VisibleMatch);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }
    }
}