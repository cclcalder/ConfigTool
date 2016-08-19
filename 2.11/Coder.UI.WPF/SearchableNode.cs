using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using Coder.UI.WPF;
using Exceedra.Common.Utilities;

namespace Coder.WPF.UI
{
    using System.Windows.Input;

    public abstract class SearchableNode : DependencyObject, INotifyPropertyChanged
    {
        private static bool skipRootNode = false;
        public abstract string Title { get; set; }

        private string _foreground;

        public virtual string Foreground
        {
            get
            {
                return _foreground ?? "Black";
            }
            set
            {
                _foreground = value;
            }
        }

        public virtual string Icon { get; set; }
        public SearchableNode Parent { get; protected set; }
        protected ISearchableTreeViewNodeEventsConsumer EventsConsumer { get; set; }
        public abstract IEnumerable<SearchableNode> Children { get; }
        internal ISearchableTreeViewNodeEventsConsumer HostingTreeView { get; set; }
        protected bool IsInitialising;

        public string NodeID;

        public SearchableNode(ISearchableTreeViewNodeEventsConsumer eventsConsumer = null,
                              bool isSingleSelectMode = false, SearchableNode parent = null, bool isSelectAnyOne = false)
        {
            Icon = "/Coder.UI.WPF;component/Images/empty.png";
            Parent = parent;
            EventsConsumer = eventsConsumer;
            IsSingleSelectMode = isSingleSelectMode;
            IsSelectAnyOne = isSelectAnyOne;

            // To expand the root node of tree by default
            if (parent == null)
                IsExpanded = true;
        }


        #region IsSingleSelectMode
        /// <summary>
        /// Gets or sets the IsSingleSelectMode of this SearchableTreeViewNode.
        /// </summary>
        public bool IsSingleSelectMode { get; set; }

        public bool IsSelectAnyOne { get; set; }
        #endregion

        #region IsExpanded property

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set
            {
                SetValue(IsExpandedProperty, value);

                // Expand all the way up to the root.
                if (value && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SearchableNode), new UIPropertyMetadata(false, IsExpandedPropertyChanged));


        public static void IsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = d as SearchableNode;
        }

        #endregion

        #region IsHighlighted property

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(SearchableNode), new UIPropertyMetadata(false));

        #endregion

        public void SkipSelectingChildren()
        {
            skipRootNode = true;
        }

        #region IsSelected property

        public bool IsSelected
        {
            get
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                    return (bool)Application.Current.Dispatcher.Invoke(new Func<bool>(() => IsSelected));

                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);

                // K1 - Added to auto expand the nodes when they are pre-selected
                if (value)
                    IsExpanded = true;
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SearchableNode),
            new UIPropertyMetadata(false, IsSelectedPropertyChanged));

        private static bool _doingInSelectedHandler;
        public static void IsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = d as SearchableNode;
            if (node != null && node.IsInitialising)
                return;

            if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => IsSelectedPropertyChanged(d, e)));
                return;
            }
            var searchableNode = ((SearchableNode)d);
            if (_doingInSelectedHandler)
            {
                searchableNode.OnSelectedChanged();
                return;
            }

            try
            {
                if (searchableNode.Parent == null && skipRootNode)
                {
                    skipRootNode = false;
                    return;
                }

                _doingInSelectedHandler = true;
                if (!searchableNode.IsSingleSelectMode)
                {
                    searchableNode.GetAllChildren().Do(c => c.IsSelected = searchableNode.IsSelected);
                }

                SearchableNode rootNode = null;
                if (searchableNode.Parent != null)
                {
                    rootNode = GetRootNode(searchableNode);
                    skipRootNode = true;
                    rootNode.IsSelected = !rootNode.GetAllChildren().Any(c => c.IsSelected == false);

                    if (searchableNode.IsSingleSelectMode)
                    {
                        rootNode.GetAllChildren().Where(a => a != node && a.IsSelected).Do(a => a.IsSelected = false);
                    }
                }

                if (searchableNode.EventsConsumer != null)
                    searchableNode.EventsConsumer.NotifySelectedNodeChanged();

                if (searchableNode.HostingTreeView != null)
                    searchableNode.HostingTreeView.NotifySelectedNodeChanged();
            }
            finally
            {
                _doingInSelectedHandler = false;
                skipRootNode = false;
                searchableNode.PropertyChanged.Raise(d, "IsSelected");
                searchableNode.OnSelectedChanged();
            }
        }

        public static SearchableNode GetRootNode(SearchableNode node)
        {
            var rootNode = node.Parent;
            while (rootNode.Parent != null)
            {
                rootNode = rootNode.Parent;
            }

            return rootNode;
        }

        protected virtual void OnSelectedChanged()
        {

        }

        #endregion

        #region Search

        public bool IsAnyMatchInFullPath(string[] searchWords)
        {
            if (IsMatch(searchWords))
                return true;

            if (Children != null)
            {
                foreach (var child in Children)
                    if (child.IsAnyMatchInFullPath(searchWords))
                        return true;
            }

            return false;
        }

        public bool IsMatch(string[] searchWords)
        {
            return Title.ContainsAll(searchWords, false);
        }

        public void PerformSearch(string[] searchWords)
        {

            IsExpanded = IsAnyMatchInFullPath(searchWords);

            //added to clear any selection that is going to be made invisible to the user
            if (IsSingleSelectMode && IsExpanded == false)
            {
                //IsSelected = false;
            }

            IsHighlighted = IsMatch(searchWords);
            Children.Do(c => c.PerformSearch(searchWords));
        }

        public void ClearSearch()
        {
            IsExpanded = IsSelected;
            IsHighlighted = false;
            Children.Do(c => c.ClearSearch());
        }

        #endregion

        public IEnumerable<SearchableNode> SelectedNodes
        {
            get
            {
                if (IsSelected)
                    yield return this;

                foreach (var selectedChild in Children.SelectMany(c => c.SelectedNodes))
                {
                    yield return selectedChild;
                }
            }
        }


        #region ButonsGroupName
        /// <summary>
        /// Gets or sets the ButonsGroupName of this SearchableTreeViewNode.
        /// </summary>
        public string ButonsGroupName
        {
            get
            {
                // when we are not in IsSingleSelectMode, each control must have a unique GroupName
                // this prevents effect of radiobutton on checkboxes
                return IsSingleSelectMode ? "InsideTreeView" : Guid.NewGuid().ToString();
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public void DeselectWithChildren()
        {
            IsSelected = false;
            foreach (var child in Children)
            {
                child.DeselectWithChildren();
            }
        }

        public bool HasChildren
        {
            get { return Children.Any(); }
        }

        //private bool m_isExpanded;
        //public bool IsExpanded
        //{
        //    get { return m_isExpanded; }
        //    set
        //    {
        //        m_isExpanded = value;
        //        OnPropertyChanged("IsExpanded"););
        //        // Expand all the way up to the root.
        //        if (value && Parent != null)
        //            Parent.IsExpanded = true;
        //    }
        //}

        //public void PerformSearch(string[] searchWords)
        //{

        //    IsExpanded = IsAnyMatchInFullPath(searchWords);

        //    IsHighlighted = IsMatch(searchWords);
        //    Children.Do(c => c.PerformSearch(searchWords));
        //}

        public void PerformExpand(bool forceAll = false)
        {
            IsExpanded = IsAnySelectedInFullPath() || forceAll;

            Children.Do(c => c.PerformExpand(forceAll));
        }



        private IEnumerable<SearchableNode> GetChildren(SearchableNode parent)
        {
            yield return parent;

            if (parent.Children != null)
            {
                foreach (var relative in parent.Children.SelectMany(GetChildren))
                    yield return relative;
            }
        }

        public IEnumerable<SearchableNode> GetFlatTree()
        {
            var l = GetChildren(this);

            return l.Distinct();
        }

        public bool IsAnySelectedInFullPath()
        {
            if (IsSelected != false || Parent == null)
            {
                return true;
            }


            if (Children != null)
            {
                foreach (var child in Children)
                    if (child.IsAnySelectedInFullPath())
                        return true;
            }

            return false;
        }
    }
}
