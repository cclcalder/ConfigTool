using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using Exceedra.Common.Utilities;
using Model.Annotations;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace Model.Entity.Listings
{
    [XmlType(IncludeInSchema = false)]
    public enum ItemChoiceType
    {
        ID,
        Idx,
        Item_Idx,
        Parent,
        ParentIdx,
        Parent_Idx
    }

    public class TreeViewHierarchy : INotifyPropertyChanged
    {
        public TreeViewHierarchy()
        { }

        public TreeViewHierarchy(TreeViewHierarchy tvh)
        {
            Idx = tvh.Idx;
            Name = tvh.Name;
            ParentIdx = tvh.ParentIdx;
            IsSelected = tvh.IsSelected;
            IsParentNode = tvh.IsParentNode;

            switch (IsSelected)
            {
                case "1":
                case "True":
                    IsSelectedBool = true;
                    break;
                case "0":
                case "False":
                    IsSelectedBool = false;
                    break;
                default:
                    IsSelectedBool = null;
                    break;
            }

            IsExpanded = false;
            Children = new MTObservableCollection<TreeViewHierarchy>();

            if (tvh.Children == null) return;

            foreach (var item in tvh.Children)
            {
                Children.Add(new TreeViewHierarchy(item));
            }
        }

        public TreeViewHierarchy(XElement xml)
        {
            //var ser = new XmlSerializer(typeof(TreeViewHierarchy), new XmlRootAttribute(xml.Name.ToString()));
            //var tvh = (TreeViewHierarchy)ser.Deserialize(new StringReader(xml.ToString()));

            var ser = new XmlSerializer(typeof(ProductCollection));
            var wrapper = (ProductCollection)ser.Deserialize(new StringReader(xml.ToString()));

            FlatTree = wrapper.custs.Any() ? wrapper.custs : wrapper.cust.Any() ? wrapper.cust : wrapper.prods.Any() ? wrapper.prods : wrapper.prods2.Any() ? wrapper.prods2 : wrapper.users.Any() ? wrapper.users : wrapper.status.Any() ? wrapper.status : wrapper.items;

            var tvh = ConvertListToTree(FlatTree);

            Idx = tvh.Idx;
            Name = tvh.Name;
            Code = tvh.Code ?? tvh.Name;
            ParentIdx = tvh.ParentIdx;
            IsSelected = tvh.IsSelected;
            IsParentNode = tvh.IsParentNode;
            IsSelectable = tvh.IsSelectable;
            Template = tvh.Template;
            Date = tvh.Date;

            IsExpanded = true;
            Children = tvh.Children;

            FlatTree.Where(node => node.HasChildren).Do(n => n.HadChildrenInitially = true);
        }

        public static TreeViewHierarchy ConvertListToTree(List<TreeViewHierarchy> list)
        {          
            var hashedList = list.ToLookup(l => l.ParentIdx);
            list.ForEach(item =>
            {
                item.Children.Clear();
                item.Children.AddRange(hashedList[item.Idx].ToList());
                item.Children.Where(c => c.Parent == null || c.Parent.Idx != item.Idx).Do(c => c.Parent = item);
            });

            try
            {
                var rootNodes = list.Where(t => string.IsNullOrEmpty(t.ParentIdx)).ToList();
                if (!rootNodes.Any())
                {
                    var allNodesIdxs = list.Select(node => node.Idx);
                    var nodesWithNonExistingParents = list.Where(node => !allNodesIdxs.Contains(node.ParentIdx)).ToList();
                    nodesWithNonExistingParents.ForEach(rootNode => rootNode.ParentIdx = "0");

                    rootNodes = nodesWithNonExistingParents;
                }

                if (rootNodes.Count == 1)
                {
                    var firstRootNode = rootNodes.First();
                    firstRootNode.FlatTree = list;
                    return firstRootNode;
                }

                if (rootNodes.Count > 1)
                    return new TreeViewHierarchy
                    {
                        Idx = "0",
                        Name = "ALL",
                        Children = new MTObservableCollection<TreeViewHierarchy>(rootNodes),
                        FlatTree = list
                    };

                return null;
            }
            catch (Exception ex)
            {
                var tv = new TreeViewHierarchy();
                tv.Name ="No Items loaded";
                return tv;   
            }
        }

        #region Properties

        /* Maintain a flat version of the tree so we can easily do searches on the tree (e.g. get leafs) */
        [XmlIgnore]
        public List<TreeViewHierarchy> FlatTree { get; set; }

        /* This is the border colour */
        public string Colour { get; set; }

        private string _stringBackground = "#00ffffff";
        public string StringBackground
        {
            get { return _stringBackground; }
            set
            {
                _stringBackground = value;
                PropertyChanged.Raise(this, "StringBackground");
            }
        }

        private string m_isSelected;
        public string IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;
                PropertyChanged.Raise(this, "IsSelected");
            }
        }

        private string _template;
        public string Template
        {
            get
            {
                return _template ?? "default";
            }
            set
            {
                if (_template == value) return;

                _template = value;
                PropertyChanged.Raise(this, "Template");
            }
        }

        [XmlIgnore]
        public ItemChoiceType EnumType;

        [XmlChoiceIdentifier("EnumType")]
        [XmlElement("ID")]
        [XmlElement("Idx")]
        [XmlElement("Item_Idx")]
        public string Idx { get; set; }

        private string _code;
        public string Code
        {
            get
            {
                if (string.IsNullOrEmpty(_code))
                {
                    return Name;
                }
                else
                {
                    return _code;
                }

            }
            set { _code = value; PropertyChanged.Raise(this, "Code"); }
        }

        public string Name { get; set; }

        #region Date & dates propagating mechanism

        private DateTime? _date;
        public DateTime? Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    PropertyChanged.Raise(this, "Date");
                }
            }
        }

        /// <summary>
        /// Posting the date of this TreeViewHierarchy to all of its children.
        /// </summary>
        public void PostDateToChildren()
        {
            if (Children == null || !Children.Any()) return;

            foreach (var child in Children)
            {
                if (child.IsSelected != "0") child.Date = Date;
                child.PostDateToChildren();
            }
        }

        /// <summary>
        /// Going up the tree (starting from this TreeViewHierarchy) checks if parent nodes' children have all the same date.
        /// If so, this date will be assigned to the parent as well. Otherwise the parent's date will be null (blank).
        /// </summary>
        /// <param name="rootNode">This method requires access to all the tree nodes which are supposed to be passed in the rootNode argument as a top tree node.</param>
        /// <param name="referenceNode">Node from which the adjusting will be started (the node itself won't be adjusted - starting from its parents) </param>
        public void AdjustParentsDate()
        {
            if (Parent == null)
                return;

            Parent.AdjustDate();
            Parent.AdjustParentsDate();
        }

        public void AdjustDate()
        {
            var selectedChildren = Children.Where(child => child.IsSelected != "0")
                    .ToList();

            if (!selectedChildren.Any())
                return;

            var doAllSelectedChildrenHaveTheSameDate =
                selectedChildren
                .All(child => child.Date == selectedChildren[0].Date);

            Date = doAllSelectedChildrenHaveTheSameDate
                ? selectedChildren[0].Date
                : null;
        }

        #endregion

        [XmlChoiceIdentifier("EnumType")]
        [XmlElement("Parent")]
        [XmlElement("ParentIdx")]
        [XmlElement("Parent_Idx")]
        public string ParentIdx { get; set; }

        public bool HasChildren
        {
            get
            {
                return Children != null && Children.Any();
            }
        }

        public bool HadChildrenInitially { get; set; }

        /// <summary>
        /// Is used for hierachy level planning
        /// </summary>
        private bool _isParentNode;
        public bool IsParentNode
        {
            get { return _isParentNode; }
            set
            {
                // !!! WARNING !!! This property doesn't mean if this node has any children or not - it's used for hierachy level planning.
                // If you want to check if this property is a parent in meaning if it has any children use the "HasChildren" property.
                _isParentNode = value;
                PropertyChanged.Raise(this, "IsParentNode");
            }
        }

        private bool _isSelectable = true;
        public bool IsSelectable
        {
            get
            {
                return _isSelectable;
            }
            set
            {
                _isSelectable = value;
                PropertyChanged.Raise(this, "IsSelectable");
            }
        }

        private bool? m_isSelectedBool;
        public bool? IsSelectedBool
        {
            get { return GetIsSelectedBool(); }
            set
            {
                if (value == null && !HasChildren) return;
                if (value == IsSelectedBool) return;

                m_isSelectedBool = value;
                SetIsSelectedString();
                PropertyChanged.Raise(this, "IsSelectedBool");
            }

        }

        private bool? GetIsSelectedBool()
        {
            switch (IsSelected)
            {
                case "1":
                case "True":
                    //m_isSelectedBool = true;
                    return true;
                // break;
                case "0":
                case "False":
                    //m_isSelectedBool = false;
                    return false;
                // break;
                default:
                    //m_isSelectedBool = null;
                    return null;
                    // break;
            }
            //return m_isSelectedBool;
        }

        private void SetIsSelectedString()
        {
            switch (m_isSelectedBool)
            {
                case true:
                    IsSelected = "1";
                    break;
                case false:
                    IsSelected = "0";
                    break;
                default:
                    IsSelected = "2";
                    break;
            }
        }

        private bool m_isHighlighted;
        public bool IsHighlighted
        {
            get { return m_isHighlighted; }
            set
            {
                m_isHighlighted = value;
                PropertyChanged.Raise(this, "IsHighlighted");
            }

        }

        private bool? _isDelisted = false;
        public bool? IsDelisted
        {
            get { return _isDelisted; }
            set
            {
                _isDelisted = value;
                PropertyChanged.Raise(this, "IsDelisted");
            }
        }

        private bool m_isExpanded;
        public bool IsExpanded
        {
            get { return m_isExpanded; }
            set
            {
                m_isExpanded = value;
                PropertyChanged.Raise(this, "IsExpanded");
                // Expand all the way up to the root.
                if (value && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        public void PerformSearch(string[] searchWords)
        {
            IsExpanded = IsAnyMatchInFullPath(searchWords);

            IsHighlighted = IsMatch(searchWords);
            Children.Do(c => c.PerformSearch(searchWords));
        }

        public void PerformExpand(bool forceAll = false)
        {
            IsExpanded = IsAnySelectedInFullPath();
        }

        public void ExpandAll()
        {
            IsExpanded = true;
            Children.Do(c => c.IsExpanded = true);
        }

        public void CollapseAll()
        {
            IsExpanded = false;
            Children.Do(c => c.IsExpanded = false);
        }

        public void SingleSelectionExpandToSelection()
        {
            IsExpanded = IsAnySelectedInFullPath();

            if (IsExpanded)
                Children.Do(c => c.SingleSelectionExpandToSelection());
        }

        public bool IsAnySelectedInFullPath()
        {
            if (IsSelectedBool != false || ParentIdx == null)
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

        public bool ContainsAnyListingsInFullPath(List<string> Listings)
        {
            if (Listings.Contains(Idx))
            {
                IsSelectedBool = true;
                return true;
            }

            if (Children != null)
            {
                int deletedChildCount = 0;
                List<TreeViewHierarchy> copyOfChildren = new List<TreeViewHierarchy>();
                copyOfChildren.AddRange(Children);
                foreach (var child in Children)
                {

                    if (child.ContainsAnyListingsInFullPath(Listings))
                    {
                        IsSelectedBool = true;

                    }
                    else
                    {
                        child.IsSelectedBool = false;
                        copyOfChildren.RemoveAll(c => c.Idx == child.Idx);
                        deletedChildCount = deletedChildCount + 1;
                    }
                }
                Children.Clear();
                Children.AddRange(copyOfChildren);

                if (deletedChildCount == Children.Count())
                {
                    IsSelectedBool = false;
                }
            }

            if (IsSelectedBool != false)
                return true;

            return false;
        }

        public bool IsAnyMatchInFullPath(string[] searchWords)
        {
            if (IsMatch(searchWords))
            {
                StringBackground = "#ffff96";
                return true;
            }
            else
            {
                StringBackground = "#00ffffff";
            }


            if (Children != null)
            {
                foreach (var child in Children)
                    if (child.IsAnyMatchInFullPath(searchWords))
                        return true;
            }

            return false;
        }

        public void ClearSearch()
        {
            IsExpanded = IsAnySelectedInFullPath();
            IsHighlighted = false;
            StringBackground = "#00ffffff";
            Children.Do(c => c.ClearSearch());
        }

        public bool IsMatch(string[] searchWords)
        {
            return Name.ContainsAll(searchWords, false);
        }

        private TreeViewHierarchy m_parent;
        [XmlIgnore]
        public TreeViewHierarchy Parent
        {
            get { return m_parent; }
            set
            {
                m_parent = value;
                PropertyChanged.Raise(this, "Parent");
            }
        }

        private MTObservableCollection<TreeViewHierarchy> m_children = new MTObservableCollection<TreeViewHierarchy>();
        public MTObservableCollection<TreeViewHierarchy> Children
        {
            get { return m_children; }
            set
            {
                m_children = value; if (Children == null) { Children = new MTObservableCollection<TreeViewHierarchy>(); };
                PropertyChanged.Raise(this, "Children");
            }
        }

        private bool _singleSelectedItem;
        public bool SingleSelectedItem
        {
            get { return _singleSelectedItem; }
            set
            {
                if (SingleSelectedItem == value) return;
                _singleSelectedItem = value;
                PropertyChanged.Raise(this, "SingleSelectedItem");
            }
        }

        #endregion

        public void ExpandAllSelected()
        {
            if (IsSelectedBool != false)
            {
                IsExpanded = true;
                if (Children != null && Children.Any())
                    Children.Do(c => c.ExpandAllSelected());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Generic Methods 

        /* If a parent used to have children but now those children have been removed, then we can removed that parent also.  */
        public static bool RemoveChildlessParents<T>(T ti) where T : TreeViewHierarchy
        {
            if (ti.HasChildren)
                ti.Children.Remove(ti.Children.Where(RemoveChildlessParents).ToList());

            if (ti.HadChildrenInitially && !ti.HasChildren)
                return true;

            return false;
        }

        /* If a Parent has only one child, and that child is not a leaf, then we can remove that parent from sight */
        public static T RemoveSingularParents<T>(T ti) where T : TreeViewHierarchy
        {
            if (ti.HasChildren && ti.Children.Count == 1)
            {
                ti.Children[0].Parent = null;
                ti.Children[0].ParentIdx = null;
                return RemoveSingularParents((T) ti.Children[0]);
            }

            return ti;

        }

        public static IEnumerable<T> GetFlatTree<T>(T tree) where T : TreeViewHierarchy
        {
            var l = GetChildren(tree);

            return l.Distinct();
        }

        private static IEnumerable<T> GetChildren<T>(T parent) where T : TreeViewHierarchy
        {
            yield return parent;

            if (parent.Children != null)
            {
                foreach (var relative in parent.Children.SelectMany(c => GetChildren((T)c)))
                    yield return relative;
            }
        }

        public static void CheckAllStates<T>(T ti) where T : TreeViewHierarchy
        {
            if (ti.Children != null && ti.Children.Any())
            {
                foreach (var c in ti.Children)
                    CheckAllStates(c);

                if ((ti.Children.All(c => c.IsSelectedBool == true)))
                    ti.IsSelectedBool = true;
                else if (ti.Children.Any(c => c.IsSelectedBool == null) || (ti.Children.Any(c => c.IsSelectedBool == false) && ti.Children.Any(c => c.IsSelectedBool == true)))
                    ti.IsSelectedBool = null;
                else
                    ti.IsSelectedBool = false;
            }
        }

        public IEnumerable<T> GetLeafs<T>(T tree) where T : TreeViewHierarchy
        {
            return GetFlatTree(tree).Where(n => !n.HasChildren);
        }
        #endregion
    }

    [XmlRoot("Results")]
    [Serializable]
    public class ProductCollection
    {
        [XmlElement("Products")]
        public List<TreeViewHierarchy> prods { get; set; }

        [XmlElement("Product")]
        public List<TreeViewHierarchy> prods2 { get; set; }

        [XmlElement("Customers")]
        public List<TreeViewHierarchy> custs { get; set; }

        [XmlElement("Customer")]
        public List<TreeViewHierarchy> cust { get; set; }

        [XmlElement("Users")]
        public List<TreeViewHierarchy> users { get; set; }

        [XmlElement("Item")]
        public List<TreeViewHierarchy> items { get; set; }

        [XmlElement("Status")]
        public List<TreeViewHierarchy> status { get; set; }
    }

    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        public MTObservableCollection(List<T> toList)
        {
            foreach (var t in toList)
            {
                Add(t);
            }
        }

        public MTObservableCollection()
        {
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() => nh.Invoke(this,
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                DispatcherPriority.DataBind);
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }
    }

}
