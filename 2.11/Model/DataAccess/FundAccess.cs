using System;
using System.Collections.ObjectModel;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.Generic;

namespace Model.DataAccess
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Entity.Funds;

    public class FundAccess
    {
        #region GetFund

        public FundDetail GetFund(string FundID)
        {
            var args = CommonXml.GetBaseArguments("GetFund");
            args.Add(new XElement("Fund_Idx", FundID));
            return FundDetail.FromXml(WebServiceProxy.Call(StoredProcedure.Fund.GetFund, args).Element("Fund"));
        }

        #endregion

        #region SaveFund

        public XElement SaveFund(
            FundDetail fundDetails,
            string newComment,
            DropdownItem selectedSubType,
             XElement values,
            string selectedStatusIdx,
            DropdownItem selectedCustomerLevel,
            List<string> selectedCustomerIdxs,
            DropdownItem selectedProductLevel,
            List<string> selectedProductIdxs)
        {

            XElement argument = new XElement("SaveFund");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Fund_Idx", fundDetails.ID));
            argument.Add(new XElement("Fund_Name", fundDetails.Name));
            argument.Add(new XElement("Date_Start", Convert.ToDateTime(fundDetails.Date_Start).ToString("yyyy-MM-dd")));
            argument.Add(new XElement("Date_End", Convert.ToDateTime(fundDetails.Date_End).ToString("yyyy-MM-dd")));
            argument.Add(new XElement("IsParent", fundDetails.IsParent ? "1" : "0"));
            argument.Add(new XElement("Status_Idx", selectedStatusIdx));
            argument.Add(new XElement("CustLevel_Idx", selectedCustomerLevel.ID));
            argument.Add(new XElement("ProdLevel_Idx", selectedProductLevel.ID));
            argument.Add(new XElement("FundSubType_Idx", selectedSubType.ID));

            if (!string.IsNullOrEmpty(newComment))
            {
                argument.Add(new XElement("Comment", newComment));
            }

            argument.Add(InputConverter.ToList("Customers", "Cust_Idx", selectedCustomerIdxs));
            argument.Add(InputConverter.ToList("Products", "Sku_Idx", selectedProductIdxs));

            if (values != null)
            {
                argument.Add(values);
            }

            var results = WebServiceProxy.Call(StoredProcedure.Fund.Save, argument);


            return results;
        }

        #endregion

        #region Types

        public Task<IList<DropdownItem>> GetTypes(string fundID)
        {
            var args = CommonXml.GetBaseArguments("GetTypes");
            args.Add(new XElement("Fund_Idx", fundID));
            return WebServiceProxy.CallAsync(StoredProcedure.Fund.GetTypes, args, DisplayErrors.No)
                .ContinueWith(t => GetTypesContinuation(t));
        }

        private IList<DropdownItem> GetTypesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled) return new List<DropdownItem>();
            return task.Result.Elements("Types").Select(DropdownItem.FromXml).ToList();
        }

        public Task<IList<DropdownItem>> GetSubTypes(string typeID, string fundID)
        {
            var args = CommonXml.GetBaseArguments("GetSubTypes");
            args.Add(new XElement("Type_Idx", typeID));

            if (!fundID.IsEmpty())
            {
                args.Add(new XElement("Fund_Idx", fundID));
            }


            return WebServiceProxy.CallAsync(StoredProcedure.Fund.GetSubTypes, args, DisplayErrors.No)
                .ContinueWith(t => GetSubTypesContinuation(t));
        }

        private IList<DropdownItem> GetSubTypesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled) return new List<DropdownItem>();
            return task.Result.Elements("SubTypes").Select(DropdownItem.FromXml).ToList();
        }

        #endregion

        #region Customers

        public Task<IList<DropdownItem>> GetCustomerLevels(string fundID)
        {
            var args = CommonXml.GetBaseArguments("GetCustLevels");
            args.Add(new XElement("Fund_Idx", fundID));
            return WebServiceProxy.CallAsync(StoredProcedure.Fund.GetCustomerLevels, args, DisplayErrors.No)
                .ContinueWith(t => GetCustomerLevelsContinuation(t));
        }

        private IList<DropdownItem> GetCustomerLevelsContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled) return new List<DropdownItem>();
            return task.Result.Elements("CustLevel").Select(DropdownItem.FromXml).Distinct().ToList();
        }

        public Task<IEnumerable<ComboboxItem>> GetCustomers(string customerLevelIdx, string fundIdx)
        {
            var args = CommonXml.GetBaseArguments("GetCustSelection");
            args.Add(new XElement("CustLevel_Idx", customerLevelIdx));
            args.Add(new XElement("Fund_Idx", fundIdx));
            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(
                StoredProcedure.Fund.GetCustomerLevelItems, args);
        }


        #endregion

        #region Products

        public Task<IList<DropdownItem>> GetProductLevels(string fundID)
        {
            var args = CommonXml.GetBaseArguments("GetProdLevels");
            args.Add(new XElement("Fund_Idx", fundID));
            return WebServiceProxy.CallAsync(StoredProcedure.Fund.GetProductLevels, args, DisplayErrors.No)
                .ContinueWith(t => GetProductLevelsContinuation(t));
        }

        private IList<DropdownItem> GetProductLevelsContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled) return new List<DropdownItem>();
            return task.Result.Elements("ProdLevel").Select(DropdownItem.FromXml).Distinct().ToList();
        }

        public Task<IEnumerable<ComboboxItem>> GetProducts(string productLevelIdx, List<string> custIdxs, string fundIdx)
        {
            var args = CommonXml.GetBaseArguments("GetProdSelection");
            args.Add(new XElement("ProdLevel_Idx", productLevelIdx));
            args.Add(new XElement("Fund_Idx", fundIdx));
            args.Add(InputConverter.ToCustomers(custIdxs));
            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.Fund.GetProductLevelItems, args);
        }


        #endregion

        #region Status

        public string GetStatusProc()
        {
            return StoredProcedure.Fund.GetWorkflowStatuses;
        }

        //public Task<IEnumerable<ExceedraRadComboBoxItem>> GetWorkflowStatuses(string FundID)
        //{
        //    var args = CommonXml.GetBaseArguments("GetWorkflowStatuses");
        //    args.Add(new XElement("Fund_Idx", FundID));
        //    return DynamicDataAccess.GetGenericItemAsync<SingleSelectViewModel>(
        //        StoredProcedure.Fund.GetWorkflowStatuses, args);
        //}

        //private IList<Entity.Generic.Status> GetGenericStatusesContinuation(Task<XElement> task)
        //{
        //    if (task.IsFaulted || task.IsCanceled) return new List<Entity.Generic.Status>();
        //    return task.Result.Elements("Status").Select(Entity.Generic.Status.FromXml).ToList();
        //}

        #endregion

        #region Comments


        /// <summary>
        /// Returns list of added comments for a given fund
        /// </summary>
        /// <param name="fundID"></param>
        /// <returns></returns>
        public Task<IEnumerable<Note>> GetNotes(string fundID)
        {

            XElement argument = new XElement("GetComments");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Fund_Idx", fundID));

            return WebServiceProxy.CallAsync(StoredProcedure.Fund.GetComments, argument, DisplayErrors.No).ContinueWith(t => GetNotesContinuation(t)); ;

        }

        private IEnumerable<Note> GetNotesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null) return new List<Note>();

            return task.Result.Elements("Comment").Select(Note.FromXml).ToList();
        }


        public string AddNote(string fundID, string comment)
        {
            XElement argument = new XElement("AddComment");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Fund_Idx", fundID));
            argument.Add(new XElement("Comment", comment));

            var node = WebServiceProxy.Call(StoredProcedure.Fund.AddComment, argument).Elements().FirstOrDefault();

            return node.Value;
        }


        #endregion

        #region Grids

        public Task<XElement> GetDynamicGridDataAsync(string key, string fundID)
        {
            var args = CommonXml.GetBaseArguments(key);
            args.Add(new XElement("Fund_Idx", fundID));
            return WebServiceProxy.CallAsync(StoredProcedure.Fund.GetDynamicGridData + key, args, DisplayErrors.No)
                .ContinueWith(t => GetFundBalancesContinuation(t));
        }

        public Task<XElement> GetDynamicGridDataAsync(string key, XElement arguments)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Fund.GetDynamicGridData + key, arguments, DisplayErrors.No)
                .ContinueWith(t => GetFundBalancesContinuation(t));
        }


        public XElement GetDynamicGridData(string key, XElement arguments)
        {
            return WebServiceProxy.Call(StoredProcedure.Fund.GetDynamicGridData + key, arguments, DisplayErrors.No);

        }

        public Task<XElement> GetTransferLogGrid(string key, string fundID)
        {
            var args = CommonXml.GetBaseArguments(key);
            args.Add(new XElement("Fund_Idx", fundID));
            args.Add(new XElement("IsTransferLog", "1"));

            return WebServiceProxy.CallAsync(StoredProcedure.Fund.GetDynamicGridData + key, args, DisplayErrors.No)
                .ContinueWith(t => GetFundBalancesContinuation(t));
        }

        #endregion
        
        #region ListPage
        private XElement GetFundBalancesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled) return null;
            return task.Result;
        }

        public Task<XElement> RemoveFund(List<string> Funds)
        {
            var args = CommonXml.GetBaseArguments("DeleteFund");
            args.Add(new XElement("Funds", Funds.Select(c => new XElement("Fund_Idx", c))));
            return WebServiceProxy.CallAsync(StoredProcedure.Fund.Remove, args);
        }

        public Task<XElement> CopyFund(List<string> Funds)
        {
            var args = CommonXml.GetBaseArguments("CopyFund");
            args.Add(new XElement("Funds", Funds.Select(c => new XElement("Fund_Idx", c))));
            return WebServiceProxy.CallAsync(StoredProcedure.Fund.Copy, args);
        }

        #region Funds Transfer

        public string SaveFundstransfer(XElement FundsVMElement, XElement FundsDetailRVMElement)
        {
            var argument = CommonXml.GetBaseArguments("SaveTransfer"); 
            argument.Add(FundsVMElement);
            argument.Add(FundsDetailRVMElement);

            var res = WebServiceProxy.Call(StoredProcedure.Fund.SaveFundTransfer, argument);

            MessageConverter.DisplayMessage(res);

            return res.Value;
        }
         

         


        public string UpdateStatusParentFunds(string[] Ids, string StatusID)
        {
            XElement argument = new XElement("UpdateFundStatus");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Target_Status_Idx", StatusID));
            argument.Add(new XElement("AppType_Idx"));
            argument.Add(new XElement("Funds"));

            foreach (var id in Ids)
            {
                argument.Element("Funds").Add(new XElement("Fund_Idx", id));
            }

            var node = WebServiceProxy.Call(StoredProcedure.Fund.MultipleStatusParentFundUpdate, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        #endregion

        #endregion

        #region Impacts

        public IEnumerable<FundImpact> GetImpacts(XElement arguments)
        {
            return ComboBoxAccess.GetComboboxGenericItems<FundImpact>(StoredProcedure.Fund.GetImpacts, arguments);
        }

        #endregion

        public XElement GetTransferDetails(string FundID)
        {
            var args = CommonXml.GetBaseArguments("GetData");
            args.Add(new XElement("Fund_Idx", FundID));
            return WebServiceProxy.Call(StoredProcedure.Fund.GetFundTransferWindowDetails, args);
        }

        public XElement GetTransferChildFunds(List<string> list, string pId)
        {
            var args = CommonXml.GetBaseArguments("GetData");
            args.Add(new XElement("Parent_Fund_Idx", pId));

            var chX = new XElement("Child_Funds");

            foreach (var l in list.Where(t=>t != null))
            {
                chX.Add(new XElement("Idx", l));
            }
            args.Add(chX);

            
            

            return WebServiceProxy.Call(StoredProcedure.Fund.GetFundTransferWindowChildFunds, args);
        }
    }
}
