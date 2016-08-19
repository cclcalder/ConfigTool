using System.Collections.Generic;

namespace Exceedra.Common.GroupingMenu
{
    public interface IGroupingMenu
    {
        /// <summary>
        /// Flat list of all the menu items
        /// </summary>
        IList<IMenuItem> MenuItems { get; set; }

        /// <summary>
        /// List of all the menu items grouped into tree
        /// </summary>
        IList<IMenuItem> GroupedMenuItems { get; }
    }
}