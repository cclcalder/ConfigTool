using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using DataGrid = System.Windows.Controls.DataGrid;
using DataGridCell = System.Windows.Controls.DataGridCell;

namespace Coder.UI.WPF
{
    /// <summary>
    /// Interaction logic for DataTreeView.xaml
    /// </summary>
    public partial class DataTreeView : INotifyPropertyChanged, IDataTreeViewNodeEventsConsumer
    {
        public DataTreeView()
        {
            InitializeComponent();
            IsEnabledChanged += OnIsEnabledChanged;
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsEnabled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifySelectedNodeChanged()
        {
            //if (RootNodes != null)
            //    SelectedNodes = RootNodes.SelectMany(n => n.SelectedNodes);
        }

        #region ItemTemplate/PreferredItemTemplate properties
        public HierarchicalDataTemplate ItemTemplate
        {
            get { return (HierarchicalDataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(HierarchicalDataTemplate), typeof(DataTreeView), new UIPropertyMetadata(null, ItemTemplatePropertyPropertyChanged));

        public static void ItemTemplatePropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tv = (DataTreeView)d;
            tv.OnPropertyChanged("PreferredItemTemplate");
        }

        public HierarchicalDataTemplate PreferredItemTemplate
        {
            get { return ItemTemplate ?? Resources["DefaultNodeTemplate"] as HierarchicalDataTemplate; }
        }
        #endregion

        #region rootnodes
        public static readonly DependencyProperty RootNodesProperty =
            DependencyProperty.Register("RootNodes", typeof(IEnumerable), typeof(DataTreeView),
            new UIPropertyMetadata(RootNodesPropertyChanged));

        private static void RootNodesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = (DataTreeView)d;
            var nodes = (IEnumerable<DataTreeNode>)e.NewValue;

            var collection = e.OldValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged -= treeView.RootNodesOnCollectionChanged;
            }

            collection = e.NewValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged += treeView.RootNodesOnCollectionChanged;
            }

            var bindableItemList = new BindableDataTreeNodeList();

            if (e.NewValue != null)
            {
                foreach (DataTreeNode eachItem in (IEnumerable)e.NewValue)
                {
                    bindableItemList.Add(eachItem);
                }
            }

            if (nodes == null) return;
            var allNodes = nodes.SelectMany(r => r.WithAllChildren());
            allNodes.Do(sn => sn.HostingTreeView = treeView);
            treeView.SetValue(BindableDataTreeItemListProperty, bindableItemList);
            treeView.OnPropertyChanged("FilteredRootNodes");
        }

        private void RootNodesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                FilteredRootNodes.Clear();
                FilteredRootNodes.AddRange(RootNodes.Cast<DataTreeNode>().Select(o => CreateBindableItem(o, this)));

            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (!FilteredRootNodes.Any(bi => bi.Equals(newItem)))
                    {
                        FilteredRootNodes.Add(CreateBindableItem((DataTreeNode)newItem, this));
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var bindableItem = FilteredRootNodes.FirstOrDefault(bi => bi.Equals(oldItem));
                    if (bindableItem != null)
                    {
                        FilteredRootNodes.Remove(bindableItem);
                    }
                }
            }
        }

        private DataTreeNode CreateBindableItem(DataTreeNode dataTreeNode, DataTreeView treeView)
        {
            return dataTreeNode;
        }
        public static readonly DependencyProperty BindableDataTreeItemListProperty = DependencyProperty.Register("FilteredRootNodes", typeof(BindableDataTreeNodeList), typeof(DataTreeView));


        public BindableDataTreeNodeList FilteredRootNodes
        {
            get { return (BindableDataTreeNodeList)GetValue(BindableDataTreeItemListProperty); }
            set { SetValue(BindableDataTreeItemListProperty, value); }
        }

        public IEnumerable RootNodes
        {
            get { return (IEnumerable)GetValue(RootNodesProperty); }
            set
            {
                SetValue(RootNodesProperty, value);
            }
        }
        #endregion

        #region SelectedNode
        public IEnumerable<DataTreeNode> SelectedNodes
        {
            get { return (IEnumerable<DataTreeNode>)GetValue(SelectedNodesProperty); }
            set { SetValue(SelectedNodesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedNodes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedNodesProperty =
            DependencyProperty.Register("SelectedNodes", typeof(IEnumerable<DataTreeNode>), typeof(DataTreeView),
            new UIPropertyMetadata(Enumerable.Empty<DataTreeNode>()));
        #endregion


        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataTreeView), new PropertyMetadata(default(bool), IsReadOnlyPropertyChangedCallback));

       // private BindableDataTreeNodeList _filteredRootNodes;

        private static void IsReadOnlyPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((DataTreeView)dependencyObject).OnPropertyChanged("IsNotReadOnly");
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


        public static readonly DependencyProperty CanSaveProperty =
            DependencyProperty.Register("CanSave", typeof(bool), typeof(DataTreeView), new PropertyMetadata(default(bool)));

        public bool CanSave
        {
            get { return (bool)GetValue(CanSaveProperty); }
            set { SetValue(CanSaveProperty, value); }
        }

        private void tv_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            tv.IsEnabled = true;
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the DataGridCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var cell = sender as DataGridCell;
            // Perform only for Checkbox Column types
            if (cell.Column is DataGridCheckBoxColumn)
            {
                if (cell != null)
                {
                    if (!cell.IsFocused)
                    {
                        cell.Focus();
                    }
                    var dataGrid = FindVisualParent<DataGrid>(cell);
                    if (dataGrid != null)
                    {
                        if (dataGrid.SelectionUnit == DataGridSelectionUnit.Cell)
                        {
                            DataRowView selectedDataRowView = (DataRowView)cell.DataContext;
                            DataRow SelectedRow = selectedDataRowView.Row;
                            int selectedNodeId = 0;
                            int selectedNodeParentId = 0;
                            bool isSelectedNodeParent = false;
                            if (SelectedRow != null)
                            {
                                selectedNodeId = int.Parse(SelectedRow[DataTreeNode.DataTreeNodeId].ToString());
                                selectedNodeParentId = int.Parse(SelectedRow[DataTreeNode.DataTreeParentNodeId].ToString());
                                isSelectedNodeParent = Boolean.Parse(SelectedRow[DataTreeNode.IsParent].ToString());
                            }
                            UpdateSelectionOnMatchingChildren(cell, selectedNodeId, selectedNodeParentId, !bool.Parse(SelectedRow[DataTreeNode.IsSelected].ToString()), isSelectedNodeParent);
                            dataGrid.Items.Refresh();
                        }
                        else
                        {
                            var row = FindVisualParent<DataGridRow>(cell);
                            if (row != null && !row.IsSelected)
                            {
                                row.IsSelected = true;
                            }
                        }
                    }
                }
            }
        }

        private IList<int> GetChilNodeIds(DataTreeNode dataTreeNode)
        {
            IList<int> childNodeIds = new List<int>();
            var childRows=dataTreeNode.Children.FirstOrDefault().DataTreeNodeSource.Rows;
            foreach (DataRow childRow in childRows)
            {
                int nodeId = int.Parse(childRow[DataTreeNode.DataTreeNodeId].ToString());
                childNodeIds.Add(nodeId);
            }

            return childNodeIds;
        }

        private IList<int> GetUpdatedChildNodeIds(IList<int> updatedChildNodeIds, IList<int> childNodeIds)
        {
            return childNodeIds.Where(c => updatedChildNodeIds.Contains(c)).ToList();
        }

        private void UpdateSelectionOnMatchingChildren(DataGridCell selectedDataGridCell, int selectedNodeId, int selectedNodeParentId, bool isSelected, bool isSelectedNodeParent)
        {
            var treeView = FindVisualParent<DataTreeView>(selectedDataGridCell);
            BindableDataTreeNodeList newParentDataTreeNodes = new BindableDataTreeNodeList();
            IEnumerable<DataTreeNode> existingParentDataTreeNodes = treeView.FilteredRootNodes;
            bool resetSelectAll = true;
            if (existingParentDataTreeNodes != null)
            {
                var existingParentDataTreeNodeWithChanges = existingParentDataTreeNodes.FirstOrDefault(x => (isSelectedNodeParent && x.NodeId == selectedNodeId) || (!isSelectedNodeParent && x.NodeId == selectedNodeParentId));
                IList<int> updatedChildNodeIds = new List<int>();
                if (isSelectedNodeParent)
                {
                    updatedChildNodeIds = GetChilNodeIds(existingParentDataTreeNodeWithChanges);
                }
                else
                {
                    updatedChildNodeIds.Add(selectedNodeId);
                }

                foreach (var parentDataTreeNode in existingParentDataTreeNodes)
                {
                    var childNodeIds = GetChilNodeIds(parentDataTreeNode);
                    bool existingParentDataTreeNode = existingParentDataTreeNodeWithChanges.Equals(parentDataTreeNode);
                    IList<int> updatedNodeIdsInParent = GetUpdatedChildNodeIds(updatedChildNodeIds, childNodeIds);
                    if (existingParentDataTreeNode || updatedNodeIdsInParent.Any())
                    {
                        var newParentDataTreeNodeSource = parentDataTreeNode.DataTreeNodeSource;
                        var newChildren = new List<DataTreeNode>();

                        //set children
                        foreach (var existingChild in parentDataTreeNode.Children)
                        {
                            var childDataTreeNodeSource = existingChild.DataTreeNodeSource;
                            foreach (DataRow childDatarow in childDataTreeNodeSource.Rows)
                            {
                                if ((isSelectedNodeParent && childDatarow[DataTreeNode.DataTreeParentNodeId].ToString().Equals(selectedNodeId.ToString())))
                                {
                                    childDatarow[DataTreeNode.IsSelected] = isSelected;
                                    if (isSelected == false) resetSelectAll = false;
                                }
                                else if ((!isSelectedNodeParent && childDatarow[DataTreeNode.DataTreeParentNodeId].ToString().Equals(selectedNodeParentId.ToString()) && childDatarow[DataTreeNode.DataTreeNodeId].ToString().Equals(selectedNodeId.ToString())))
                                {
                                    childDatarow[DataTreeNode.IsSelected] = isSelected;
                                    if (isSelected == false) resetSelectAll = false;
                                }
                                else if(updatedNodeIdsInParent.Contains(int.Parse(childDatarow[DataTreeNode.DataTreeNodeId].ToString())))
                                {
                                    childDatarow[DataTreeNode.IsSelected] = isSelected;
                                    if (isSelected == false) resetSelectAll = false;
                                }
                            }
                            existingChild.DataTreeNodeSource = new DataTable();
                            existingChild.DataTreeNodeSource = childDataTreeNodeSource;
                            newChildren.Add(existingChild);
                            parentDataTreeNode.Children = new List<DataTreeNode>(newChildren);
                        }

                        //set parent
                        foreach (DataRow datarow in newParentDataTreeNodeSource.Rows)
                        {
                            if (isSelectedNodeParent &&
                            datarow[DataTreeNode.DataTreeNodeId].ToString().Equals(selectedNodeId.ToString()))
                            {
                                datarow[DataTreeNode.IsSelected] = isSelected;
                                if (isSelected == false) resetSelectAll = false;
                            }
                            else// if (isSelectedNodeParent == false && datarow[DataTreeNode.DataTreeNodeId].ToString().Equals(selectedNodeParentId.ToString()))
                            //one child unselected
                            {
                                if (
                                    !(parentDataTreeNode.Children.Any(
                                        x => x.DataTreeNodeSource.Select().Any(y => y.Field<bool>(DataTreeNode.IsSelected) == false))))
                                {
                                    datarow[DataTreeNode.IsSelected] = true;
                                }
                                else
                                {
                                    datarow[DataTreeNode.IsSelected] = false;
                                    resetSelectAll = false;
                                }
                            }
                        }
                        parentDataTreeNode.DataTreeNodeSource = newParentDataTreeNodeSource;
                        newParentDataTreeNodes.Add(parentDataTreeNode);
                    }
                    else
                    {

                        if (parentDataTreeNode.DataTreeNodeSource.AsEnumerable()
                                              .Any(dr => !bool.Parse(dr[DataTreeNode.IsSelected].ToString())) &&
                            parentDataTreeNode.Children.ToList().Any(x => x.DataTreeNodeSource.AsEnumerable()
                                              .Any(dr => !bool.Parse(dr[DataTreeNode.IsSelected].ToString()))))
                            resetSelectAll = false;
                        newParentDataTreeNodes.Add(parentDataTreeNode);
                    }
                }

                if (isSelectedNodeParent) treeView.SetValue(BindableDataTreeItemListProperty, newParentDataTreeNodes);
                treeView.OnPropertyChanged("FilteredRootNodes");
                IsSelectAllChecked = resetSelectAll;
            }
        }

        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        private void dgDetailed_AutoGeneratinColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == DataTreeNode.IsParent || e.Column.Header.ToString() == DataTreeNode.DataTreeNodeId || e.Column.Header.ToString() == DataTreeNode.DataTreeParentNodeId)
            {
                e.Column.Visibility = Visibility.Hidden;
            }
            if (e.Column.Header.ToString() == DataTreeNode.IsSelected)
            {
                e.Column.Header = "";
            }
            else if (e.Column.Header.ToString() == DataTreeNode.IsDeleteVisible)
            {
                DataGridTemplateColumn deleteColumn = new DataGridTemplateColumn();
                deleteColumn.Header = "";
                FrameworkElementFactory deleteColumnFactory = new FrameworkElementFactory(typeof(Image));
                Binding deleteBinding = new Binding(DataTreeNode.IsDeleteVisible) { Converter = new BoolToVisibilityConverter() };
                deleteBinding.Mode = BindingMode.TwoWay;
                deleteColumnFactory.SetValue(Image.VisibilityProperty, deleteBinding);

                deleteColumnFactory.SetValue(Image.SourceProperty, FindResource("Eraser"));
                deleteColumnFactory.SetValue(Image.WidthProperty, 20.0);
                deleteColumnFactory.SetValue(Image.HeightProperty, 20.0);

                deleteColumnFactory.AddHandler(Image.MouseDownEvent, new MouseButtonEventHandler(DeleteButton_Click));
                DataTemplate deleteCellTemplate = new DataTemplate();
                deleteCellTemplate.VisualTree = deleteColumnFactory;
                deleteColumn.CellTemplate = deleteCellTemplate;
                e.Column = deleteColumn;
            }
            else
            {
                e.Column.MinWidth = 150;
                e.Column.IsReadOnly = IsReadOnly;
            }
        }

        private void DeleteButton_Click(object sender, MouseButtonEventArgs e)
        {
            Image btnClick = sender as Image;
            var dataGridRow = FindVisualParent<DataGridRow>(btnClick);
            if (dataGridRow == null) return;
            int nodeIdPosition = ((DataRowView)dataGridRow.Item).Row.Table.Columns[DataTreeNode.DataTreeNodeId].Ordinal;
            int parentNodeIdPosition = ((DataRowView)dataGridRow.Item).Row.Table.Columns[DataTreeNode.DataTreeParentNodeId].Ordinal;
            string selectedNodeId = ((DataRowView)dataGridRow.Item).Row.ItemArray[nodeIdPosition].ToString();
            string selectedNodeParentId = ((DataRowView)dataGridRow.Item).Row.ItemArray[parentNodeIdPosition].ToString();
            if (FilteredRootNodes != null)
            {
                var existingParentDataTreeNodeWithChanges = FilteredRootNodes.FirstOrDefault(x => (x.NodeId.ToString() == selectedNodeId && x.ParentNodeId.ToString() == selectedNodeParentId));
                if (existingParentDataTreeNodeWithChanges != null)
                {
                    FilteredRootNodes.Remove(existingParentDataTreeNodeWithChanges);
                    (RootNodes as IList).Remove(existingParentDataTreeNodeWithChanges);
                    SetValue(RootNodesProperty, RootNodes);
                }
                else
                {
                    BindableDataTreeNodeList newParentDataTreeNodes = new BindableDataTreeNodeList();
                    foreach (DataTreeNode parentDataTreeNode in RootNodes)
                    {
                        if (parentDataTreeNode.Children != null)
                        {
                            var childrenDataTreeNode = parentDataTreeNode.Children;
                            var deleteChildList = new List<DataTreeNode>();
                            foreach (var childDataTreeNode in childrenDataTreeNode)
                            {
                                var removeChildNode = childDataTreeNode.DataTreeNodeSource.AsEnumerable().FirstOrDefault(x => x[DataTreeNode.DataTreeNodeId].ToString() == selectedNodeId &&
                                                                                                                                x[DataTreeNode.DataTreeParentNodeId].ToString() == selectedNodeParentId);
                                if (removeChildNode != null)
                                {
                                    if (childDataTreeNode.DataTreeNodeSource.AsEnumerable().Count() == 1)
                                    {
                                        deleteChildList.Add(childDataTreeNode);
                                    }
                                    else
                                    {
                                        parentDataTreeNode.Children.FirstOrDefault().DataTreeNodeSource =
                                            childDataTreeNode.DataTreeNodeSource.AsEnumerable()
                                                                .Where(dr => dr != removeChildNode).CopyToDataTable();

                                    }
                                }
                            }
                            if (deleteChildList.Count > 0)
                            {
                                var list = parentDataTreeNode.Children as IList;
                                if (list != null)
                                {
                                    foreach (var deleteDataTreeNode in deleteChildList)
                                    {
                                        if (list.Contains(deleteDataTreeNode))
                                            list.Remove(deleteDataTreeNode);
                                    }
                                }
                            }
                            parentDataTreeNode.Children = childrenDataTreeNode;
                        }
                        newParentDataTreeNodes.Add(parentDataTreeNode);
                    }
                    SetValue(RootNodesProperty, RootNodes);
                    SetValue(BindableDataTreeItemListProperty, newParentDataTreeNodes);
                }
                SetValue(BindableDataTreeItemListProperty, FilteredRootNodes);
                OnPropertyChanged("FilteredRootNodes");
                SetValue(CanSaveProperty, true);
            }
        }

        private bool _isSelectAllChecked;
       // private bool _ignoreCollectionChange;

        public bool IsSelectAllChecked
        {
            get { return _isSelectAllChecked; }
            set
            {
                _isSelectAllChecked = value;
                OnPropertyChanged("IsSelectAllChecked");
            }
        }


        protected void SelectAllSelectionChange(object o, RoutedEventArgs e)
        {
            SelectAllDataTreeUpdate();
            OnPropertyChanged("IsSelectAllChecked");

        }


        private void SelectAllDataTreeUpdate()
        {
            if (RootNodes != null)
            {
                IEnumerable<DataTreeNode> existingParentDataTreeNodes = FilteredRootNodes;
                foreach (var parentDataTreeNode in existingParentDataTreeNodes)
                {
                    parentDataTreeNode.DataTreeNodeSource.AsEnumerable()
                                      .All(dr => (bool)(dr[DataTreeNode.IsSelected] = IsSelectAllChecked));
                    if (parentDataTreeNode.Children != null)
                        parentDataTreeNode.Children.ToList().ForEach(child => child.DataTreeNodeSource.AsEnumerable().ToList().ForEach(dr => dr[DataTreeNode.IsSelected] = IsSelectAllChecked));
                }
                OnPropertyChanged("FilteredRootNodes");
            }
        }
    }

    public interface IDataTreeViewNodeEventsConsumer
    {
        void NotifySelectedNodeChanged();
    }
}

