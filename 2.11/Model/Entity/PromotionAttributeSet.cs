namespace Model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Annotations;

    public class PromotionAttributeSet : INotifyPropertyChanged
    {
        private string _comment;
        public List<PromotionAttribute> Attributes { get; set; }
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (value == _comment) return;
                _comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}