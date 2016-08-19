using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Model.Annotations;

namespace Model.Entity.Admin
{
    public class Pattern1List : INotifyPropertyChanged
    {
        private string m_id = "Item_Idx";
        private string m_name = "Name";
        private string m_isEnabled = "IsEnabled";
        private string m_parentId = "Parent_Idx";

        public string id { get; set; }
        public string name { get; set; }
        public bool isEnabled { get; set; }
        public string isEnabledString { get; set; }
        public string parentId { get; set; }

        private bool _isSelectedBool { get; set; }

        public bool IsSelectedBool
        {
            get { return _isSelectedBool; }
            set
            {
                _isSelectedBool = value;
                OnPropertyChanged("IsSelectedBool");
            }
        }

        public Pattern1List(XElement element)
        {
            id = element.GetValue<string>(m_id);
            name = element.GetValue<string>(m_name);
            isEnabledString = element.GetValue<string>(m_isEnabled);
            if(isEnabledString == "1")
            {
                isEnabled = true;
            }
            if(isEnabledString == "0")
            {
                isEnabled = false;
            }
            parentId = element.GetValue<string>(m_parentId);

        }

        public Pattern1List()
        { }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
