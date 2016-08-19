using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Exceedra.Common.Utilities;
using ViewModels;

namespace WPF.ViewModels
{
    public class PromotionHierarchy : INotifyPropertyChanged
    {
        public PromotionHierarchy ()
        { }

        //public HierarchyClass(string folderPath, string fullPath)
        //{
        //    FullPath = fullPath;
        //    FolderLabel = folderPath;
        //    Folders = new List<IHierarchyInterface>();
        //}

        //public string FullPath { get; set; }
        //public string FolderLabel { get; set; }

        //private List<IHierarchyInterface> m_folders;
        //public List<IHierarchyInterface> Folders
        //{ get { return m_folders; } set { m_folders = value; if (Folders == null) { Folders = new List<IHierarchyInterface>(); }; NotifyPropertyChanged(this, vm => vm.Folders); } } 

        public PromotionHierarchy(PromotionProduct prodVM)
        {
            bool? res;

            if (prodVM.IsSelected == "1")
            { res = true; }
            else if (prodVM.IsSelected == "0")
            { res = false; }
            else
            { res = null; }

            IsSelectedBool = res;
            IsSelected = prodVM.IsSelected.ToString();
            IsExpanded = true;
            ID = prodVM.ID;
            UserName = prodVM.DisplayName;
            ParentID = prodVM.ParentId;

            if (Children == null)
            {
                Children = new ObservableCollection<PromotionHierarchy>();
            }

            foreach (var item in prodVM.Children)
            {
                Children.Add(convertListToHierarchy(item));
            }
           thisBackground = new SolidColorBrush(Colors.White);
        }


        public List<PromotionHierarchy> convertListToListHierarchy(List<PromotionProduct> prodVM)
        {
            var t = new List<PromotionHierarchy>();
            foreach (var r in prodVM)
            {
                t.Add( convertListToHierarchy(r));
            }
            return t;
        }

        private PromotionHierarchy convertListToHierarchy(PromotionProduct prodVM)
        {
            PromotionHierarchy thisHierarchyClass = new PromotionHierarchy();

            bool? res;

            if (prodVM.IsSelected == "1")
            { res = true; }
            else if (prodVM.IsSelected == "0")
            { res = false; }
            else
            { res = null; }

            thisHierarchyClass.IsSelectedBool = res;
            thisHierarchyClass.IsSelected = prodVM.IsSelected.ToString();

            thisHierarchyClass.IsSelected2 = prodVM.IsParentNode;

            thisHierarchyClass.ID = prodVM.ID;
            thisHierarchyClass.UserName = prodVM.DisplayName;
            thisHierarchyClass.ParentID = prodVM.ParentId;

            foreach (PromotionProduct element in prodVM.Children)
            {
                if (thisHierarchyClass.Children == null)
                {
                    thisHierarchyClass.Children = new ObservableCollection<PromotionHierarchy>();
                }

                thisHierarchyClass.Children.Add(convertListToHierarchy(element));
            }

            thisHierarchyClass.thisBackground = new SolidColorBrush(Colors.White);
            thisHierarchyClass.StringBackground = "#ffffff";

            return thisHierarchyClass;
        }

        public SolidColorBrush thisBackground
        { get; set; }

        private string m_StringBackground;
        public string StringBackground
        {
            get { return m_StringBackground; }
            set
            {
                m_StringBackground = value;
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

                bool? res;
                if (IsSelected == "1")
                { res = true; }
                else if (IsSelected == "0")
                { res = false; }
                else
                { res = null; }

                IsSelectedBool = res;

               

            }
        }
        public string ID { get; set; }
        public string UserName { get; set; }
        public string ParentID { get; set; }

        private bool? m_isSelectedBool;
        public bool? IsSelectedBool
        {
            get { return m_isSelectedBool; }
            set
            {
                m_isSelectedBool = value;
                PropertyChanged.Raise(this, "IsSelectedBool");
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


        private bool m_isSelected2;
        public bool IsSelected2
        {
            get { return m_isSelected2; }
            set
            {
                m_isSelected2 = value;
                PropertyChanged.Raise(this, "IsSelected2");
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
            IsExpanded = IsAnySelectedInFullPath() || forceAll;

            Children.Do(c => c.PerformExpand(forceAll));
        }



        public bool IsAnySelectedInFullPath()
        {
            if (IsSelectedBool != false || ParentID == null)
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

        public bool IsAnyMatchInFullPath(string[] searchWords)
        {
            if (IsMatch(searchWords))
            {
                StringBackground = "#ffff96";
                return true;
            }
            else
            {
                StringBackground = "#ffffff";
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
            StringBackground = "#ffffff";
            Children.Do(c => c.ClearSearch());
        }

        public bool IsMatch(string[] searchWords)
        {
            return UserName.ContainsAll(searchWords, false);
        }

        private PromotionHierarchy m_parent;
        public PromotionHierarchy Parent
        {
            get { return m_parent; }
            set
            {
                m_parent = value; if (Parent == null) { Parent = new PromotionHierarchy(); };
                PropertyChanged.Raise(this, "Parent");
            }
        }

        private ObservableCollection<PromotionHierarchy> m_children;
        public ObservableCollection<PromotionHierarchy> Children
        {
            get { return m_children; }
            set
            {
                m_children = value; if (Children == null) { Children = new ObservableCollection<PromotionHierarchy>(); };
                PropertyChanged.Raise(this, "Children");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }


 
        }


        private IEnumerable<PromotionHierarchy> GetChildren(PromotionHierarchy parent)
        {
            yield return parent;

            if (parent.Children != null)
            {
                foreach (var relative in parent.Children.SelectMany(GetChildren))
                    yield return relative;
            }
        }

        public IEnumerable<PromotionHierarchy> GetFlatTree()
        {
            var l = GetChildren(this);

            return l.Distinct();
        }
    }
}
