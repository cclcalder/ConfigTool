using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity.Admin
{
    class UserListData
    {
        public UserListData(XElement element)
        {
            ID = element.GetValue<string>("User_Idx");
            MenuItem = element.GetValue<string>("MenuItem_Idx");
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this UserData.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region MenuItem
        /// <summary>
        /// Gets or sets the Name of this UserData.
        /// </summary>
        public string MenuItem { get; set; }

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
