using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace Model.Entity
{


    public class UserData:INotifyPropertyChanged
    {
        public UserData() { }

        public UserData(XElement el)
        {
            ID = el.GetValue<string>("User_Idx");
            Name = el.GetValue<string>("UserName");
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this UserData.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this UserData.
        /// </summary>
        public string Name { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
