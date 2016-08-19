using Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.Generic;
using Model.Entity.NPD;

namespace Model.DataAccess
{
    public class NDPAccess
    {
        public string NPDIdx; 
        public XElement UserIdxElement = new XElement("User_Idx", User.CurrentUser.ID);
        public XElement SalesOrgIdxElement = new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID);

        public NDPAccess(string npdIdx = null)
        {
            NPDIdx = npdIdx;
        }

        public Task<NPDDate> GetDefaultFilterDates(bool resetCache)
        {
            XElement arguments = GetBaseNPDArguments("GetFilterDates");
            return DynamicDataAccess.GetGenericItemAsync<NPDDate>(StoredProcedure.NPD.GetDefaultFilterDates, arguments, resetCache);
        }

        public bool SaveUserPreferences(XElement args)
        {
            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.NPD.SaveUserPreferences, args));
        }

        public string ReturnedMessage(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                return null;
            }

            if (task.Result != null)
            {
                var errors = task.Result.Elements("Error").ToArray();
                if (errors.Any())
                {
                    return errors.Select(e => e.Value).FirstOrDefault();
                }
                var warnings = task.Result.Elements("Warning").ToArray();
                if (warnings.Any())
                {
                    return warnings.Select(e => e.Value).FirstOrDefault();
                }
                var success = task.Result.Elements("SuccessMessage").ToArray();
                if (success.Any())
                {
                    return success.Select(e => e.Value).FirstOrDefault();
                }
            }

            return "No message returned";
        }

        public string GetNPDsProc()
        {
            return StoredProcedure.NPD.GetNPDs;
        }

        public Task<XElement> GetNpds(ObservableCollection<NPDStatus> selectedStatusesList, DateTime startDate, DateTime endDate)
        {
            XElement arguments = GetBaseNPDArguments("GetNPDs");

            arguments.AddElement("Start_Date", startDate.Date.ToString("yyyy-MM-dd"));
            arguments.AddElement("End_Date", endDate.Date.ToString("yyyy-MM-dd"));

            XElement statuses = new XElement("Statuses");
            foreach (var s in selectedStatusesList)
            {
                statuses.AddElement("Status_Idx", s.Idx);
            }

            arguments.Add(statuses);

            return WebServiceProxy.CallAsync(StoredProcedure.NPD.GetNPDs, arguments).ContinueWith(t => GetNpdsContinuation(t));
        }

        private XElement GetNpdsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new XElement("Results");

            return task.Result;
        }

        public Task<IEnumerable<ComboboxItem>> GetFilterStatuses(bool resetCache)
        {
            XElement arguments = GetBaseNPDArguments("GetFilterStatuses");
            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.NPD.GetFilterStatuses, arguments, resetCache);
        }

        public string GetUsers(string npdIdx = null)
        {
            return StoredProcedure.NPD.GetUsers;           
        }

        public string GetCustomers(string npdIdx = null)
        {
            return StoredProcedure.NPD.GetCustomers;  
        }

        public Task<XElement> GetNPDProductGrid(string selectedMasterProductIdx = null)
        {
            XElement arguments = new XElement("GetNPDProductGrid");
            arguments.Add(UserIdxElement);

            if (selectedMasterProductIdx != null)
                arguments.AddElement("BasedOnSku_Idx", selectedMasterProductIdx);                
            else
                arguments.AddElement("NPD_Idx", NPDIdx);            

            return WebServiceProxy.CallAsync(StoredProcedure.NPD.GetProductGrid, arguments).ContinueWith(t => GetNPDProductGridContinuation(t));
        }

        public Task<XElement> GetNPDDesignGrid()
        {
            var args = GetBaseNPDArguments("GetGrid");

            return DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.NPD.GetDesignGrid, args);
        }

        private XElement GetNPDProductGridContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new XElement("Results");

            return task.Result;
        }

        public Task<XElement> GetNPDCustomerProductGrid(string selectedMasterProductIdx, string selectedMasterCustomerIdx)
        {
            XElement arguments = new XElement("GetNPDProductGrid");
            arguments.Add(UserIdxElement);
            arguments.AddElement("BasedOnCust_Idx", selectedMasterCustomerIdx);

            if (selectedMasterCustomerIdx != null && selectedMasterProductIdx != null)
            {
                arguments.AddElement("BasedOnSku_Idx", selectedMasterProductIdx);                
            }                
            else            
                arguments.AddElement("NPD_Idx", NPDIdx);            

            return WebServiceProxy.CallAsync(StoredProcedure.NPD.GetCustomerProductGrid, arguments).ContinueWith(t => GetNPDCustomerProductGridContinuation(t));
        }

        private XElement GetNPDCustomerProductGridContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new XElement("Results");

            return task.Result;
        }

        public string GetReplacementProductsProc()
        {
            return StoredProcedure.NPD.GetFilterReplacementProducts;
        }

        public Task<IEnumerable<ComboboxItem>> GetWorkflowStatuses()
        {
            XElement arguments = GetBaseNPDArguments("GetNPDStatuses");

            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.NPD.GetWorkflowStatuses, arguments);
        }

        public bool CopyNPD(List<string> npdIds)
        {
            var args = CommonXml.GetBaseArguments("CopyNPD");
            args.Add(InputConverter.ToList("NPDs", "NPD_Idx", npdIds));

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.NPD.Copy, args));
        }

        public bool RemoveNPD(List<string> npdIds)
        {
            var args = CommonXml.GetBaseArguments("DeleteNPD");
            args.Add(InputConverter.ToList("NPDs", "NPD_Idx", npdIds));

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.NPD.Remove, args));
        }

        public bool ReplaceNPD(string npdIdx, string replacementSkuId)
        {
            var args = CommonXml.GetBaseArguments("ReplaceNPD");
            args.AddElement("ReplaceWith_Sku_Idx", replacementSkuId);
            args.AddElement("NPD_Idx", npdIdx);

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.NPD.Replace, args));
        }

        public string Save(XElement saveNpdArguments)
        {
            saveNpdArguments.Add(UserIdxElement);
            saveNpdArguments.Add(SalesOrgIdxElement);
            saveNpdArguments.AddElement("NPD_Idx", NPDIdx);

            var res = WebServiceProxy.Call(StoredProcedure.NPD.NPDSave, saveNpdArguments);

            var innerXml = res.Element("NPD");

            XElement message = innerXml.Element("Message");
            string returnedIdx = innerXml.Element("NPD_Idx").MaybeValue();

            DisplayMessage(message);

            return returnedIdx;
        }

        private void DisplayMessage(XElement result)
        {
            bool success = result.ToString().ToLower().Contains("success");
            var mess = result.Value;
            MessageBox.Show(mess, (success ? "Success" : "Error"), MessageBoxButton.OK, (success ? MessageBoxImage.Information : MessageBoxImage.Error));
        }

        public XElement GetBaseNPDArguments(string tag = null)
        {
            var args = CommonXml.GetBaseArguments(tag);
            args.AddElement("NPD_Idx", NPDIdx);

            return args;
        }

        #region Forecasting

        public IEnumerable<NPDForecastFactor> GetForecastFactors()
        {
            var args = CommonXml.GetBaseArguments();
            args.AddElement("NPD_Idx", NPDIdx);

            var holderXml = XElement.Parse("<Results><Operation><ID>1</ID><Name>Addition</Name><Operator>+</Operator><IsEnabled>1</IsEnabled><IsSelected>1</IsSelected></Operation><Operation><ID>2</ID><Name>Multiplication</Name><Operator>*</Operator><IsEnabled>1</IsEnabled><IsSelected>0</IsSelected></Operation></Results>");
#if DEBUG
            return holderXml.Elements().Select(n => new NPDForecastFactor(n));
#else
            return DynamicDataAccess.GetGenericEnumerableAsync<NPDForecastFactor>(StoredProcedure.NPD.GetForecastFilters, args).Result;
#endif
        }

        public IEnumerable<ComboboxItem> GetForecasts()
        {
            var args = CommonXml.GetBaseArguments();
            args.AddElement("NPD_Idx", NPDIdx);

            args = XElement.Parse("<DataSourceInput><User_Idx>12</User_Idx><SalesOrg_Idx>1</SalesOrg_Idx><Screen_Idx>14</Screen_Idx><IsROBAppTypeEntry>0</IsROBAppTypeEntry><ColumnCode>FORECAST</ColumnCode></DataSourceInput>");

            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.NPD.GetFiltersGridPopulateDropdowns, args).Result;
        } 

        #endregion

    }
}
