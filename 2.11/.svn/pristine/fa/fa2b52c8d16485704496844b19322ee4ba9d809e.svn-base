using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Model.Annotations;

namespace Model.Entity.Admin
{
    public class MenuItemList : INotifyPropertyChanged
    {
        //private string UserID = "User_Idx";
        private const string MenuID = "MenuItem_Idx";
        private const string MenuItem = "MenuItem_Name";
        private const string GroupCode = "MenuGroup_Code";
        private const string ItemSort = "MenuItem_Sort";
        private const string CreateNewPermission = "CanCreateNew";
        private const string CanDeletePermission = "CanDelete";
        private const string PatternType = "GUIType";
        private const string MenuItemDescription = "MenuItem_Description";
        private const string GroupName = "MenuGroup_Name";
        private const string Title = "Title";
        private const string canCopy = "CanCopy";

        public MenuItemList(XElement element)
        {
            //ID = element.GetValue<string>(UserID);
            ItemTitle = element.GetValue<string>(Title);
            MenuItemID = element.GetValue<string>(MenuID);
            MenuItemName = element.GetValue<string>(MenuItem);
            MenuGroupCode = element.GetValue<string>(GroupCode);
            MenuItemSort = element.GetValue<string>(ItemSort);
            CanCreatNew = element.GetValue<string>(CreateNewPermission);
            CanDelete = element.GetValue<string>(CanDeletePermission);
            GUIType = element.GetValue<string>(PatternType);
            DesciptionMenuItem = element.GetValue<string>(MenuItemDescription);
            MenuGroupName = element.GetValue<string>(GroupName);
            CanCopy = element.GetValue<string>(canCopy);

            Url = element.GetValue<string>("URL");
        }

        //public string ID { get; set; }
        public string ItemTitle { get; set; }
        public string MenuItemID { get; set; }
        public string MenuItemName { get; set; }
        public string MenuGroupCode { get; set; }
        public string MenuItemSort { get; set; }
        public string CanCreatNew { get; set; }
        public string CanDelete { get; set; }
        public string GUIType { get; set; }
        public string DesciptionMenuItem { get; set; }
        public string MenuGroupName { get; set; }
        public string CanCopy { get; set; }

        public string Url { get; set; }

        #region IsSelected
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
