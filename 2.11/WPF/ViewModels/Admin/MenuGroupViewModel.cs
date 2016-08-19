using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Coder.WPF.UI;
using Exceedra.Common;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess.Admin;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity.Admin;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Listings;
using WPF.UserControls.Trees.ViewModels;
using Model.Entity.Listings;

namespace WPF.ViewModels.Admin
{
    public class MenuGroupViewModel : ViewModelBase
    {
        private static string menuItemIdx;
        private static ObservableCollection<TreeViewHierarchy> m_hierarchyClass;
        private readonly bool _isDeleteButtonVisible = true;
        private readonly MenuItemAccessor menuItemAccessor = new MenuItemAccessor();
        private bool _hasData;
        private TreeViewModel _pattern2LeftTree = new TreeViewModel();
        private TreeViewModel _pattern2RightTree = new TreeViewModel();
        private string _breadCrumbSection;
        private string _breadcrumbTitle;
        private ObservableCollection<ObservableCollection<MenuItemList>> _collectionOfMenuGroups;
        private bool _gui1Vis;
        private bool _gui2Vis;
        private string _guiCode;
        private ObservableCollection<SearchableNode> _leftPattern1TreeNode;
        private ISearchableTreeViewNodeEventsConsumer _pattern1EventConsumer;
        private ObservableCollection<Pattern1List> _pattern1LeftList;
        private ObservableCollection<Pattern2ListVM> _pattern1LeftTreeView;
        private Pattern1ViewModel _pattern1VM;
        private string _pattern2Description;
        private string _pattern2LeftlistHeader;
        private string _pattern2RightListgHeader;
        private string _pattern2Title;
        private Pattern2ListVM _pattern2VM;
        private ObservableCollection<SearchableNode> _rightSelectedNode;

        public MenuGroupViewModel()
        {
            GUI1Vis = false;
            GUI2Vis = false;

            LoadMenuItemList(menuItemAccessor.GetMenuList());
        }

        public string GUICode
        {
            get { return _guiCode; }
            set
            {
                _guiCode = value;
                NotifyPropertyChanged(this, vm => vm.GUICode);
            }
        }

        public bool GUI2Vis
        {
            get { return _gui2Vis; }
            set
            {
                _gui2Vis = value;
                NotifyPropertyChanged(this, vm => vm.GUI2Vis);
            }
        }

        public bool GUI1Vis
        {
            get { return _gui1Vis; }
            set
            {
                _gui1Vis = value;
                NotifyPropertyChanged(this, vm => vm.GUI1Vis);
            }
        }

        public ObservableCollection<ObservableCollection<MenuItemList>> CollectionOfMenuGroups
        {
            get { return _collectionOfMenuGroups; }
            set
            {
                _collectionOfMenuGroups = value;
                NotifyPropertyChanged(this, vm => vm.CollectionOfMenuGroups);
            }
        }

        public ObservableCollection<SearchableNode> RightSelectedNode
        {
            get { return _rightSelectedNode; }
            set
            {
                _rightSelectedNode = value;
                NotifyPropertyChanged(this, vm => vm.RightSelectedNode);
            }
        }

        public ObservableCollection<SearchableNode> LeftPattern1TreeNode
        {
            get { return _leftPattern1TreeNode; }
            set
            {
                _leftPattern1TreeNode = value;
                NotifyPropertyChanged(this, vm => vm.LeftPattern1TreeNode);
            }
        }

        public ICommand MenuItemSelection
        {
            get { return new ViewCommand(GoToLoadData); }
        }

        public ObservableCollection<TreeViewHierarchy> HierarchyClass
        {
            get { return m_hierarchyClass; }
            set
            {
                m_hierarchyClass = value;
                NotifyPropertyChanged(this, vm => vm.HierarchyClass);
            }
        }

        public TreeViewModel Pattern2RightTree
        {
            get { return _pattern2RightTree; }
            set
            {
                _pattern2RightTree = value;
                NotifyPropertyChanged(this, vm => vm.Pattern2RightTree);
            }
        }

        public TreeViewModel Pattern2LeftTree
        {
            get { return _pattern2LeftTree; }
            set
            {
                _pattern2LeftTree = value;
                NotifyPropertyChanged(this, vm => vm.Pattern2LeftTree);
            }
        }

        public ObservableCollection<Pattern1List> Pattern1LeftList
        {
            get { return _pattern1LeftList; }
            set
            {
                _pattern1LeftList = value;
                NotifyPropertyChanged(this, vm => vm.Pattern1LeftList);
            }
        }

        public string Pattern2LeftListHeader
        {
            get { return _pattern2LeftlistHeader; }
            set
            {
                _pattern2LeftlistHeader = value;
                NotifyPropertyChanged(this, vm => vm.Pattern2LeftListHeader);
            }
        }

        public string Pattern2RightListHeader
        {
            get { return _pattern2RightListgHeader; }
            set
            {
                _pattern2RightListgHeader = value;
                NotifyPropertyChanged(this, vm => vm.Pattern2RightListHeader);
            }
        }

        public string MenuTitle
        {
            get { return _pattern2Title; }
            set
            {
                _pattern2Title = value;
                NotifyPropertyChanged(this, vm => vm.MenuTitle);
            }
        }

        public string Pattern2Description
        {
            get { return _pattern2Description; }
            set
            {
                _pattern2Description = value;
                NotifyPropertyChanged(this, vm => vm.Pattern2Description);
                NotifyPropertyChanged(this, vm => vm.HasNotes);
            }
        }

        public ISearchableTreeViewNodeEventsConsumer Pattern1EventConsumer
        {
            get { return _pattern1EventConsumer; }
            set
            {
                _pattern1EventConsumer = value;
                NotifyPropertyChanged(this, vm => vm.Pattern1EventConsumer);
            }
        }

        public string Pattern2CurrentMenuItem { get; set; }

        public ObservableCollection<Pattern2ListVM> Pattern1LeftTreeView
        {
            get { return _pattern1LeftTreeView; }
            set
            {
                _pattern1LeftTreeView = value;
                NotifyPropertyChanged(this, vm => vm.Pattern1LeftTreeView);
            }
        }

        public Pattern1ViewModel Pattern1VM
        {
            get { return _pattern1VM; }
            set
            {
                if (!Equals(_pattern1VM, value))
                {
                    _pattern1VM = value;
                    NotifyPropertyChanged(this, vm => vm.Pattern1VM);
                }
            }
        }

        public Pattern2ListVM Pattern2VM
        {
            get { return _pattern2VM; }
            set
            {
                if (_pattern2VM != value)
                {
                    _pattern2VM = value;
                    NotifyPropertyChanged(this, vm => vm.Pattern2VM);
                }
            }
        }

        public ICommand Apply_NewItem
        {
            get { return new ViewCommand(GetApplyListNewItem); }
        }

        public ICommand Save_Pattern2
        {
            get { return new ViewCommand(CanSave, SavePattern2); }
        }

        public bool CanPattern2Save { get; set; }

        public bool HasData
        {
            get { return _hasData; }
            set
            {
                _hasData = value;
                NotifyPropertyChanged(this, vm => vm.HasData);
            }
        }

        public ICommand Cancel_Pattern2
        {
            get { return new ViewCommand(CancelPattern2); }
        }

        public ICommand Apply_Pattern2
        {
            get { return new ViewCommand(CanApply, ApplyPattern2); }
        }

        //public ICommand Delete_Pattern2
        //{
        //    get { return new ViewCommand(CanDelete, DeletePattern2); }
        //}

        public Visibility DeleteButtonVisibility
        {
            get
            {
                if (_isDeleteButtonVisible) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public string BreadcrumbTitle
        {
            get { return _breadcrumbTitle; }
            set
            {
                _breadcrumbTitle = value;
                NotifyPropertyChanged(this, vm => vm.BreadcrumbTitle);
            }
        }

        public string BreadCrumbSection
        {
            get { return (string.IsNullOrEmpty(_breadCrumbSection) ? "Home" : _breadCrumbSection); }
            set
            {
                _breadCrumbSection = value;
                NotifyPropertyChanged(this, vm => vm.BreadCrumbSection);
            }
        }

        public bool HasNotes
        {
            get { return !string.IsNullOrEmpty(Pattern2Description); }
        }

        public void CallAccessorAndPopulateCollections()
        {
            LoadMenuItemList(menuItemAccessor.GetMenuList());
        }

        public void LoadMenuItemList(Task<IList<MenuItemList>> enteredListTask)
        {
            if (CollectionOfMenuGroups == null)
                CollectionOfMenuGroups = new ObservableCollection<ObservableCollection<MenuItemList>>();
            //int nullCount = 0;
            IList<MenuItemList> listToPass;
            listToPass = enteredListTask.Result;

            var listOfGroupNames = new List<string>(from item in listToPass
                where item.MenuGroupName != null
                select item.MenuGroupName);

            foreach (var a in listOfGroupNames.Distinct())
            {
                var thisMenuItemList = new ObservableCollection<MenuItemList>(
                    from menuList in listToPass
                    where menuList.MenuGroupName == a
                    select menuList);

                CollectionOfMenuGroups.Add(thisMenuItemList);
            }
        }

        public void GoToLoadData(object thisMenuItemList)
        {
            var menuItemList = thisMenuItemList as MenuItemList;
            if (menuItemList == null) return;

            ClearSelection();
            LoadData(thisMenuItemList as MenuItemList);
            SelectMenuItem(menuItemList.MenuItemID);

            CancelPattern2(null);

            NotifyPropertyChanged(this, vm => vm.CollectionOfMenuGroups);
        }

        private void ClearSelection()
        {
            foreach (var menuItem in CollectionOfMenuGroups.SelectMany(menuGroup => menuGroup))
                menuItem.IsSelected = false;
        }

        private void SelectMenuItem(string menuItemId)
        {
            var menuItemToSelect =
                CollectionOfMenuGroups.SelectMany(menuGroup => menuGroup)
                    .FirstOrDefault(menuItem => menuItem.MenuItemID == menuItemId);

            if (menuItemToSelect != null) menuItemToSelect.IsSelected = true;
        }

        private void LoadData(MenuItemList thisMenuItemList)
        {
            if (Pattern1VM != null)
            {
                Pattern1VM.GridRVM = new RowViewModel();
                Pattern1VM.HasData = false;
            }

            if (Pattern2VM != null)
            {
                Pattern2VM.hierarchyClass = null;
                HasData = false;
                CanPattern2Save = false;
            }

            Pattern2LeftListHeader = "";
            Pattern2Description = "";
            Pattern2RightListHeader = "";
            BreadCrumbSection = "";
            MenuTitle = "";

            DoesPattern2VmExist();

            GUICode = thisMenuItemList.GUIType;

            Pattern2Description = thisMenuItemList.DesciptionMenuItem;
            if (thisMenuItemList.ItemTitle != null)
            {
                var tempStrings = thisMenuItemList.ItemTitle.Split(',');
                if (tempStrings.Any())
                {
                    Pattern2LeftListHeader = tempStrings[0];
                }
                else
                {
                    Pattern2LeftListHeader = "";
                }
                if (tempStrings.Count() > 1)
                {
                    Pattern2RightListHeader = tempStrings[1];
                }
                else
                {
                    Pattern2RightListHeader = "";
                }
            }
            BreadCrumbSection = thisMenuItemList.MenuGroupName;
            MenuTitle = BreadcrumbTitle = thisMenuItemList.MenuItemName;

            GUI1Vis = (GUICode == "1");

            GUI2Vis = (GUICode == "2");

            if (GUICode == "1")
            {
                if (Pattern1VM != null)
                    Pattern1VM.cancelPattern(null);

                Pattern1VM.GridRVM = new RowViewModel();

                Pattern1VM = new Pattern1ViewModel(thisMenuItemList.MenuItemID, thisMenuItemList.CanCopy,
                    thisMenuItemList.CanCreatNew, thisMenuItemList.CanDelete);
                if (Pattern1VM.CurrentPattern1List.Any(pattern1Item => pattern1Item.parentId != null))
                {
                    var thisPattern2ListVm = new Pattern2ListVM();
                    thisPattern2ListVm.IsThisSingleSelect = true;
                    if (Pattern1EventConsumer != null)
                        thisPattern2ListVm.AssignTree(Pattern1EventConsumer);
                    else
                        thisPattern2ListVm.AssignTree();

                    Pattern1LeftTreeView = new ObservableCollection<Pattern2ListVM>();
                    thisPattern2ListVm.GetLeftItemList(thisMenuItemList.MenuItemID);
                    Pattern1LeftTreeView.AddRange(thisPattern2ListVm.LeftObservableForPattern1);
                    Pattern1VM.Pattern1LeftTreeView = new ObservableCollection<Pattern2ListVM>();
                    Pattern1VM.Pattern1LeftTreeView.AddRange(Pattern1LeftTreeView);
                }
                Pattern2VM.hierarchyClass = null;
            }


            if (GUICode == "2")
            {
                Pattern2CurrentMenuItem = thisMenuItemList.MenuItemID;
                menuItemIdx = thisMenuItemList.MenuItemID;

                LoadPattern2LeftTree(thisMenuItemList.MenuItemID);

                if (Pattern1VM != null)
                {
                    Pattern1VM.cancelPattern(null);
                }
            }
        }

        private void LoadPattern2LeftTree(string menuIdx)
        {
            var args = CommonXml.GetBaseArguments("GetList");
            args.AddElement("MenuItem_Idx", menuIdx);
            Pattern2LeftTree.Listings = DynamicDataAccess.GetGenericItem<TreeViewModel>(Pattern2Access.GetLeftTreeProc(), args).Listings;
            Pattern2LeftTree.ListTree[0].IsExpanded = true;
        }

        public void NewPattern2ListVM(ISearchableTreeViewNodeEventsConsumer eventComsumer)
        {
            DoesPattern2VmExist();
            if (Pattern2VM != null)
            {
                Pattern2VM = new Pattern2ListVM();
            }
            if (eventComsumer != null)
            {
                Pattern2VM.AssignTree(eventComsumer);
            }
            else
            {
                Pattern2VM.AssignTree();
            }
        }

        public void NewPattern1List(ISearchableTreeViewNodeEventsConsumer eventConsumer)
        {
            DoesPattern1Exist();

            if (eventConsumer != null)
            {
                Pattern1EventConsumer = eventConsumer;
            }
        }

        public void DoesPattern2VmExist()
        {
            if (Pattern2VM == null)
            {
                Pattern2VM = new Pattern2ListVM();
            }
        }

        public void DoesPattern1Exist()
        {
            if (Pattern1VM == null)
            {
                Pattern1VM = new Pattern1ViewModel();
            }
        }

        public void GetApplyListNewItem(object obj)
        {
            if (GUICode == "2")
            {
            }

            if (GUICode == "1")
            {
                Pattern1VM.GetPattern1Grid(Pattern2CurrentMenuItem);
            }
        }

        private bool CanSave(object obj)
        {
            return CanPattern2Save;
        }

        public void SavePattern2(object paramater)
        {
            var leftList = new Pattern2SaveClass();
            //Pattern2LeftTree
            foreach (var idx in Pattern2LeftTree.GetAllNodeIdxs())
            {
                leftList.ID.Add(idx);
            }
            foreach (var node in Pattern2LeftTree.GetFlatTree())
            {
                leftList.isSelected.Add(node.IsSelected == "1" ? 1 : 0);
            }


            //Move pattern2listvm stuff into telerik (hirearchy?) stuff 
            //foreach (Pattern2ListVM currentVM in pattern2LeftList.SelectMany(a => a.Children))
            //{
            //    leftList.ID.Add(currentVM.Id);
            //}
            //foreach (Pattern2ListVM currentVM in pattern2LeftList.SelectMany(a => a.Children))
            //{
            //    leftList.isSelected.Add(currentVM.IsSelected == true ? 1 : 0);
            //}

            var isAnythingSelectedRight = Pattern2RightTree.GetSelectedIdxs(true).Any();
            var isAnythingSelectedLeft = Pattern2LeftTree.GetSelectedIdxs().Any();


            //(leftList.isSelected.Count() == leftList.ID.Count()) && (rightList.isSelected.Count() == rightList.ID.Count())
            if ((isAnythingSelectedRight) && (isAnythingSelectedLeft))
            {
                var newPattern2Access = new Pattern2Access();

                var result = newPattern2Access.SavePattern2(Pattern2CurrentMenuItem, Pattern2LeftTree.GetSelectedIdxs(),
                    Pattern2RightTree.GetAsDictionary(), Pattern2RightTree.GetAsDictionaryWithDates());

                ParseResult(result);
            }
            else
            {
                CustomMessageBox.Show(
                    "Your changes could not be saved! \n Please select at least one value from both lists.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Pattern2SaveClass GetRightList()
        {
            var rightList = new Pattern2SaveClass();
            if (HierarchyClass != null) GetHeirs(rightList, HierarchyClass);
            return rightList;
        }

        private void GetHeirs(Pattern2SaveClass rightList, ObservableCollection<TreeViewHierarchy> obj)
        {
            foreach (var currentVM in obj)
            {
                rightList.ID.Add(currentVM.Idx);

                int selection;
                if (currentVM.IsSelectedBool == true)
                {
                    selection = 1;
                }
                else if (currentVM.IsSelectedBool == false)
                {
                    selection = 0;
                }
                else
                {
                    selection = Convert.ToInt32(currentVM.IsSelected);
                }
                rightList.isSelected.Add(selection);

                if (currentVM.Children != null)
                {
                    GetHeirs(rightList, currentVM.Children);
                }
            }
        }

        private bool CanDelete(object obj)
        {
            return IsItemSelectedOnBothLists();
        }

        private bool IsItemSelectedOnBothLists()
        {
            var rightList = GetRightList();

            return (Pattern2LeftTree.GetSelectedIdxs().Any() && (rightList.isSelected.Any(a => (a != 0))));
        }

        //private void DeletePattern2(object obj)
        //{
        //    if (IsItemSelectedOnBothLists())
        //    {
        //        var rightList = GetRightList();
        //        var result = RemoveItems(rightList);
        //        ParseResult(result);
        //    }
        //    else ShowItemNotSelectedError();
        //}

        private static void ShowItemNotSelectedError()
        {
            CustomMessageBox.Show(
                "Selected item(s) could not be deleted! \n Please select at least one value from both lists.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void ParseResult(string result)
        {
            var resultXElement = XElement.Parse(result);
            var message = resultXElement.Descendants("Msg").FirstOrDefault().MaybeValue();
            var resetCache = resultXElement.Descendants("ReloadCache").FirstOrDefault().MaybeValue() == "1";

            if (resetCache)
            {
                var listingsAccess = new ListingsAccess();
                listingsAccess.ResetListingsData();
            }

            CustomMessageBox.Show(message, (result.Contains("success") ? "Success" : "Error"), MessageBoxButton.OK,
                (result.Contains("success") ? MessageBoxImage.Information : MessageBoxImage.Error));
        }

        //private string RemoveItems(Pattern2SaveClass rightList)
        //{
        //    var newPattern2Access = new Pattern2Access();
        //    var result = newPattern2Access.DeletePattern2(Pattern2CurrentMenuItem,
        //        Pattern2LeftTree.GetSelectedIdxs(), rightList);
        //    return result;
        //}

        private void CancelPattern2(object obj)
        {
            Pattern2RightTree.ClearTree();

            Pattern2VM.hierarchyClass = HierarchyClass;
            HasData = false;
            CanPattern2Save = false;
        }

        private bool CanApply(object obj)
        {
            return (Pattern2LeftTree.GetAllNodeIdxs().Any());
        }

        private void ApplyPattern2(object obj)
        {
            var arguments = CommonXml.GetBaseArguments("GetList");
            arguments.AddElement("MenuItem_Idx", menuItemIdx);
            arguments.Add(InputConverter.ToList("SelectedItems", "Item_Idx", Pattern2LeftTree.GetSelectedIdxs()));

            Pattern2RightTree.Listings =
                DynamicDataAccess.GetGenericItem<TreeViewModel>(StoredProcedure.AdminPatternList.ApplySelection, arguments).Listings;

            HasData = true;
            CanPattern2Save = true;
        }
    }
}