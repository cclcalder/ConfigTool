namespace WPF.ViewModels
{
    using Model;
    using global::ViewModels;

    public class PromotionDataViewModel : ViewModelBase
    {
        private readonly IObserver Observer;
        private bool _IsSelected;
        private PromotionData _PromotionData;

        public PromotionDataViewModel(PromotionData promoData, bool isSelected)
            : this(promoData, isSelected, null)
        {
        }

        public PromotionDataViewModel(PromotionData promoData, bool isSelected, IObserver observer)
        {
            PromotionData = promoData;
            IsSelected = isSelected;
            Observer = observer;
        }

        public PromotionData PromotionData
        {
            get { return _PromotionData; }
            set
            {
                _PromotionData = value;
                NotifyPropertyChanged(this, vm => vm.PromotionData);
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
    }
}