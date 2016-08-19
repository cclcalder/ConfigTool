using System.Windows.Input;
using ViewHelper;
using WPF.Navigation;

namespace WPF.ViewModels
{
    using System;
    using global::ViewModels;

    public class PhasingViewModel : ViewModelBase
    {
        private readonly ApplyPhasingViewModel _applyPhasingViewModel;
        private readonly ManagePhasingViewModel _managePhasingViewModel;

        public PhasingViewModel(ApplyPhasingViewModel applyPhasingViewModel,
                                ManagePhasingViewModel managePhasingViewModel)
        {
            _applyPhasingViewModel = applyPhasingViewModel;
            _managePhasingViewModel = managePhasingViewModel;
            _managePhasingViewModel.ChangesSaved += ManagePhasingViewModelOnChangesSaved;
        }

        private void ManagePhasingViewModelOnChangesSaved(object sender, EventArgs eventArgs)
        {
            _applyPhasingViewModel.RefreshProfiles();
        }

        public bool CanApplyPhasing
        {
            get { return _applyPhasingViewModel != null && _applyPhasingViewModel.Promotions.Count > 0; }
        }

        public ApplyPhasingViewModel ApplyPhasingViewModel
        {
            get { return _applyPhasingViewModel; }
        }

        public ManagePhasingViewModel ManagePhasingViewModel
        {
            get { return _managePhasingViewModel; }
        }

        public ICommand CancelCommand => new ViewCommand(o =>
        {
            RedirectMe.ListScreen("promo");
        });
    }
}