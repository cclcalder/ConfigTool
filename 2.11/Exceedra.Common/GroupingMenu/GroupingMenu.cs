using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Exceedra.Common.GroupingMenu
{
    public class GroupingMenu : IGroupingMenu
    {
        private IList<IMenuItem> _menuItems;

        private static void CheckForDuplicatedId(IList<IMenuItem> menuItems)
        {
            if (menuItems.Distinct(x => x.Id).Count() != menuItems.Count)
                throw new Exception("Menu items with the same id found");
        }

        /// <summary>
        /// Flat list of all the menu items
        /// </summary>
        public IList<IMenuItem> MenuItems
        {
            get { return _menuItems ?? (_menuItems = new List<IMenuItem>()); }
            set
            {
                CheckForDuplicatedId(value);

                _menuItems = value;
            }
        }

        /// <summary>
        /// List of all the menu items grouped into tree basing on their ParentId property
        /// </summary>
        public IList<IMenuItem> GroupedMenuItems
        {
            get
            {
                SortMenuItems();
                FillMenuItemsChildren();

                return new List<IMenuItem>(MenuItems.Where(groupedMenuItem => string.IsNullOrEmpty(groupedMenuItem.ParentId)));
            }
        }

        private void FillMenuItemsChildren()
        {
            ClearMenuItemsChildren();

            foreach (var menuItem in MenuItems)
                if (menuItem.HasParent)
                {
                    var parentMenuItem = MenuItems.FirstOrDefault(pmi => pmi.Id == menuItem.ParentId);
                    if (parentMenuItem == null) throw new Exception("Not able to find parent of specified id");

                    parentMenuItem.Children.Add(menuItem);
                }
        }

        private void ClearMenuItemsChildren()
        {
            foreach (var menuItem in MenuItems)
                menuItem.Children.Clear();
        }

        private void SortMenuItems()
        {
            MenuItems = MenuItems.OrderBy(x => x.SortOrder).ToList();
        }
    }
}
