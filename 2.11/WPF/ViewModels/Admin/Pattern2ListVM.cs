using System;
using System.Collections.Generic;
using System.Linq;
using Coder.WPF.UI;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Exceedra.Common.Utilities;
using Model.DataAccess.Admin;
using Model.Entity.Listings;

namespace Model.Entity.Admin
{
    public class Pattern2ListVM : SearchableNode
    {
        private readonly List<Pattern2ListVM> m_children;
        private readonly AdminApplySelectionList m_adminList;

        public AdminApplySelectionList Product
        {
            get { return m_adminList; }
        }

        public Pattern2ListVM()
        {

        }


        public Pattern2ListVM(ISearchableTreeViewNodeEventsConsumer eventsConsumer, Pattern2ListVM parent,
                                bool isSingleSelection,AdminApplySelectionList AdminList)
            : base(eventsConsumer, isSingleSelection, parent,true)
        {
            IsInitialising = true;
            m_adminList = AdminList;

            m_children = new List<Pattern2ListVM>((from child in m_adminList.Children
                                                             orderby child.UserName
                                                             select 
                                                                new
                                                                        Pattern2ListVM(eventsConsumer, this, isSingleSelection, child 
                                                                        ))

                                                            .ToList<Pattern2ListVM>());

            IsSelected = AdminList.IsSelectedBool;

            IsInitialising = false;
        }

        private static ObservableCollection<AdminApplySelectionList> m_rightListAdminListCollection;
        public ObservableCollection<AdminApplySelectionList> rightListAdminCollection
        {
            get {return m_rightListAdminListCollection;}
            set {m_rightListAdminListCollection = value;}
        }

        private static ObservableCollection<AdminApplySelectionList> m_filteredRightAdminListCollection;
        public ObservableCollection<AdminApplySelectionList> filteredRightAdminListCollection
        {
            get { return m_filteredRightAdminListCollection; }
            set { m_filteredRightAdminListCollection = value; }
        }

        public ObservableCollection<AdminApplySelectionList> ReturnListForFilter()
        {
            rightListAdminCollection = new ObservableCollection<AdminApplySelectionList>();
            List<string> thisString = new List<string>();
            foreach (var element in LeftObservableList)
            {
                thisString.Add(element.ID);
            }

            Pattern2Access pattern2Access = new Pattern2Access();

            foreach (var element in (pattern2Access.GetApplySelection(currentMenuItemIdx, thisString).Result))
            {
                rightListAdminCollection.Add(element);
            }

            if (rightListAdminCollection.Count > 0)
            {

                return rightListAdminCollection; 
            }
            else 
            { return null; }
        }

        private ObservableCollection<TreeViewHierarchy> _hierarchyClass;
        public ObservableCollection<TreeViewHierarchy> hierarchyClass
        { 
            get {return _hierarchyClass;}
            set { _hierarchyClass = value;
                PropertyChanged.Raise(this,"hierarchyClass");
            }
        
        }

        public override IEnumerable<SearchableNode> Children
        {
            get { return m_children; }
        }

        public string Id
        {
            get { return m_adminList.ID; }
        }

        public override string Title
        {
            get { return m_adminList.UserName; }
            set { throw new NotImplementedException(); }
        }

        //public delegate void NoListSelection(Pattern2ListVM unCheck);
        //public event NoListSelection CheckBoxIsUnTicked;

        //public delegate void ListHasSelection(Pattern2ListVM check);
        //public event ListHasSelection CheckBoxIsTicked;


        private bool m_isSomethingChecked;
        public bool IsSomethingChecked
        {
            get { return m_isSomethingChecked; }
            set { m_isSomethingChecked = value; }
        }

        protected override void OnSelectedChanged()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(OnSelectedChanged));
                return;
            }
            m_adminList.IsSelectedBool = IsSelected;
            base.OnSelectedChanged();
            OnProductSelectionChanged(new EventArgs());

            //if (thisVM == null)
            //{
            //    thisVM = new ObservableCollection<Pattern2ListVM>();
            //}
            //if ((thisVM.SelectMany(a => a.SelectedNodes).Count() > 0) && (CheckBoxIsTicked != null))
            //{
            //    IsSomethingChecked = true;
            //    CheckBoxIsTicked(this);
            //}
            //else if (CheckBoxIsTicked != null)
            //{
            //    IsSomethingChecked = false;
            //    CheckBoxIsTicked(this);
            //    //CheckBoxIsUnTicked(this);
            //}

        }

        public event EventHandler<EventArgs> SelectedProductsChanged;

        protected virtual void OnProductSelectionChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectedProductsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static ObservableCollection<AdminApplySelectionList> m_leftObservableList;
        public  ObservableCollection<AdminApplySelectionList> LeftObservableList { get { return m_leftObservableList; } set { m_leftObservableList = value; } }

        private static string m_currentMenuItemIdx;
        public  string currentMenuItemIdx { get { return m_currentMenuItemIdx; } set { m_currentMenuItemIdx = value;} }

        public ObservableCollection<Pattern2ListVM> GetLeftItemList(string menuItemIdx)
        {
            //Task<List<AdminApplySelectionList>> ApplySelectionTaskToTurnIntoList;

            Pattern2Access getPattern2Data = new Pattern2Access();
            currentMenuItemIdx = menuItemIdx;
            var ApplySelectionTaskToTurnIntoList = getPattern2Data.GetUserList(menuItemIdx);
            LeftObservableList = new ObservableCollection<AdminApplySelectionList>();
            foreach (var element in ApplySelectionTaskToTurnIntoList.Result)
            {
                LeftObservableList.Add(element);
            }
            LeftObservableForPattern1 = new ObservableCollection<Pattern2ListVM>();
            LeftObservableForPattern1.AddRange(sortData(ApplySelectionTaskToTurnIntoList.Result as List<AdminApplySelectionList>));
            return LeftObservableForPattern1;
        }

        private ObservableCollection<Pattern2ListVM> m_leftObsrervableForPattern1;
        public ObservableCollection<Pattern2ListVM> LeftObservableForPattern1 { get { return m_leftObsrervableForPattern1; } set { m_leftObsrervableForPattern1 = value; } }

        public ObservableCollection<Pattern2ListVM> GetRightItemList(string menuItemIdx)
        {
            //Task<List<AdminApplySelectionList>> ApplySelectionTaskToTurnIntoList;

            Pattern2Access getPattern2Data = new Pattern2Access();

            var ApplySelectionTaskToTurnIntoList = getPattern2Data.GetApplySelection(menuItemIdx);

            return sortData(ApplySelectionTaskToTurnIntoList.Result as List<AdminApplySelectionList>);
        }

        public  ObservableCollection<Pattern2ListVM> GetApplySelection(string menuItemIdx, List<string> selectedItems)
        {
            Pattern2Access getPattern2Data = new Pattern2Access();

            var ApplySelectionTaskToTurnIntoList = getPattern2Data.GetApplySelection(menuItemIdx, selectedItems);

            return sortData(ApplySelectionTaskToTurnIntoList.Result as List<AdminApplySelectionList>);
        }

        public  ObservableCollection<Pattern2ListVM> sortData(List<AdminApplySelectionList> thisList)
        {
            //foreach (var currentList in thisList)
            //{
            //    if (thisList.Where(p => p.Children == null).Count() > 0)
            //    {
                    //var c  = (from x in currentList where x.)
            //currentList.addToChildren(thisList.Where(p => p.ParentID == ID).ToList());
      
            if (thisTree != null)
            {
                thisVM = new ObservableCollection<Pattern2ListVM>(thisList.Where(p=>p.ParentID == null)
                                                                        .Select(
                                                                             m =>
                                                                             new Pattern2ListVM(thisTree, null,
                                                                                 IsThisSingleSelect, m)));
            }

            return thisVM;
        }

        private bool m_isThisSingleSelect;
        public bool IsThisSingleSelect
        {
            get { return m_isThisSingleSelect; }
            set { m_isThisSingleSelect = value; }
        }

        public ObservableCollection<AdminApplySelectionList> GetTelerikList(string menuItemIdx, List<string> items)
        {
            telerikList = new ObservableCollection<AdminApplySelectionList>();

            Pattern2Access access = new Pattern2Access();

            List<AdminApplySelectionList> thisList = new List<AdminApplySelectionList>();

            thisList = access.GetApplySelection(menuItemIdx,items).Result as List<AdminApplySelectionList>; 

            foreach(AdminApplySelectionList element in thisList)
            {
                telerikList.Add(element);
            }

            rightListAdminCollection = telerikList;
            return telerikList;
        }

        private ObservableCollection<AdminApplySelectionList> m_telerikList;
        public ObservableCollection<AdminApplySelectionList> telerikList
        {
            get { return m_telerikList; }
            set { m_telerikList = value; }
        }

        private  ObservableCollection<Pattern2ListVM> m_thisVM;
        public  ObservableCollection<Pattern2ListVM> thisVM 
        {
            get { return m_thisVM; }
            set { m_thisVM = value; }
        }

        private  ISearchableTreeViewNodeEventsConsumer m_thisTree;
        public  ISearchableTreeViewNodeEventsConsumer thisTree
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
         
        public event PropertyChangedEventHandler PropertyChanged;

     
    }
}
