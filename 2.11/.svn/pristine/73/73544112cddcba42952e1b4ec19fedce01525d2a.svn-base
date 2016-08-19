using Exceedra.Common;

namespace Model
{
    using System.ComponentModel;
    using System.Xml.Linq;

    public class PromotionVolumeOperation : INotifyPropertyChanged
    {
        public string ID { get; set; }

        private string _value;
        public string LabelText { get; set; }
        public string StoredProc { get; set; }
        public bool IsEnabled { get; set; }
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        public int Row { get; set; }

        public static PromotionVolumeOperation FromXml(XElement xml, int index)
        {
            return new PromotionVolumeOperation
                {
                    LabelText = xml.Element("LabelText").MaybeValue(),
                    StoredProc = xml.Element("StoredProc").MaybeValue(),
                    Row = index,
                    ID = xml.Element("ID").MaybeValue(),
                    Value = xml.Element("Value").MaybeValue()
                };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}