using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Exceedra.Common.Utilities;
using Model.Annotations;
using Model.Entity.Listings;
using Telerik.Windows;
using Telerik.Windows.Controls;
using WPF.UserControls.Trees.ViewModels;
using System.Windows.Data;

namespace WPF.UserControls.Trees.Controls
{
    /// <summary>
    /// Interaction logic for TreeControl.xaml
    /// </summary>
    public partial class TreeControl : INotifyPropertyChanged
    {
        //private TreeViewModel _viewModel;

        public TreeControl()
        {
            InitializeComponent();

            MainRadTreeViewNew.AddHandler(RadMenuItem.ClickEvent, new RoutedEventHandler(OnContextMenuClick));
            //MainRadTreeViewOld.AddHandler(RadMenuItem.ClickEvent, new RoutedEventHandler(OnContextMenuClick));
        }

        public static readonly DependencyProperty TreeSourceProperty =
    DependencyProperty.Register("TreeSource", typeof(TreeViewModel),
        typeof(TreeControl),
        new FrameworkPropertyMetadata() { PropertyChangedCallback = OnDataChanged, BindsTwoWayByDefault = true }
        );

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeControl)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((TreeViewModel)e.NewValue != null)
            {
                TreeSource.PropertyChanged += _viewModel_PropertyChanged;
                if(TreeSource.IsReadOnly != null)
                IsReadOnly = TreeSource.IsReadOnly == true;
                CheckSafe = TreeSource.IsCheckSafe;


                //_viewModel = (TreeViewModel)e.NewValue;
                //_viewModel.PropertyChanged += _viewModel_PropertyChanged;
                //DataContext = _viewModel;


            }
        }

        private void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsReadOnly":
                    Dispatcher.Invoke((Action)(() => IsReadOnly = TreeSource.IsReadOnly == true));                    
                    break;
                case "IsCheckSafe":
                    Dispatcher.Invoke((Action)(() => CheckSafe = TreeSource.IsCheckSafe));
                    break;
            }
        }

        public TreeViewModel TreeSource
        {
            get { return (TreeViewModel)GetValue(TreeSourceProperty); }

            set { SetValue(TreeSourceProperty, value); }
        }


        //        public static readonly DependencyProperty ReadOnlyProperty =
        //            DependencyProperty.Register("ReadOnly", typeof(bool),
        //            typeof(TreeControl),
        //            new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true }
        //);

        /// <summary>
        /// If set to true acting normally.
        /// If set to false the items' checkboxes will be unactive. When a user clicks on the item the UnsafeCheckedEvent will be raised to be handled by the owner.
        /// </summary>
        public static readonly DependencyProperty CheckSafeProperty =
            DependencyProperty.Register("CheckSafe", typeof(bool),
            typeof(TreeControl),
            new FrameworkPropertyMetadata(true)
            );



        //public bool ReadOnly
        //{
        //    get { return (bool)GetValue(ReadOnlyProperty); }

        //    set { SetValue(ReadOnlyProperty, value); }
        //}

        public bool CheckSafe
        {
            get { return (bool)GetValue(CheckSafeProperty); }

            set { SetValue(CheckSafeProperty, value); }
        }

        public static readonly DependencyProperty TreeTitleProperty =
            DependencyProperty.Register("TreeTitle", typeof(string),
            typeof(TreeControl),
            new FrameworkPropertyMetadata() { PropertyChangedCallback = OnTitleChanged, BindsTwoWayByDefault = false }
);

        public static readonly DependencyProperty RightClickEnabledProperty =
            DependencyProperty.Register("RightClickEnabled", typeof(bool),
            typeof(TreeControl),
            new FrameworkPropertyMetadata { BindsTwoWayByDefault = true }
            );

        /// <summary>
        /// Raised when a user clicks on an item and the UnsafeCheck property is set to false.
        /// </summary>
        public static readonly RoutedEvent UnsafeCheckEvent =
            EventManager.RegisterRoutedEvent("UnsafeCheck",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler),
            typeof(TreeControl));

        public event RoutedEventHandler UnsafeCheck
        {
            add { AddHandler(UnsafeCheckEvent, value); }
            remove { RemoveHandler(UnsafeCheckEvent, value); }
        }

        public bool RightClickEnabled
        {
            get { return (bool)GetValue(RightClickEnabledProperty); }

            set { SetValue(RightClickEnabledProperty, value); }
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeControl)d).OnTitleTrackerInstanceChanged(e);
        }

        protected virtual void OnTitleTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            TreeName.Text = (string)e.NewValue;
        }

        public string TreeTitle
        {
            get { return (string)GetValue(TreeTitleProperty); }

            set { SetValue(TreeTitleProperty, value); }
        }

        private void TreeView_OnChecked(object sender, RadRoutedEventArgs e)
        {
            var current = e.OriginalSource as RadTreeViewItem;

            var x = (RadTreeView)sender;

            var isInitiallyChecked = ((RadTreeViewCheckEventArgs)e).IsUserInitiated;
            if (!isInitiallyChecked && (x.Items.Count != 0)) return;
            if (current == null) return;

            var c = current.Item as TreeViewHierarchy;

            //c.IsSelectedBool = c.IsSelectedBool != true;

            TreeSource.GetSelected(c);

            //e.Handled = true;
        }
        
        //private void GetNextHighlighted(string id, RadTreeView tv, IEnumerable<TreeViewHierarchy> items)
        //{
        //    var currentNodes = items.FirstOrDefault(t => t.Parent == null).GetFlatTree().ToList();

        //    // find current selected node
        //    // find index of current selected node and shift all above to bottom of list
        //    var res = tv.FindTreeViewItems().FindIndex(t => t.Tag == id);
        //    var step = res + 1;

        //    // start searching from selected node to next highlighted node
        //    var top = currentNodes.Take(step).ToList();

        //    //move unwanted IDs to bottom of list so we can always get to the next highlighted node
        //    currentNodes.Remove(top);
        //    currentNodes.AddRange(top);

        //    var next = currentNodes.FirstOrDefault(t => t.IsHighlighted);
        //    // return first highlighted node
        //    var y = tv.FindTreeViewItems().FirstOrDefault(t => t.Tag.ToString() == next.Idx);
        //    //tv.IsVirtualizing = false;
        //    if (y != null)
        //        y.BringIntoView();
        //    //tv.IsVirtualizing = true;
        //}


        private void UIElement_OnMouseDown(object sender, RoutedEventArgs routedEventArgs)
        {
            TextBox currentSearchTextBox = UseNewStyle ? SearchTextBoxNew : SearchTextBox;
            currentSearchTextBox.Text = "";

            Search();
        }

        private void Image_MouseDown(object sender, RoutedEventArgs routedEventArgs)
        {
            Search();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Search();

            }
        }

        private void Search()
        {
            ItemCollection nodes = MainRadTreeView.Items;

            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                TreeViewHierarchy n = (TreeViewHierarchy)nodes[0];
                n.PerformSearch(SearchTextBox.Text.Split(' '));
            }
            else
            {
                TreeViewHierarchy n = (TreeViewHierarchy)nodes[0];
                n.ClearSearch();
            }
        }

        RadTreeViewItem _clickedElement;
        private void RadContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (!RightClickEnabled || IsReadOnly)
            {
                ((RadContextMenu)sender).IsOpen = false;
                return;
            }

            // Find the tree item that is associated with the clicked context menu item
            _clickedElement = ((RadContextMenu)sender).GetClickedElement<RadTreeViewItem>();

            // disabling opening the context menu for the leaf nodes
            var clickedTreeViewHierarchy = (TreeViewHierarchy)_clickedElement.Item;
            if (clickedTreeViewHierarchy.Children == null || !clickedTreeViewHierarchy.Children.Any())
                ((RadContextMenu)sender).IsOpen = false;
        }

        private void OnContextMenuClick(object sender, RoutedEventArgs args)
        {
            var c = _clickedElement.Item as TreeViewHierarchy;

            if (c == null || c.Children == null || !c.Children.Any()) return;

            // Raising the UnsafeCheck event when a user toggles or toggles out a tree item.
            if (TryRaiseUnsafeCheckEvent(args))
                // If in response to the UnsafeCheck event a handler will decide that the event is already handled
                // the method will return true and we will prevent the rest of the method to be processed.
                return;

            c.IsParentNode = !c.IsParentNode;

            //if (c.IsParentNode)
            //    c.IsSelectedBool = true;


            //    TreeSource.GetSelected(c);

            if (c.IsSelectedBool != true)
            {
                c.IsSelectedBool = true;
                TreeSource.GetSelected(c);
                c.PerformExpand();
            }

        }

        public static readonly DependencyProperty UseNewStyleProperty = DependencyProperty.Register(
    "UseNewStyle", typeof(bool), typeof(TreeControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Used to change the visual style of the control
        /// to the one using a group box
        /// </summary>
        public bool UseNewStyle
        {
            get { return (bool)GetValue(UseNewStyleProperty); }
            set { SetValue(UseNewStyleProperty, value); }
        }

        private TextBlock TreeName { get { return TreeNameNew; } }
        private TextBox SearchTextBox { get { return SearchTextBoxNew; } }
        private RadTreeView MainRadTreeView { get { return MainRadTreeViewNew; } }

        private bool _isSingleSelect;
        public bool IsSingleSelect { get { return _isSingleSelect; } set { _isSingleSelect = value; OnPropertyChanged("IsSingleSelect"); } }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TreeView_OnItemClick(object sender, RadRoutedEventArgs e)
        {
            var treeViewItem = e.OriginalSource as RadTreeViewItem;
            if (treeViewItem == null) return;

            var c = treeViewItem.DataContext as TreeViewHierarchy;
            if (c == null) return;

            // Raising the UnsafeCheck event when a user toggles or toggles out a tree item.
            if (TryRaiseUnsafeCheckEvent(e))
                // If in response to the UnsafeCheck event a handler will decide that the event is already handled
                // the method will return true and we will prevent the rest of the method to be processed.
                return;

            if (c.IsSelectedBool != true)
            {
                c.IsSelectedBool = true;
                TreeSource.GetSelected(c);
                c.PerformExpand();
            }
            else
            {
                c.IsSelectedBool = false;
                TreeSource.GetSelected(c);
            }
        }

        private bool TryRaiseUnsafeCheckEvent(RoutedEventArgs e)
        {
            if (TreeSource != null && !CheckSafe && !IsReadOnly)
            {
                RoutedEventArgs newEventArgs = new RoutedEventArgs(UnsafeCheckEvent, e);
                RaiseEvent(newEventArgs);

                return newEventArgs.Handled;
            }

            return false;
        }

        private void SingleSelect_OnPreviewSelected(object sender, SelectionChangedEventArgs e)
        {
            if (IsSingleSelect && e.AddedItems.Count > 0)
            {
                var treeItem = ((TreeViewHierarchy)e.AddedItems[0]);
                if (treeItem != null && (treeItem.Children != null && treeItem.Children.Any() || !treeItem.IsSelectable) || IsReadOnly)
                    e.Handled = true;
            }
        }

        private void SingleSelect_OnSelected(object sender, RadRoutedEventArgs e)
        {
            OnPropertyChanged("SingleSelectedItem");
            /* Not sure we need this since the the tree binds to the tvh prop SingleSelectedItem */
            //if (IsSingleSelect)
            //{
            //    var radTreeViewItem = e.OriginalSource as RadTreeViewItem;
            //    if (radTreeViewItem != null)
            //    {
            //        var treeItem = ((TreeViewHierarchy)radTreeViewItem.Item);
            //        TreeSource.SetSingleSelection(treeItem);
            //        OnPropertyChanged("SingleSelectedItem");
            //    }
            //}
        }

        public string SingleSelectedItem
        {
            get
            {
                if (TreeSource != null)
                {
                    var singleSelectedNode = TreeSource.GetSingleSelectedNode();
                    if (singleSelectedNode != null)
                        return singleSelectedNode.Name;
                }

                return "  No item selected";
            }
        }

        private void SingleSelect_OnUnselected(object sender, RadRoutedEventArgs e)
        {
            if (IsSingleSelect)
            {
                TreeSource.DeselectAll();
                OnPropertyChanged("SingleSelectedItem");
            }
        }

        //private bool _isReadOnly;
        //public bool IsReadOnly
        //{
        //    get { return _isReadOnly; }
        //    set
        //    {
        //        _isReadOnly = value;
                
        //    }
        //    }
        //}

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TreeControl), new FrameworkPropertyMetadata() {PropertyChangedCallback = PropertyChangedCallback, BindsTwoWayByDefault = true});

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((TreeControl)dependencyObject).OnReadOnlyInstanceChanged(dependencyPropertyChangedEventArgs);
        }

        protected virtual void OnReadOnlyInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            PropertyChanged.Raise(this, "IsReadOnly");
        }
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #region Dates & DatePickers

        /// <summary>
        /// Occurs whenever a user changes the date of a node (if its datepicker is visible).
        /// </summary>
        private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;

            // Sometimes this event occurs when a user just expands/collapses a node - 
            // then the tag property is null and we won't post anything.
            if (datePicker == null || datePicker.Tag == null) return;

            PostDateUpAndDownTree(datePicker);
        }

        /// <summary>
        /// Handles setting dates of a node's children and parent according to the provide node date selection.
        /// </summary>
        /// <param name="treeViewNode"></param>
        private void PostDateUpAndDownTree(DatePicker datePicker)
        {
            // Every time we change dates of children or parent nodes this event will be hit again.
            // To prevent posting dates multiple times we check if we don't have any dates posting going on already.
            if (_isPostingDates) return;

            var nodeWithChangedDateIdx = datePicker.Tag.ToString();
            var nodeWithChangedDate = TreeSource.GetFlatTree().FirstOrDefault(node => node.Idx == nodeWithChangedDateIdx);

            if (nodeWithChangedDate == null) return;

            // a workaround for the issue: https://trello.com/c/XJt3NWDS/163-admins-listings-view-edit-listings-dates-applied-to-all-do-no-filter-down-tree
            if (nodeWithChangedDate.Date == null)
                nodeWithChangedDate.Date = datePicker.SelectedDate;

            _isPostingDates = true;
            nodeWithChangedDate.PostDateToChildren();
            nodeWithChangedDate.AdjustParentsDate();
            _isPostingDates = false;
        }

        /// <summary>
        /// Used in inheriting dates from parent nodes and vice versa to prevent 
        /// Tells if the view is currently posting dates from a node to its children or parent.
        /// </summary>
        private bool _isPostingDates;

        #endregion
    }

    public class TreeNodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultNodeTemplate { get; set; }
        public DataTemplate DateNodeTemplate { get; set; }
        public DataTemplate AddNodeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            TreeViewHierarchy treeViewHierarchy = item as TreeViewHierarchy;
            if (treeViewHierarchy == null) return null;

            if (treeViewHierarchy.Template.ToLower() == "date")
                return DateNodeTemplate;
            if(treeViewHierarchy.Template.ToLower() == "add")
                return AddNodeTemplate;

            return DefaultNodeTemplate;
        }
    }
}
