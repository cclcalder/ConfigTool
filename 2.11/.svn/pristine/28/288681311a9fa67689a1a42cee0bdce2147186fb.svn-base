using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Model.DataAccess.Admin;

namespace Model.Entity.Admin
{
    public class AdminApplySelectionList
    {
        private bool m_isThisLeftList;
        public bool isThisTheLeftList
        {
            get { return m_isThisLeftList; }
            set { m_isThisLeftList = value; }
        }

        private string m_id = "Item_Idx";
        private string m_UserName = "Name";
        private string m_isSelected = "IsSelected";
        private string m_parentID = "Parent_Idx";

        public AdminApplySelectionList()
        { }

        public AdminApplySelectionList(XElement element)
        {
            ID = element.GetValue<string>(m_id);
            UserName = element.GetValue<string>(m_UserName);
            IsSelected = element.GetValue<string>(m_isSelected);
            ParentID = element.GetValue<string>(m_parentID);

            if (IsSelected == "1")
            {
                IsSelectedBool = true;
            }
            else
            {
                IsSelectedBool = false;
            }

            //if (Parent == null)
            //{
            //    Parent = new AdminApplySelectionList();
            //    if (isThisTheLeftList == true)
            //    {
            //        Parent = Pattern2Access.GetUserList().Where(p => p.ID == ParentID) as AdminApplySelectionList;
            //    }
            //    else
            //    {
            //        Parent = Pattern2Access.GetApplySelection().Where(p => p.ID == ParentID) as AdminApplySelectionList;
            //    }
            //}
        }

        public string IsSelected {get; set;}
        public string ID { get; set; }
        public string UserName { get; set; }
        public string ParentID { get; set; }

        public bool IsSelectedBool { get; set; }

        public void addToChildren(List<AdminApplySelectionList> child)
        {
            Children.AddRange(child);
        }

        //private AdminApplySelectionList m_parent;
        //public AdminApplySelectionList Parent
        //{
        //    get
        //    {
        //        //if (m_parent == null)
        //        //{
        //        //    if (isThisTheLeftList == true)
        //        //    {
        //        //        m_parent = Pattern2Access.GetUserList().Where(p => p.ID == ParentID) as AdminApplySelectionList;
        //        //    }
        //        //    else
        //        //    {
        //        //        m_parent = Pattern2Access.GetApplySelection().Where(p => p.ID == ParentID) as AdminApplySelectionList;
        //        //    }
        //        //}

        //        return m_parent;
        //    }
        //    set { m_parent = value; }
        //}

        private List<AdminApplySelectionList> m_children;
        public List<AdminApplySelectionList> Children
        {
            get
            {
                if (m_children == null)
                {
                    //isThisTheLeftList = true;
                    if (isThisTheLeftList == true)
                    {
                        m_children = Pattern2Access.GetUserList().Where(p => p.ParentID == ID).ToList();
                    }
                    else
                    {
                        m_children = Pattern2Access.GetApplySelection().Where(p => p.ParentID == ID).ToList();
                    }

                }
                return m_children;
            }
            internal set { m_children = value.ToList(); }
            //get
            //{
            //    return m_children;
            //}
            //set { m_children = value; }
        }

        //#region Children
        ///// <summary>
        ///// Gets or sets the Children of this Customer.
        ///// </summary>
        //private List<ConditionProduct> _Children;
        //public IEnumerable<ConditionProduct> Children
        //{
        //    get
        //    {
        //        if (_Children == null)
        //        {
        //            _Children = new Pattern2Access().GetApplySelection(ParentID); //.Where(p => p.ParentID == ID).ToList(); //ConditionAccess().GetFilterProducts().Where(p => p.ParentId == ID).ToList();

        //        }
        //        return _Children;
                    
        //    }
        //    internal set { _Children = value.ToList(); }

        //}
        //#endregion
     
    }
}
