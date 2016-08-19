using Model.Entity;
using ViewModels;

namespace WPF.ViewModels.Scenarios
{
    public class SalesOrgDataViewModel : ViewModelBase
    {
        private SalesOrgData _salesOrgData;
        private bool _isSelected;

        public SalesOrgDataViewModel(SalesOrgData salesOrgData, bool isSelected)
        {
            _salesOrgData = salesOrgData;
            _isSelected = isSelected;
        }

        public SalesOrgData SalesOrgData
        {
            get { return _salesOrgData; }
            set
            {
                _salesOrgData = value;
                NotifyPropertyChanged(this, vm => vm.SalesOrgData);
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