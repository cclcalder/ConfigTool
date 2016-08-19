using Exceedra.Common.GroupingMenu;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Exceedra.Views
{
    /// <summary>
    /// Interaction logic for GrMenu.xaml
    /// </summary>
    public partial class GrMenu
    {
        public GrMenu()
        {
            InitializeComponent();
        }

        #region Menu Dependency Property
        public GroupingMenu Menu
        {
            get { return (GroupingMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register("Menu", typeof(GroupingMenu),
            typeof(GrMenu), new PropertyMetadata(null));
        #endregion

        #region SelectItemCommand Dependency Property
        /// <summary>
        /// This command is invoked everytime one of the menu items is selected
        /// </summary>
        public ICommand SelectItemCommand
        {
            get { return (ICommand)GetValue(SelectItemCommandProperty); }
            set { SetValue(SelectItemCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectItemCommandProperty =
            DependencyProperty.Register("SelectItemCommand", typeof(ICommand),
            typeof(GrMenu), new PropertyMetadata(null));
        #endregion

        #region SelectedMenuItem Dependency Property
        public IMenuItem SelectedMenuItem
        {
            get { return (IMenuItem)GetValue(SelectedMenuItemProperty); }
            set { SetValue(SelectedMenuItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedMenuItemProperty =
            DependencyProperty.Register("SelectedMenuItem", typeof(IMenuItem),
                typeof(GrMenu), new PropertyMetadata(null));
        #endregion

        #region Managing selecting menu items
        /// <summary>
        /// Sets the IsSelected property of the selected menu item to TRUE
        /// (all the other menu items will have IsSelected property set to FALSE)
        /// </summary>
        private void MenuItemButton_OnClick(object sender, RoutedEventArgs e)
        {
            string menuItemId = (string)((Button)sender).Tag;
            var selectedMenuItem = Menu.MenuItems.FirstOrDefault(menuItem => menuItem.Id == menuItemId);

            // if no selected menu item found
            // or selected menu item is the same as selected before
            // ignore the click
            if (selectedMenuItem == null || SelectedMenuItem != null && selectedMenuItem.Id == SelectedMenuItem.Id)
                return;

            DeselectMenuItems();

            selectedMenuItem.IsSelected = true;
            SelectedMenuItem = selectedMenuItem;

            if (selectedMenuItem.Url != null && (selectedMenuItem.Url.Contains(".xlsm") ||selectedMenuItem.Url.Contains(".xlsx") ||
                selectedMenuItem.Url.Contains(".xltm") || selectedMenuItem.Url.Contains(".xltx") ||selectedMenuItem.Url.Contains(".csv")))
            {
                selectedMenuItem.HasValidUrl = true;
                Process.Start(selectedMenuItem.Url);
            }
           
        }

        private void DeselectMenuItems()
        {
            foreach (var menuItem in Menu.MenuItems)
                menuItem.IsSelected = false;
        }
        #endregion
    }
}