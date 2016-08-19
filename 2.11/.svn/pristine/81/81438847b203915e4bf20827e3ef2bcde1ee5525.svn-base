namespace WPF.ViewModels
{
    using System.ComponentModel;
    using global::ViewModels;

    public class PromotionPhasingViewModel : INotifyPropertyChanged
    {
        private bool _selected;
        private string _dayAPhaseId;
        private string _dayBPhaseId;
        private string _dayCPhaseId;
        private string _weekPhaseId;
        private string _postPromoPhaseId;

        public string ID { get; set; }
        public string Name { get; set; }
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }

        public string DayAPhaseID
        {
            get { return _dayAPhaseId; }
            set { Set(ref _dayAPhaseId, value, "DayAPhaseID"); }
        }

        public string DayBPhaseID
        {
            get { return _dayBPhaseId; }
            set { Set(ref _dayBPhaseId, value, "DayBPhaseID"); }
        }

        public string DayCPhaseID
        {
            get { return _dayCPhaseId; }
            set { Set(ref _dayCPhaseId, value, "DayCPhaseID"); }
        }

        public string PostPromoPhaseID
        {
            get { return _postPromoPhaseId; }
            set { Set(ref _postPromoPhaseId, value, "PostPromoPhaseID"); }
        }

        public string WeekPhaseID
        {
            get { return _weekPhaseId; }
            set { Set(ref _weekPhaseId, value, "WeekPhaseID"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set(ref string field, string value, string propertyName)
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}