using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using Coder.UI.WPF;
using System.Windows.Input;
using System.Windows.Media;

namespace Coder.WPF.UI
{
    /// <summary>
    /// Interaction logic for SearchableTreeView.xaml
    /// </summary>
    public partial class SearchableTreeView : INotifyPropertyChanged, ISearchableTreeViewNodeEventsConsumer
    {
        public SearchableTreeView()
        {
            InitializeComponent();
            IsEnabledChanged += OnIsEnabledChanged;
        }
      

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsEnabled = true;
        }

        #region RootNodes property

        public IEnumerable<SearchableNode> RootNodes
        {
            get { return (IEnumerable<SearchableNode>)GetValue(RootNodesProperty); }
            set { SetValue(RootNodesProperty, value); }
        }

        public static readonly DependencyProperty RootNodesProperty =
            DependencyProperty.Register("RootNodes", typeof(IEnumerable<SearchableNode>), typeof(SearchableTreeView),
            new UIPropertyMetadata(null, RootNodesPropertyChanged));

        public static void RootNodesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = (SearchableTreeView)d;
            var nodes = (IEnumerable<SearchableNode>)e.NewValue;

            if (nodes == null) return;
            var allNodes = nodes.SelectMany(r => r.WithAllChildren());
            allNodes.Do(sn => sn.HostingTreeView = treeView);

            treeView.OnPropertyChanged("FilteredRootNodes");
        }

        #endregion

        public IEnumerable<SearchableNode> FilteredRootNodes
        {
            get
            {
                if (RootNodes == null || String.IsNullOrWhiteSpace(SearchText))
                    return RootNodes;
                var searchWords = SearchText.Split(' ').Trim().ToArray();
                return RootNodes.Where(n => n.IsAnyMatchInFullPath(searchWords));
            }
        }

        #region title
        public static readonly DependencyProperty TreeTitleProperty =
          DependencyProperty.Register("TreeTitle", typeof(string),
          typeof(SearchableTreeView),
          new FrameworkPropertyMetadata() { PropertyChangedCallback = OnTitleChanged, BindsTwoWayByDefault = false }
            );

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SearchableTreeView)d).OnTitleTrackerInstanceChanged(e);
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

    #endregion

        #region style

        public static readonly DependencyProperty StyleSourceProperty =
         DependencyProperty.Register("StyleSource", typeof(Style),
         typeof(SearchableTreeView),
         new FrameworkPropertyMetadata() { PropertyChangedCallback = OnStyleSourceChanged, BindsTwoWayByDefault = false }
           );

        private static void OnStyleSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SearchableTreeView)d).OnStyleSourceTrackerInstanceChanged(e);
        }

        protected virtual void OnStyleSourceTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            TreeName.Style = (Style)e.NewValue;
        }

        public Style StyleSource
        {
            get { return (Style)GetValue(StyleSourceProperty); }

            set { SetValue(StyleSourceProperty, value); }
        }

        #endregion 

        #region SearchText property
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SearchableTreeView),
            new UIPropertyMetadata(String.Empty, SearchTextPropertyChanged));

        public static readonly DependencyProperty SearchKeystrokeProperty =
          DependencyProperty.Register("SearchKeystroke", typeof(bool), typeof(SearchableTreeView),
          new UIPropertyMetadata(true, SearchKeystrokePropertyChanged));

        private static void SearchKeystrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var x = d;

        }

        private static bool _searchKeystroke;
        public bool SearchKeystroke
        {
            get
            {
                return _searchKeystroke;
            }
            set
            {
                _searchKeystroke = value;
            }
        }

        public static void SearchTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(e.NewValue.ToString()) && !e.NewValue.Equals(e.OldValue))
                Search((SearchableTreeView)d);

            if (_searchKeystroke == true)
            {
                Search((SearchableTreeView)d);
            }

        }

        private static void Search(SearchableTreeView tv = null)
        {
            if (tv == null)
            {
                throw new Exception("error");
            }


            if (String.IsNullOrWhiteSpace(tv.SearchText))
            {
                tv.RootNodes.Do(n => n.ClearSearch());
            }
            else
            {
                var searchWords = tv.SearchText.Split(' ').Trim().ToArray();
                tv.RootNodes.Do(n => n.PerformSearch(searchWords));
            }
            tv.OnPropertyChanged("FilteredRootNodes");
        }
        #endregion

        #region ItemTemplate/PreferredItemTemplate properties
        public HierarchicalDataTemplate ItemTemplate
        {
            get { return (HierarchicalDataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(HierarchicalDataTemplate), typeof(SearchableTreeView), new UIPropertyMetadata(null, ItemTemplatePropertyPropertyChanged));

        public static void ItemTemplatePropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tv = (SearchableTreeView)d;
            tv.OnPropertyChanged("PreferredItemTemplate");
        }

        public HierarchicalDataTemplate PreferredItemTemplate
        {
            get { return ItemTemplate ?? Resources["DefaultNodeTemplate"] as HierarchicalDataTemplate; }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifySelectedNodeChanged()
        {
            if (RootNodes != null)
                SelectedNodes = RootNodes.SelectMany(n => n.SelectedNodes);
        }

        public IEnumerable<SearchableNode> SelectedNodes
        {
            get { return (IEnumerable<SearchableNode>)GetValue(SelectedNodesProperty); }
            set { SetValue(SelectedNodesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedNodes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedNodesProperty =
            DependencyProperty.Register("SelectedNodes", typeof(IEnumerable<SearchableNode>), typeof(SearchableTreeView),
            new UIPropertyMetadata(Enumerable.Empty<SearchableNode>()));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(SearchableTreeView), new PropertyMetadata(default(bool), IsReadOnlyPropertyChangedCallback));

        private static void IsReadOnlyPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((SearchableTreeView)dependencyObject).OnPropertyChanged("IsNotReadOnly");
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public bool IsNotReadOnly
        {
            get { return !IsReadOnly; }
        }

        private void tv_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            tv.IsEnabled = true;
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Search((SearchableTreeView)this);
        }

        private void imgSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Search((SearchableTreeView)this);
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Search((SearchableTreeView)this);
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Search((SearchableTreeView)this);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }


        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
           // GetNextHighlighted((sender as CheckBox).Tag.ToString()).BringIntoView();

        }

        private TreeViewItem GetNextHighlighted(string id)
        {
            var _currentNodes = FilteredRootNodes.FirstOrDefault(t => t.Parent == null).GetFlatTree().ToList();

            // find current selected node
            // find index of current selected node and shift all above to bottom of list
            var res = tv.FindTreeViewItems().FindIndex(t => t.Tag.ToString() == id);
            var step = res + 1;

            // start searching from selected node to next highlighted node
            var top = _currentNodes.Take(step).ToList();

            _currentNodes.Remove(top);
            _currentNodes.AddRange(top);

            var next = _currentNodes.FirstOrDefault(t => t.IsHighlighted);
            // return first highlighted node
            var y = tv.FindTreeViewItems().FirstOrDefault(t => t.Tag.ToString() == next.NodeID);

            return y;
 
        }

 
    }

    public static class grow
    {
        public static List<TreeViewItem> FindTreeViewItems(this Visual @this)
        {
            if (@this == null)
                return null;

            var result = new List<TreeViewItem>();

            var frameworkElement = @this as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.ApplyTemplate();
            }

            Visual child = null;
            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(@this); i < count; i++)
            {
                child = VisualTreeHelper.GetChild(@this, i) as Visual;

                var treeViewItem = child as TreeViewItem;
                if (treeViewItem != null)
                {
                    result.Add(treeViewItem);
                    if (!treeViewItem.IsExpanded)
                    {
                        treeViewItem.IsExpanded = true;
                        treeViewItem.UpdateLayout();
                    }
                }
                foreach (var childTreeViewItem in FindTreeViewItems(child))
                {
                    result.Add(childTreeViewItem);
                }
            }
            return result;
        }
    }
}
