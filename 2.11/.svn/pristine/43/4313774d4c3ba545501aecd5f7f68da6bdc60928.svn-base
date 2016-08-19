using Model.Entity.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Model.DataAccess.Admin
{
    public class Pattern1Access
    {
        private const string sendUserIdAndMenuItemId = @"<{0}><User_Idx>{1}</User_Idx><MenuItem_Idx>{2}</MenuItem_Idx></{0}>";
        private const string sendUserIdMenuItemIdSelectedItem = @"<{0}><User_Idx>{1}</User_Idx><MenuItem_Idx>{2}</MenuItem_Idx><SelectedItem_Idx>{3}</SelectedItem_Idx></{0}>";
        private const string saveGrid = @"<{0}><{1}><User_Idx>{2}</User_Idx><MenuItem_Idx>{3}</MenuItem_Idx><SelectedItem_Idx>{4}</SelectedItem_Idx>{5}</{1}></{0}>";

        //public Task<List<Pattern1ValidationCode>> GetValidationCode(string menuItemIdx)
        //{
        //    const string getCode = "GetValidationCode";
        //    string arguments = sendUserIdAndMenuItemId.FormatWith(getCode, User.CurrentUser.ID, menuItemIdx);

        //    return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.GetValidationCodes, XElement.Parse(arguments))
        //        .ContinueWith(t => GetValidationList(t));  
        //}

        //public List<Pattern1ValidationCode> GetValidationList(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //    {
        //        return new List<Pattern1ValidationCode>();
        //    }

        //    List<Pattern1ValidationCode> tempList = new List<Pattern1ValidationCode>();
        //    tempList.Add(new Pattern1ValidationCode(task.Result));

        //    return task.Result.Elements().Select(n => new Pattern1ValidationCode(n)).ToList();
        //}

        //public XElement GetGirdXElement(string menuItemIdx)
        //{
        //                const string getGridLocal = "GetGrid";
        //                string arguments = sendUserIdAndMenuItemId.FormatWith(getGridLocal, User.CurrentUser.ID, menuItemIdx);

        //    return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.GetGrid, XElement.Parse(arguments)).Result;
        //}

        public XElement GetGirdXElement(string menuItemIdx, string selectedItemIdx)
        {
            const string getGridLocal = "GetGrid";
            string arguments = sendUserIdMenuItemIdSelectedItem.FormatWith(getGridLocal, User.CurrentUser.ID, menuItemIdx, selectedItemIdx);

            return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.GetGrid, XElement.Parse(arguments)).Result;
        }

        /// <summary>
        /// Will need to change the XML: ASK ED!
        /// </summary>
        /// <param name="menuItemIdx"></param>
        /// <param name="selectedItemsIdx"></param>
        /// <returns></returns>
        public XElement GetGirdXElement(string menuItemIdx, List<string> selectedItemsIdx)
        {
            const string getGridLocal = "GetGrid";
            string arguments = sendUserIdMenuItemIdSelectedItem.FormatWith(getGridLocal, User.CurrentUser.ID, menuItemIdx, selectedItemsIdx);

            return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.GetGrid, XElement.Parse(arguments)).Result;
        }

        //public Task<List<Pattern1Grid>> GetGrid(string menuItemIdx)
        //{
        //    const string getGridLocal = "GetGrid";
        //    string arguments = sendUserIdMenuItemIdSelectedItem.FormatWith(getGridLocal, User.CurrentUser.ID, menuItemIdx);

        //    return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.GetGrid, XElement.Parse(arguments))
        //        .ContinueWith(t => GetGridList(t));
        //}

        //public List<Pattern1Grid> GetGridList(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //    {
        //        return new List<Pattern1Grid>();
        //    }

        //    List<Pattern1Grid> tempList = new List<Pattern1Grid>();
        //    tempList.Add(new Pattern1Grid(task.Result));

        //    return task.Result.Elements().Select(n => new Pattern1Grid(n)).ToList();
        //}

        public Task<List<Pattern1List>> GetLeftList(string menuItemIdx)
        {
            const string getList = "GetList";
            string arguments = sendUserIdAndMenuItemId.FormatWith(getList, User.CurrentUser.ID, menuItemIdx);

            return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.GetListPattern1, XElement.Parse(arguments))
                .ContinueWith(t => GeneratePattern1List(t));

        }

        public List<Pattern1List> GeneratePattern1List(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                return new List<Pattern1List>();
            }

            //List<Pattern1List> tempList = new List<Pattern1List>();
            //tempList.Add(new Pattern1List(task.Result));

            return task.Result.Elements().Select(n => new Pattern1List(n)).ToList();
        }
        
        //public Task<string> CopyGrid(string menuItemIdx, string selectedItemIdx)
        //{
        //    const string copyGrid = "CopyGrid";
        //    string arguments = sendUserIdMenuItemIdSelectedItem.FormatWith(copyGrid, User.CurrentUser.ID, menuItemIdx, selectedItemIdx);

        //    return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.CopyGrid, XElement.Parse(arguments))
        //        .ContinueWith(t => returnedMessage(t));
        //}

        //public string returnedMessage (Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //    {
        //        return null;
        //    }

        //    if (task.Result != null)
        //    {
        //        var errors = task.Result.Elements("Error").ToArray();
        //        if (errors.Any())
        //        {
        //            return errors.Select(e => e.Value).FirstOrDefault().ToString();

        //        }
        //        var warnings = task.Result.Elements("Warning").ToArray();
        //        if (warnings.Any())
        //        {
        //            return warnings.Select(e => e.Value).FirstOrDefault().ToString();
        //        }
        //        var success = task.Result.Elements("SuccessMessage").ToArray();
        //        if (success.Any())
        //        {
        //            return success.Select(e => e.Value).FirstOrDefault().ToString();
        //        }
        //    }

        //    return "No message returned";
        //}

        public XElement SaveGrid(XElement arguments)
        { 
            return WebServiceProxy.Call(StoredProcedure.AdminPatternList.SaveGrid,  arguments);
        }

        public XElement DeleteGrid(XElement arguments)
        {
            return WebServiceProxy.Call(StoredProcedure.AdminPatternList.DeleteGrid, arguments);
        }

        public XElement CopyGrid(XElement arguments)
        {
            return WebServiceProxy.Call(StoredProcedure.AdminPatternList.CopyGrid, arguments);
        }

        //public Task<List<Pattern1Permission>> GetPermissions(string menuItemIdx)
        //{
        //    const string getPermissions = "GetPermissions";

        //    string arguments = sendUserIdAndMenuItemId.FormatWith(User.CurrentUser.ID, getPermissions);

        //    return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.GetPermissions, XElement.Parse(arguments))
        //        .ContinueWith(t => GeneratePattern1PermissionList(t));
        //}

        //public List<Pattern1Permission> GeneratePattern1PermissionList (Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //    {
        //        return new List<Pattern1Permission>();
        //    }

        //    return task.Result.Elements().Select(n => new Pattern1Permission(n)).ToList();
        //}

    }
}
