using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Coder.WPF.UI;
using Exceedra.Common;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.Messages;
using Exceedra.DynamicGrid.Models;
using Model;
using Model.DataAccess.Admin;
using Model.DataAccess.Listings;
using Model.Entity.Admin;
using Model.Utilities;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Listings;

//using WPF.ExtensionHelper;

namespace WPF.ViewModels.Admin
{
    public class Pattern1ViewModel : ViewModelBase
    {
        private string m_currentMenuItemIdx;
        public string currentMenuItemIdx
        {
            get { return m_currentMenuItemIdx; }
            set { m_currentMenuItemIdx = value; NotifyPropertyChanged(this, vm => vm.currentMenuItemIdx); }
        }

        private ObservableCollection<Pattern1List> m_currentPattern1list;
        public ObservableCollection<Pattern1List> CurrentPattern1List
        {
            get { return m_currentPattern1list; }
            set { m_currentPattern1list = value; NotifyPropertyChanged(this, vm => vm.CurrentPattern1List); }
        }

        private ObservableCollection<Pattern2ListVM> m_pattern1LeftTreeView;
        public ObservableCollection<Pattern2ListVM> Pattern1LeftTreeView
        {
            get { return m_pattern1LeftTreeView; }
            set
            {
                m_pattern1LeftTreeView = value;
                NotifyPropertyChanged(this, vm => vm.Pattern1LeftTreeView);
            }
        }

        private Pattern1List _selected;
        public Pattern1List Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;

                if (CurrentPattern1List != null)
                    foreach (var pattern1List in CurrentPattern1List)
                        pattern1List.IsSelectedBool = false;

                if (_selected != null)
                    _selected.IsSelectedBool = true;

                NotifyPropertyChanged(this, vm => vm.Selected);
            }
        }

        private RowViewModel _gridRVM;
        public RowViewModel GridRVM
        {
            get
            {
                return _gridRVM;
            }
            set
            {
                if (_gridRVM != value)
                {
                    _gridRVM = value;
                    NotifyPropertyChanged(this, vm => vm.GridRVM);
                    NotifyPropertyChanged(this, vm => vm.RVMCount);

                }
            }
        }

        /// <summary>
        /// First record from GridRVM - node loaded to edition panel
        /// </summary>
        private RowRecord LoadedNode
        {
            get
            {
                var loadedNode = GridRVM.Records.FirstOrDefault();
                // if (loadedNode == null) throw new Exception("The node from the edition panel is null");
                return loadedNode;
            }
        }

        private bool m_canCopy;
        public bool CanCopy
        {
            get { return m_canCopy; }
            set { m_canCopy = value; NotifyPropertyChanged(this, vm => vm.CanCopy); }
        }

        private bool m_canCreateNew;
        public bool CanCreateNew
        {
            get { return m_canCreateNew; }
            set { m_canCreateNew = value; NotifyPropertyChanged(this, vm => vm.CanCreateNew); }
        }

        private bool m_canDelete;
        public bool CanDelete
        {
            get { return m_canDelete; }
            set
            {
                m_canDelete = value;
                NotifyPropertyChanged(this, vm => vm.CanDelete);
                NotifyPropertyChanged(this, vm => vm.DeleteButtonVisibility);
            }
        }

        public string RVMCount
        {
            get { return GridRVM.Records.First().Properties.Count().ToString(); }
        }

        private bool _hasData;
        public bool HasData
        {
            get { return _hasData; }
            set
            {
                _hasData = value;
                NotifyPropertyChanged(this, vm => vm.HasData);
            }
        }


        public Pattern1ViewModel()
        {
            canPattern1Save = false;
        }

        public ICommand Apply_Load
        {
            get
            {
                return new ViewCommand(IsAnyItemSelected, GetApplyList);
            }

        }

        public ICommand Apply_Delete
        {
            get
            {
                return new ViewCommand(IsAnyItemSelected, Delete);
            }
        }

        public Visibility DeleteButtonVisibility
        {
            get
            {
                return CanDelete ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool IsAnyItemSelected(object obj)
        {
            if (Pattern1LeftTreeView == null)
                return (Selected != null);

            return (SelectedIds.Any(t => t.IsSelected));
        }

        private void Delete(object obj)
        {
            var selectedItemId = GetSelectedItemId();
            var result = DeleteItem(selectedItemId);
            ParseResult(result);

            // refreshing the tree/list of nodes
            if (Pattern1LeftTreeView != null) ReloadLeftTreeView();
            ReloadLeftList();

            ClearEditingPanel();
        }

        private static void ParseResult(XElement result)
        {
            var message = result.Descendants("Msg").FirstOrDefault().MaybeValue();
            var resetCache = result.Descendants("ReloadCache").FirstOrDefault().MaybeValue() == "1";

            if (resetCache)
            {
                ListingsAccess _listingsAccess = new ListingsAccess();
                _listingsAccess.ResetListingsData();
                App.CachedListingsViewModels = new List<ListingsViewModel>();
            }

            CustomMessageBox.Show(message, (message.Contains("success") ? "Success" : "Error"), MessageBoxButton.OK,
                (message.Contains("success") ? MessageBoxImage.Information : MessageBoxImage.Error));
        }

        private XElement DeleteItem(string selectedItemId)
        {
            Pattern1Access access = new Pattern1Access();
            return access.DeleteGrid(DeleteX(StoredProcedure.AdminPatternList.DeleteGrid, currentMenuItemIdx, selectedItemId));
        }

        private string GetSelectedItemId()
        {
            if (Selected != null) return Selected.id;
            if (SelectedIds != null && SelectedIds.Any())
                return SelectedIds.First().Id;

            return "0";
        }

        public IEnumerable<Pattern2ListVM> SelectedIds
        {
            get
            {
                List<Pattern2ListVM> nodes =
                Pattern1LeftTreeView.Flatten(rp => rp.Children.Cast<Pattern2ListVM>()).ToList();
                var list = (from node in nodes
                            where node.IsSelected == true
                            select node).ToList();


                return list;
            }
        }

        public ICommand Save
        {
            get { return new ViewCommand(CanSave, SaveGrid); }
        }


        public ICommand Copy
        {
            get { return new ViewCommand(IsAnyItemSelected, CopyGrid); }
        }

        private void CopyGrid(object obj)
        {

            Pattern2ListVM selectedTreeNode = null;

            #region actual coping
            Pattern1Access access = new Pattern1Access();
            XElement result = null;
            if (Selected != null)
            {
                result = access.CopyGrid(CopyX(StoredProcedure.AdminPatternList.CopyGrid, currentMenuItemIdx, Selected.id));
            }
            else if (SelectedIds != null)
            {
                selectedTreeNode = SelectedIds.FirstOrDefault();
                result = access.CopyGrid(CopyX(StoredProcedure.AdminPatternList.CopyGrid, currentMenuItemIdx, SelectedIds.First().Id));
            }

            ParseResult(result);
            //MessageBox.Show(mess, (mess.Contains("success") ? "Success" : "Error"), MessageBoxButton.OK, (mess.Contains("success") ? MessageBoxImage.Information : MessageBoxImage.Error));

            if (CurrentPattern1List != null) CurrentPattern1List = new ObservableCollection<Pattern1List>(access.GetLeftList(currentMenuItemIdx).Result);
            #endregion

            if (Pattern1LeftTreeView != null) ReloadLeftTreeView();

            if (selectedTreeNode != null) SelectNodeOnTree(selectedTreeNode.Id);
            ClearEditingPanel();
        }

        private bool CanSave(object obj)
        {
            return canPattern1Save && GridRVM.AreRecordsFulfilled();
        }

        private bool m_canPattern1Save;
        public bool canPattern1Save { get { return m_canPattern1Save; } set { m_canPattern1Save = value; } }

        /// <summary>
        /// Checks if the nodes have the same id value
        /// </summary>
        private bool IsMatchingLoadedNode(string nodeId)
        {
            return nodeId == LoadedNode.Item_Idx;
        }

        private void ClearEditingPanel()
        {
            HasData = false;
        }

        private void SelectNodeOnTree(string nodeId)
        {
            var allNodesFromTree = Pattern1LeftTreeView.Flatten(rp => rp.Children.Cast<Pattern2ListVM>());

            if (allNodesFromTree != null)
            {
                var matchingNode = allNodesFromTree.FirstOrDefault(x => x.Id == nodeId);
                if (matchingNode != null) matchingNode.IsSelected = true;
            }

            NotifyPropertyChanged(this, vm => vm.SelectedIds);
        }

        private void SelectedItemInList(string nodeId)
        {
            if (CurrentPattern1List != null)
            {
                var matchingItem = CurrentPattern1List.FirstOrDefault(a => a.id == nodeId);
                if (matchingItem != null) Selected = matchingItem;
            }

            NotifyPropertyChanged(this, vm => vm.Selected);
        }

        private void SaveGrid(object obj)
        {
            if (canPattern1Save)
            {
                bool isSavingExistingNode = LoadedNode.Item_Idx != "-1";

                // remembering selected node
                string selectedNodeId = string.Empty;
                bool isSelectedNodeTakenFromTreeView = false;

                if (isSavingExistingNode)
                {
                    // we keep selected node id to reselect it after refreshing the tree/list of nodes
                    if (Pattern1LeftTreeView != null)
                    {
                        selectedNodeId = SelectedIds.First().Id;
                        isSelectedNodeTakenFromTreeView = true;
                    }
                    else if (CurrentPattern1List != null)
                        selectedNodeId = Selected.id;

                    // checking if node selected from the tree/list is the same as the one loaded for editing
                    if (!IsMatchingLoadedNode(selectedNodeId))
                    {
                        ShowNodesMismatchError();
                        return;
                    }
                }

                #region actual saving
                var result = SaveGrid();
                ParseResult(result);
                #endregion

                // refreshing the tree/list of nodes
                if (Pattern1LeftTreeView != null) ReloadLeftTreeView();
                ReloadLeftList();

                // reselecting previously selected node
                if (isSavingExistingNode)
                    if (isSelectedNodeTakenFromTreeView) SelectNodeOnTree(selectedNodeId);
                    else SelectedItemInList(selectedNodeId);
                // if saving new node..
                else ClearEditingPanel();
            }
        }

        private void ReloadLeftList()
        {
            Pattern1Access access = new Pattern1Access();
            CurrentPattern1List = new ObservableCollection<Pattern1List>(access.GetLeftList(currentMenuItemIdx).Result);
        }

        private XElement SaveGrid()
        {
            Pattern1Access access = new Pattern1Access();
            XElement result = access.SaveGrid(SaveX(StoredProcedure.AdminPatternList.SaveGrid, currentMenuItemIdx));
            return result;
        }

        /// <summary>
        /// Shows an error of node selected from the tree and loaded node to be different
        /// </summary>
        private static void ShowNodesMismatchError()
        {
            CustomMessageBox.Show("Selected node is different from the one that is currently under editing",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ReloadLeftTreeView()
        {
            Pattern2ListVM thisPattern2ListVm = new Pattern2ListVM();
            thisPattern2ListVm.IsThisSingleSelect = true;
            thisPattern2ListVm.AssignTree();
            thisPattern2ListVm.GetLeftItemList(currentMenuItemIdx);

            if (Pattern1LeftTreeView != null)
            {
                Pattern1LeftTreeView.Clear();
                Pattern1LeftTreeView.AddRange(thisPattern2ListVm.LeftObservableForPattern1);
            }
        }

        public XElement SaveX(string proc, string menuIDx)
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/

            var u = new XElement("User_Idx", User.CurrentUser.ID);
            var m = new XElement("MenuItem_Idx", menuIDx);

            XDocument xdoc = new XDocument(new XElement("SaveGrid"));

            xdoc.Root.Add(XElement.Parse(GridRVM.ToCoreXml().ToString()));

            xdoc.Root.Add(u);
            xdoc.Root.Add(m);

            var xml = XElement.Parse(xdoc.ToString());

            return xml;
        }

        private static string FixAdminValue(RowProperty p)
        {
            //todo - for encrypting passwords
            //encrypt password changes, no other values should be changed
            //if (p.ColumnCode == "User_LoginPassword" && p.Value !=null)
            //{
            //    p.Value = UserSecurity.Encrypt(p.Value);
            //}

            return (p.ControlType.ToLower().Contains("drop") == true && p.SelectedItem != null && p.SelectedItem.Item_Idx != null ? p.SelectedItem.Item_Idx : p.Value);
        }

        public XElement DeleteX(string proc, string menuIDx, string SelectedItemIdx)
        {
            //  /Results/RootItem/Attributes/Attribute/

            var u = new XElement("User_Idx", User.CurrentUser.ID);
            var m = new XElement("MenuItem_Idx", menuIDx);
            var s = new XElement("SelectedItem_Idx", SelectedItemIdx);
            XDocument xdoc = new XDocument(new XElement("DeleteGrid"));

            xdoc.Root.Add(u);
            xdoc.Root.Add(m);
            xdoc.Root.Add(s);

            var xml = XElement.Parse(xdoc.ToString());

            return xml;
        }


        public XElement CopyX(string proc, string menuIDx, string SelectedItemIdx)
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/

            var u = new XElement("User_Idx", User.CurrentUser.ID);
            var m = new XElement("MenuItem_Idx", menuIDx);
            var s = new XElement("SelectedItem_Idx", SelectedItemIdx);
            XDocument xdoc = new XDocument(new XElement("CopyGrid"));

            xdoc.Root.Add(u);
            xdoc.Root.Add(m);
            xdoc.Root.Add(s);

            var xml = XElement.Parse(xdoc.ToString());

            return xml;
        }

        public ICommand Apply_NewItem
        {
            get
            {
                return new ViewCommand(GetApplyListNewItem);
            }

        }


        public ICommand Cancel
        {
            get
            {
                return new ViewCommand(CanSave, cancelPattern);
            }

        }

        public void cancelPattern(object paramater)
        {
            if (currentMenuItemIdx != null)
                GetPattern1Grid(currentMenuItemIdx, "0");

            canPattern1Save = false;
            HasData = false;
        }

        public void GetApplyListNewItem(object obj)
        {
            GetPattern1Grid(currentMenuItemIdx, "-1");
            Selected = null;
            canPattern1Save = true;
            HasData = true;
        }

        public void GetApplyList(object paramter)
        {
            if (Selected != null)
            {
                GetPattern1Grid(currentMenuItemIdx, Selected.id);
            }
            else
            {
                GetPattern1Grid(m_currentMenuItemIdx, SelectedIds.First().Id);
            }
            canPattern1Save = true;
            HasData = true;
        }

        public Pattern1ViewModel(string menuItemIdx, string canCopySentString, string canCreatNewSentString, string canDeleteSentString)
        {
            AreThereParents = false;

            CanCopy = canCopySentString == "1";
            CanCreateNew = canCreatNewSentString == "1";
            CanDelete = canDeleteSentString == "1";

            currentMenuItemIdx = menuItemIdx;

            init(menuItemIdx);

            if (CurrentPattern1List.Any(pattern1Item => pattern1Item.parentId != null))
                AreThereParents = true;

            UpdateVisability();
        }

        public void UpdateVisability()
        {
            ShowPattern1LeftTreeView = AreThereParents;
            ShowPattern1LeftList = !AreThereParents;

        }

        private bool m_areThereParents;
        public bool AreThereParents
        {
            get { return m_areThereParents; }
            set { m_areThereParents = value; NotifyPropertyChanged(this, vm => vm.AreThereParents); }
        }

        private bool m_showPattern1LeftTreeView;
        public bool ShowPattern1LeftTreeView
        {
            get { return m_showPattern1LeftTreeView; }
            set { m_showPattern1LeftTreeView = value; NotifyPropertyChanged(this, vm => vm.ShowPattern1LeftTreeView); }
        }

        private bool m_showPattern1LeftList;
        public bool ShowPattern1LeftList
        {
            get { return m_showPattern1LeftList; }
            set { m_showPattern1LeftList = value; NotifyPropertyChanged(this, vm => vm.ShowPattern1LeftList); }
        }

        private void init(string menuItemIdx)
        {
            Pattern1Access access = new Pattern1Access();
            CurrentPattern1List = new ObservableCollection<Pattern1List>(access.GetLeftList(menuItemIdx).Result);
            GridRVM = new RowViewModel();
        }

        public void GetPattern1Grid(string menuItemIdx)
        {
            GetPattern1Grid(menuItemIdx, Selected.id);
        }

        public void GetPattern1Grid(string menuItemIdx, string selectedItemIdx)
        {
            currentMenuItemIdx = menuItemIdx;
            Pattern1Access access = new Pattern1Access();

            GridRVM = RowViewModel.LoadWithData(access.GetGirdXElement(menuItemIdx, selectedItemIdx));

            //check there are no dropdowns in the mix
            //            var ins = @"<GetItems>
            //                          <User_Idx>{0}</User_Idx>
            //                            <MenuItem_Idx>{1}</MenuItem_Idx>                          
            //                            <GridItem_Code>{2}</GridItem_Code>
            //                          <SelectedItem_Idx>{3}</SelectedItem_Idx>
            //                        </GetItems>";

            foreach (var col in GridRVM.Records.ToList())
            {
                foreach (var p in col.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                {
                    //p.DataSourceInput = string.Format(ins, Model.User.CurrentUser.ID,menuItemIdx,p.ColumnCode, selectedItemIdx);


                    if (!string.IsNullOrWhiteSpace(p.DataSourceInput))
                    {
                        //rip template up and send to DB

                        //var rip = XElement.Parse(p.DataSourceInput);
                        var rip = XElement.Parse(p.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<"));


                        foreach (var node in rip.Elements())
                        {

                            switch (node.Name.ToString())
                            {
                                case "User_Idx":
                                    node.Value = User.CurrentUser.ID;
                                    break;
                                case "MenuItem_Idx":
                                    node.Value = menuItemIdx;
                                    break;
                                case "GridItem_Code":
                                    node.Value = p.ColumnCode;
                                    break;
                                case "SelectedItem_Idx":
                                    node.Value = selectedItemIdx;
                                    break;

                            }

                        }
                        p.DataSourceInput = rip.ToString();
                        try
                        {
                            p.Values = new ObservableCollection<Option>(Option.GetFromXML(rip.ToString(), p.DataSource));
                            if (!string.IsNullOrWhiteSpace(p.Value))
                            {
                                p.SelectedItem = p.Values.FirstOrDefault(r => r.Item_Idx == p.Value);
                            }
                            else if (p.Values != null)
                            {
                                p.SelectedItem = p.Values.FirstOrDefault(r => r.IsSelected);
                            }
                        }

                        catch (Exception ex)
                        {

                        }
                    }


                }
            }


            NotifyPropertyChanged(this, vm => vm.GridRVM);

        }

        public void GetPattern1Grid(string menuItemIdx, List<string> selectedItemsIdx)
        {
            currentMenuItemIdx = menuItemIdx;
            Pattern1Access access = new Pattern1Access();

            GridRVM = RowViewModel.LoadWithData(access.GetGirdXElement(menuItemIdx, selectedItemsIdx));

            //check there are no dropdowns in the mix
            var ins = @"<GetItems>
                          <User_Idx>{0}</User_Idx>
                            <MenuItem_Idx>{1}</MenuItem_Idx>                          
                            <GridItem_Code>{2}</GridItem_Code>
                          <SelectedItem_Idx>{3}</SelectedItem_Idx>
                        </GetItems>";

            foreach (var col in GridRVM.Records.ToList())
            {
                foreach (var p in col.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                {
                    p.DataSourceInput = string.Format(ins, User.CurrentUser.ID, menuItemIdx, p.ColumnCode, selectedItemsIdx);
                }
            }


            NotifyPropertyChanged(this, vm => vm.GridRVM);

        }

        private ISearchableTreeViewNodeEventsConsumer m_thisTree;
        public ISearchableTreeViewNodeEventsConsumer thisTree
        {
            get { return m_thisTree; }
            set
            {
                m_thisTree = value;
            }
        }

        public void AssignTree(ISearchableTreeViewNodeEventsConsumer eventConsumer)
        {
            if (eventConsumer != null)
            {
                thisTree = eventConsumer;
            }
        }
        public void AssignTree()
        {
            thisTree = new SearchableTreeView();

        }



    }
}
