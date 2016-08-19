using Model.DataAccess.Admin;
using Model.Entity;
using Model.Entity.Admin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.DataAccess.Admin
{
    public class Pattern2Access
    {
        private static readonly List<AdminApplySelectionList> CachedRightList = new List<AdminApplySelectionList>();
        private static readonly List<AdminApplySelectionList> CachedLeftList = new List<AdminApplySelectionList>();


        public bool didWebProxyGetCalled = false;
        public string WebProxyReturned = null;
        public string returnCurrentUserID = null;
        public bool itRan = true;

        private const string GetUserIDForFirstList = @"<{0}><User_Idx>{1}</User_Idx><MenuItem_Idx>{2}</MenuItem_Idx></{0}>";
        private const string SendAndGetApplyList = @"<{0}><User_Idx>{1}</User_Idx><MenuItem_Idx>{2}</MenuItem_Idx>{3}</{0}>";
        private const string SaveApplyList = @"<{0}><User_Idx>{1}</User_Idx><MenuItem_Idx>{2}</MenuItem_Idx>{3}{4}</{0}>";
        private const string DeleteApplyList = @"<{0}><User_Idx>{1}</User_Idx><MenuItem_Idx>{2}</MenuItem_Idx>{3}{4}</{0}>";

        private readonly AutoCache<string, UserListData> _customerCache = new AutoCache<string, UserListData>(so => so.ID);

        public static string GetLeftTreeProc()
        {
            return StoredProcedure.AdminPatternList.UserList;
        }

        public Task<IList<AdminApplySelectionList>> GetUserList(string menuItemIdx)
        {
            itRan = true;
            const string getUserList = "GetList";
            string arguments = GetUserIDForFirstList.FormatWith(getUserList, User.CurrentUser.ID, menuItemIdx);

            //Variables used in testing only
            returnCurrentUserID = User.CurrentUser.ID;

            return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.UserList, XElement.Parse(arguments))
                    .ContinueWith(t => GetAdminPatternList(t));            
        }

        public IList<AdminApplySelectionList> GetAdminPatternList(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                return new List<AdminApplySelectionList>();
            }

            //Used only in unit test
            didWebProxyGetCalled = true;
            WebProxyReturned = task.ToString();

            CachedLeftList.Clear();

            CachedLeftList.AddRange(task.Result.Elements().Select(n => new AdminApplySelectionList(n)).ToList());

            foreach (AdminApplySelectionList currentList in CachedLeftList)
            {
                currentList.isThisTheLeftList = true;
            }


            return CachedLeftList;
        }

        public static List<AdminApplySelectionList> GetApplySelection()
        {
            return CachedRightList;
        }

        public static List<AdminApplySelectionList> GetUserList()
        {
            return CachedLeftList;
        }

        public string SavePattern2(string menuItemIdx, List<string> selectedLeftItems, Dictionary<string, string> rightItems, Dictionary<string, string> rightItemsDates = null) 
        {
            var selectedLeftItemsElements = new XElement("List1Items");
            foreach (var item in selectedLeftItems)
            {
                var thisItem = new XElement("Item");
                thisItem.Add(new XElement("Item_Idx", item));
                thisItem.Add(new XElement("IsSelected", "1"));

                selectedLeftItemsElements.Add(thisItem);
            }

            var selectedRightItemsElements = new XElement("List2Items");
            foreach (var item in rightItems)
            {
                var thisItem = new XElement("Item");
                thisItem.Add(new XElement("Item_Idx", item.Key));
                thisItem.Add(new XElement("IsSelected", item.Value));

                if(rightItemsDates != null 
                    && rightItemsDates.ContainsKey(item.Key)
                    && !string.IsNullOrEmpty(rightItemsDates[item.Key]))
                    thisItem.Add(new XElement("Date", rightItemsDates[item.Key]));

                selectedRightItemsElements.Add(thisItem);
            }

            const string Save = "SaveMatches";
            string arguments = SaveApplyList.FormatWith(Save, User.CurrentUser.ID, menuItemIdx, selectedLeftItemsElements, selectedRightItemsElements);

            //Variables used in testing only
            returnCurrentUserID = User.CurrentUser.ID;

            return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.SaveListMatches, XElement.Parse(arguments)).ContinueWith(t => SavePattern2Coninutation(t)).Result; 
        }



        public string SavePattern2Coninutation(Task<XElement> task)
        {

            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
              
                return "No message returned";
            }


            if (task.Result != null)
            {
                var errors = task.Result.Elements("Error").ToArray();
                if (errors.Any())
                {
                    return errors.Select(e => e.Value).FirstOrDefault().ToString();
               
                }
                var warnings = task.Result.Elements("Warning").ToArray();
                if (warnings.Any())
                {
                    return warnings.Select(e => e.Value).FirstOrDefault().ToString();
                }
                var success = task.Result.Elements("SuccessMessage").ToArray();
                if (success.Any())
                {
                    return success.Select(e => e.Value).FirstOrDefault().ToString();
                }
            }


            string messageString = task.Result.ToString();

            return messageString;

        }


        //public string DeletePattern2(string menuItemIdx, List<string> selectedLeftItems, Pattern2SaveClass selectedRightItems)
        //{
        //    var selectedLeftItemsElements = new XElement("List1Items");
        //    foreach (var item in selectedLeftItems)
        //    {
        //        var thisItem = new XElement("Item");
        //        thisItem.Add(new XElement("Item_Idx", item));
        //        thisItem.Add(new XElement("IsSelected", "1"));

        //        selectedLeftItemsElements.Add(thisItem);
        //    }

        //    var selectedRightItemsElements = new XElement("List2Items");
        //    for (int i = 0; i < selectedRightItems.ID.Count(); i++)
        //    {
        //        var thisItem = new XElement("Item");
        //        thisItem.Add(new XElement("Item_Idx", selectedRightItems.ID[i]));
        //        thisItem.Add(new XElement("IsSelected", selectedRightItems.isSelected[i]));

        //        selectedRightItemsElements.Add(thisItem);
        //    }

        //    const string Delete = "DeleteMatches";
        //    string arguments = DeleteApplyList.FormatWith(Delete, User.CurrentUser.ID, menuItemIdx, selectedLeftItemsElements, selectedRightItemsElements);

        //    //Variables used in testing only
        //    returnCurrentUserID = User.CurrentUser.ID;

        //    return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.DeleteListMatches, XElement.Parse(arguments)).ContinueWith(t => DeletePattern2Coninutation(t)).Result;
        //}



        public string DeletePattern2Coninutation(Task<XElement> task)
        {

            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {

                return "No message returned";
            }


            if (task.Result != null)
            {
                var errors = task.Result.Elements("Error").ToArray();
                if (errors.Any())
                {
                    return errors.Select(e => e.Value).FirstOrDefault().ToString();

                }
                var warnings = task.Result.Elements("Warning").ToArray();
                if (warnings.Any())
                {
                    return warnings.Select(e => e.Value).FirstOrDefault().ToString();
                }
                var success = task.Result.Elements("SuccessMessage").ToArray();
                if (success.Any())
                {
                    return success.Select(e => e.Value).FirstOrDefault().ToString();
                }
            }


            string messageString = task.Result.ToString();

            return messageString;

        }


        public Task<IList<AdminApplySelectionList>> GetApplySelection(string menuItemIdx, List<string> selectedItems)
        {
            var selectedItemsElements = new XElement("SelectedItems");
            foreach (var item in selectedItems)
            {
                selectedItemsElements.Add(new XElement("Item_Idx", item));
            }

            const string getUserList = "GetList";
            string arguments = SendAndGetApplyList.FormatWith(getUserList, User.CurrentUser.ID, menuItemIdx, selectedItemsElements);

            //Variables used in testing only
            returnCurrentUserID = User.CurrentUser.ID;


            return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.ApplySelection, XElement.Parse(arguments))
                    .ContinueWith(t => GetApplySelectionList(t));
        }

        public Task<IList<AdminApplySelectionList>> GetApplySelection(string menuItemIdx)
        {
            const string getUserList = "GetList";
            string arguments = GetUserIDForFirstList.FormatWith(getUserList, User.CurrentUser.ID, menuItemIdx);

            //Variables used in testing only
            returnCurrentUserID = User.CurrentUser.ID;


            return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.ApplySelection, XElement.Parse(arguments))
                    .ContinueWith(t => GetApplySelectionList(t));   
        }

        public IList<AdminApplySelectionList> GetApplySelectionList(Task<XElement> task)
        {

            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                return new List<AdminApplySelectionList>();
            }

            //Used only in unit test 
            didWebProxyGetCalled = true;
            WebProxyReturned = task.ToString();

            CachedRightList.Clear();

            CachedRightList.AddRange(task.Result.Elements().Select(n => new AdminApplySelectionList(n)).ToList());

            foreach(AdminApplySelectionList currentList in CachedRightList)
            {
                currentList.isThisTheLeftList = false; 
            }

            return CachedRightList;

        }



        //public Task<IList<SalesOrgData>> GetSalesOrganizations()
        //{
        //    const string getFilterSalesOrganisations = "GetFilterSalesOrganisations";
        //    string arguments = GetByUserIdTemplate.FormatWith(getFilterSalesOrganisations, User.CurrentUser.ID);
        //    return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetSalesOrgs, XElement.Parse(arguments))
        //        .ContinueWith(t => GetSalesOrganizationsContinuation(t));
        //}

        //private IList<SalesOrgData> GetSalesOrganizationsContinuation(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //    { return new List<SalesOrgData>(); }

        //    return task.Result.Elements().Select(n => new SalesOrgData(n)).ToList();
        //}
    }
}
