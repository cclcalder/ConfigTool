using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Model.Entity.Admin
{
    public class AdminPatternObservableCollection 
    {
        public AdminPatternObservableCollection(string groupName, string groupCode, MenuItemList menuGroup)
        {
            //this.m_menuGroup.Add(menuGroup);
            //this.m_groupCode = groupCode;
            GroupName = groupName;
            GroupCode = groupCode;
            MenuGroup.Add(menuGroup);
        }

        public void AddMenuGroup(MenuItemList menuGroup)
        {
            MenuGroup.Add(menuGroup);
        }

        public void AddItemToListOfItems(List<string> list)
        {
            if (ListOfItems == null) ListOfItems = new List<string>();

            ListOfItems = new List<string>(from l in list
                                where l != null
                                select l);
        }

        private List<string> m_listOfItems;
        public List<string> ListOfItems 
        {
            get ; 
            set ; 
        }

        public string GroupCode { get; set; }

        public string GroupName 
        {
            get ;
            set ;
        }

        public List<MenuItemList> MenuGroup = new List<MenuItemList>();
    }
}
