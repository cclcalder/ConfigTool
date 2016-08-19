using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Controls.ViewModels;
using Model.Entity.Listings;
using ViewHelper;
using WPF.TelerikHelpers;


namespace WPF.UserControls.Trees.ViewModels
{
    [Serializable]
    public class TreeViewModel : Base
    {
        //private static List<TreeViewHierarchy> _staticList;
        private readonly TreeViewHierarchy _noDataTreeItem = new TreeViewHierarchy(XElement.Parse("<Results><Products><Idx>0</Idx><ProdLevel_Code>2</ProdLevel_Code><Name>No Data</Name><IsSelected>0</IsSelected><IsSelectable>0</IsSelectable></Products></Results>"));

        private bool? _isReadOnly;
        public bool? IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                if (ListTree != null && ListTree.Any()) ListTree[0].ExpandAllSelected();
                NotifyPropertyChanged(this, vm => IsReadOnly);
            }
        }

        /// <summary>
        /// If set to true acting normally.
        /// If set to false the items' checkboxes will be unactive. When a user clicks on the item the UnsafeCheckedEvent will be raised to be handled by the owner.
        /// Created because the owner of the tree control may need to handle events of checking / unchecking the items under special circumstances,
        /// i.e. in Contracts we don't want to change the selection of the customers if we have any existing terms - we ask a user for the approval and this behaviour is handled in the UnsafeCheckedEvent handler.
        /// </summary>
        private bool _isCheckSafe = true;
        public bool IsCheckSafe
        {
            get { return _isCheckSafe; }
            set
            {
                _isCheckSafe = value;
                NotifyPropertyChanged(this, vm => IsCheckSafe);
            }
        }

        public void ClearTree()
        {
            Listings = _noDataTreeItem;

            // replaced by adding the "IsSelected" set to 0 node into the _noDataTreeItem
            //IsReadOnly = true;
        }

        public TreeViewModel()
        {
            Listings = _noDataTreeItem;

            // replaced by adding the "IsSelected" set to 0 node into the _noDataTreeItem
            //IsReadOnly = true;
        }

        public TreeViewModel(TreeViewHierarchy listings)
        {
            Listings = listings ?? _noDataTreeItem;
        }

        public TreeViewModel(XElement xmlIn)
        {
            Listings = new TreeViewHierarchy(xmlIn);
        }

        private TreeViewHierarchy _listings = new TreeViewHierarchy();
        public TreeViewHierarchy Listings
        {
            get { return _listings; }
            set
            {
                _listings = value;
                NotifyPropertyChanged(this, vm => vm.Listings);

                ConvertToHierarchy();
            }
        }

        private List<TreeViewHierarchy> _listTree;
        public List<TreeViewHierarchy> ListTree
        {
            get { return _listTree; }
            set
            {
                if (_listTree != value)
                {
                    _listTree = value;
                    SetPropertyChangedEvents();
                    if (ListTree[0].IsSelectedBool != false) ListTree[0].PerformExpand();
                    if (IsReadOnly == true) ListTree[0].ExpandAllSelected();

                    AdjustDates();

                    NotifyPropertyChanged(this, vm => vm.ListTree);
                }
            }
        }

        private void ConvertToHierarchy()
        {
            ListTree = new List<TreeViewHierarchy> { Listings };
        }

        private void SetPropertyChangedEvents()
        {
            if (App.Configuration.LockChildren)
                GetFlatTree().Do(node => node.PropertyChanged += NodeOnPropertyChanged);
        }

        private void NodeOnPropertyChanged(object sender, PropertyChangedEventArgs p)
        {
            if (p.PropertyName == "SingleSelectedItem")
            {
                var node = (TreeViewHierarchy) sender;

                /* Handles a case where selecting on a searched tree doesn't automatically deselect the previous node. */
                if (node.SingleSelectedItem)
                    GetFlatTree().Where(n => n.SingleSelectedItem && n.Idx != node.Idx).Do(n => n.SingleSelectedItem = false);

                if (SelectionChanged != null && node.SingleSelectedItem) SelectionChanged();
            }

            if (p.PropertyName != "IsParentNode") return;

            var nodeChanged = (TreeViewHierarchy)sender;
            if (nodeChanged.IsParentNode)
            {
                DisableHierachyChildren(nodeChanged);
            }
            else
            {
                EnableHierachyChildren(nodeChanged);
            }
        }

        private void DisableHierachyChildren(TreeViewHierarchy c)
        {
            TreeViewHierarchy.GetFlatTree(c).Except(c).Do(child => child.IsSelectable = false);
        }

        private void EnableHierachyChildren(TreeViewHierarchy c)
        {
            TreeViewHierarchy.GetFlatTree(c).Except(c).Do(child => child.IsSelectable = true);
        }

        /// <summary>
        /// Notifies listeners that the selection of at least one node of the tree has changed -
        /// at least one node was checked or unchecked.
        /// </summary>
        public event Changed SelectionChanged;
        public delegate void Changed();

        public void GetSelected(TreeViewHierarchy current, bool loading = false)
        {
            GetHeirs(current, loading);
            current.PerformExpand();

            if (SelectionChanged != null) SelectionChanged();
        }

        private void GetHeirs(TreeViewHierarchy current, bool loading = false)
        {
            if (current != null)
                //SetChecked(current, current.IsSelectedBool);
            GetHeirs(current.Children, current, loading);

            if (current != null && current.IsSelectedBool != true)
                current.IsSelectedBool = false;

            GetFlatTree().Where(node => node.IsParentNode && node.IsSelectedBool == false).Do(node => node.IsParentNode = false);
            
            TreeViewHierarchy.CheckAllStates(Listings);
        }

        private void GetHeirs(IEnumerable<TreeViewHierarchy> ph, TreeViewHierarchy currentParent, bool loading = false)
        {
            if (ph != null)
            {
                foreach (TreeViewHierarchy currentChild in ph)
                {
                    //Set all child nodes to the same as their parent
                    if (currentChild.ParentIdx == currentParent.Idx)
                    {
                        currentChild.IsSelectedBool = currentParent.IsSelectedBool;
                        currentChild.IsSelected = (currentParent.IsSelectedBool == true ? "1" : "0");

                        if (currentChild.Children != null && loading == false)
                        {
                            foreach (var item in currentChild.Children)
                            {
                                SetChecked(item, currentParent.IsSelectedBool);
                            }
                        }
                    }

                    if (currentChild.Children != null)
                    {
                        GetHeirs(currentChild.Children, currentParent);
                    }
                }
            }
        }

        public void SetChecked(TreeViewHierarchy node, bool? isIt)
        {
            node.IsSelectedBool = isIt;

            if (node.Children != null)
            {
                node.Children.Select(c => { c.IsSelectedBool = isIt; return c; }).ToList();

                foreach (var item in node.Children)
                {
                    SetChecked(item, isIt);
                }
            }

            //node.GetFlatTree().Do(n => n.IsSelectedBool = isIt);
        }

        public void SetSingleSelection(TreeViewHierarchy current)
        {
            DeselectAll();
            current.SingleSelectedItem = true;
        }

        public IEnumerable<TreeViewHierarchy> GetFlatTree()
        {
            return ListTree[0].FlatTree;
        }

        /// <param name="includePartlySelectedLeafNodes">
        /// The method returns ids of all selected nodes (with IsSelectedBool == true).
        /// If set to true the method includes partly selected leaf nodes (with IsSelectedBool == null) as well.
        /// </param>
        public List<string> GetSelectedIdxs(bool includePartlySelectedLeafNodes = false)
        {
            var selectedNodes = includePartlySelectedLeafNodes 
                ? GetSelectedNodes(true).ToList() 
                : GetSelectedNodes().ToList();

            return selectedNodes.Select(node => node.Idx).ToList();
        }
        
        public void DeselectAll()
        {
            GetFlatTree().Where(node => node.IsSelectedBool != false).Do(node => node.IsSelectedBool = false);
        }

        public void SelectAll()
        {
            SetChecked(Listings, true);
        }

        /// <param name="includePartlySelectedLeafNodes">
        /// The method returns all selected nodes (with IsSelectedBool == true).
        /// If set to true the method includes partly selected leaf nodes (with IsSelectedBool == null) as well.
        /// </param>
        public IEnumerable<TreeViewHierarchy> GetSelectedNodes(bool includePartlySelectedLeafNodes = false)
        {
            if (includePartlySelectedLeafNodes)
            {
                return GetFlatTree()
                .Where(node => node.IsSelectedBool == true
                ||
                    // We consider leaf nodes that are partly selected (IsSelected == 2 <==> IsSelectedBool == null; selected but not for all the customers) as selected so they will be returned as well.
                    (
                    !node.HasChildren // Is it a leaf node (doesn't have children)
                    && node.IsSelectedBool == null // If so it will be considered as selected even if in fact it's partly selected (IsSelectedBool == null)
                    )
                );
            }

            return GetFlatTree().Where(node => node.IsSelectedBool == true && node.Children.All(n => n.IsSelectedBool == true));
        }

        //<Idx, IsSelected>
        public Dictionary<string, string> GetAsDictionary()
        {
            var dictionary = GetAllNodesAsDictionary(ListTree[0]);

            return dictionary;
        }

        //<Idx, Date>
        public Dictionary<string, string> GetAsDictionaryWithDates()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var node in GetFlatTree())
            {
                if (node.Date == null)
                    dictionary.Add(node.Idx, string.Empty);

                else
                {
                    DateTime nodeDate = (DateTime)node.Date;
                    dictionary.Add(node.Idx, nodeDate.ToString("yyyy-MM-dd"));
                }
            }

            return dictionary;
        }

        public List<string> GetAllNodeIdxs()
        {
            return GetFlatTree().Select(node => node.Idx).ToList();
        }

        public Dictionary<string, string> GetAllNodesAsDictionary(TreeViewHierarchy parentNode)
        {
            Dictionary<string, string> allNodes = new Dictionary<string, string>
            {
                {parentNode.Idx, IsSelectedAsString(parentNode)}
            };

            if (parentNode.Children != null)
                foreach (var child in parentNode.Children)
                {
                    allNodes = allNodes.Concat(GetAllNodesAsDictionary(child)).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
                }

            return allNodes;
        }

        private string IsSelectedAsString(TreeViewHierarchy t)
        {
            switch (t.IsSelectedBool)
            {
                case true:
                    return "1";
                case false:
                    return "0";
                default:
                    return "2";
            }
        }

        private IEnumerable<string> GetAllPlanningLevelNodes()
        {
            return GetFlatTree().Where(node => node.IsParentNode).Select(n => n.Idx);
        }

        //<SelectedIdx, IsPlanningLevel>
        public Dictionary<string, string> GetAsHierarchyDictionary()
        {
            var planningIdxs = GetAllPlanningLevelNodes();
            var dictionary = GetSelectedIdxs().ToDictionary(idx => idx, idx => planningIdxs.Contains(idx) ? "1" : "0");

            return dictionary;
        }

        private bool _isTreeLoading;
        public bool IsTreeLoading
        {
            get { return _isTreeLoading; }
            set
            {
                _isTreeLoading = value;
                NotifyPropertyChanged(this, vm => vm.IsTreeLoading);
            }
        }

        #region SingleSelect

        public TreeViewHierarchy GetSingleSelectedNode()
        {
            return GetFlatTree().FirstOrDefault(node => node.SingleSelectedItem);
        }

        #endregion

        #region Populating dates

        /// <summary>
        /// Adjusts date of every parent node in the tree starting from the lowest hierarchy up.
        /// </summary>
        public void AdjustDates()
        {
            var allNodes = ListTree.FlattenTree().ToList();

            // If none node has a date don't adjust anything.
            if (allNodes.All(node => node.Date == null)) 
                return;
            
            // Start adjusting nodes from the lowest hierarchy level...
            var leafNodes = allNodes.Where(node => !node.HasChildren && !string.IsNullOrEmpty(node.ParentIdx)).ToList();

            // Take parents of leaf nodes, adjust their dates, take parents of parents, adjust their datesand so on...
            AdjustParents(leafNodes);
        }

        /// <summary>
        /// Takes parents of nodesToGetParentsFrom and adjust their dates,
        /// then takes parents of parents, adjusts their dates and so on...
        /// </summary>
        /// <param name="nodesToGetParentsFrom"></param>
        private void AdjustParents(List<TreeViewHierarchy> nodesToGetParentsFrom)
        {
            var parentsOfNodes = 
                nodesToGetParentsFrom
                .Where(node => node.Parent != null) // The "all" node doesn't have any parent and it must be ignored
                .Select(node => node.Parent)
                .Distinct().ToList();

            if (!parentsOfNodes.Any()) return;

            foreach (var parentNode in parentsOfNodes)
                parentNode.AdjustDate();

            AdjustParents(parentsOfNodes);
        }

        #endregion

        public ICommand AddButtonCommand
        {
            get { return new ViewCommand(AddClick); }
        }
        public ICommand DeleteButtonCommand
        {
            get { return new ViewCommand(DeleteClick); }
        }
        
        public Action<object> AddClick;
        public Action<object> DeleteClick;

    }
}