
//using Model;

//namespace ViewModels
//{

//    using System.ComponentModel;

//    using Model.Entity.ROBs;

//    using WPF.Annotations;


//    public class StatusViewModel : INotifyPropertyChanged
//    {
//        private readonly GenericStatus _status;

//        public string ID
//        {
//            get { return _status.ID; }
//            set { _status.ID = value; }
//        }

//        public string Name
//        {
//            get { return _status.DisplayName; }
//            set { _status.DisplayName = value; }
//        }

//        public bool IsEnabled
//        {
//            get { return _status.IsEnabled; }
//            set { _status.IsEnabled = value; }
//        }

//        public bool? IsSelected
//        {
//            get { return _status.IsSelected; }
//            set
//            {
//                if (_status.IsSelected == value) return;
//                _status.IsSelected = value;
//                OnPropertyChanged("IsSelected");
//            }
//        }

//        public string Colour
//        {
//            get { return _status.Colour; }
//            set { _status.Colour = value; }
//        }

//        public StatusViewModel(GenericStatus status, PropertyChangedEventHandler handler = null)
//        {
//            _status = status;
//            if (handler != null)
//            {
//                PropertyChanged += handler;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        [NotifyPropertyChangedInvocator]
//        protected virtual void OnPropertyChanged(string propertyName)
//        {
//            var handler = PropertyChanged;
//            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}