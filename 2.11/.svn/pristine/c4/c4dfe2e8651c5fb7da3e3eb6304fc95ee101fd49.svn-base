using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.Entity.Demand;
using Model.Entity.Generic;
using Model.Entity.ROBs;
using Status = Model.Entity.ROBs.Status;

namespace Model.DataAccess
{
    public sealed class ScenarioAccess :  IScenarioListAccess
    {
        public XElement UserIdxElement = new XElement("User_Idx", User.CurrentUser.ID);
        public XElement SalesOrgIdxElement = new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID);

        private const string UserIdElement = "User_Idx";
        private const string ScenariosElement = "Results";
        private const string ScenarioIndexElement = "Scen_Idx";
        private const string ItemIndexElement = "Item_Idx";
        private const string ScenarioTypeElement = "Item_Type";
        private const string ScenarioElement = "Scenario";
        private const string GetScenariosTemplate = "<GetScenarios><UserID>{0}</UserID><SalesOrgID>{1}</SalesOrgID><Scen_Type_Idx>{2}</Scen_Type_Idx><Scen_Idx>{3}</Scen_Idx></GetScenarios>";
        private const string GetScenariosLongTemplate = "<GetScenarios><UserID>{0}</UserID><SalesOrgID>{1}</SalesOrgID><Start>{2}</Start><End>{3}</End></GetScenarios>";
        private const string DateFormatWithDashes = "yyyy-MM-dd";
        private const string DateFormatNoDashes = "yyyyMMdd";

        #region GetProcNames

        public string FilterStatusProc { get { return StoredProcedure.Scenarios.GetFilterStatuses; } }
        public string SaveDefaultsProc { get { return StoredProcedure.Scenarios.SaveUserPrefs; } }
        public string FilterDatesProc { get { return StoredProcedure.Scenarios.GetFilterDates; } }
        public string GetScenariosProc()
        {
            return StoredProcedure.Scenarios.GetScenariosLong;
        }
        public string GetLastDateProc()
        {
            /* Just a holder proc until we actually know how the Set Last Closed button should work. */
            return StoredProcedure.Scenarios.LastClosedScenarios;
        }

        #endregion

        public Task<IEnumerable<ComboboxItem>> GetCustomers(string appTypeId, string customerLevelId, string salesOrgId,
            bool dummyVariable, string scenarioId)
        {
            var args = CreateArgs("GetCustLevelItems", appTypeId, scenarioId, salesOrgId);
            args.Add(new XElement("CustLevelID", customerLevelId));
            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(
                StoredProcedure.Scenarios.GetCustLevelItems, args);
        }

        public Task<IList<ScenarioProduct>> GetProducts(string appTypeId, string productLevelId, string salesOrgId, IList<string> customerIds, string scenarioId)
        {
            var args = CreateArgs("GetProdLevelItems", appTypeId, scenarioId, salesOrgId);
            args.Add(new XElement("ProdLevelID", productLevelId));
            var customers = new XElement("Customers");
            if (customerIds != null && customerIds.Count > 0)
            {
                foreach (var d in customerIds)
                    customers.Add(new XElement("CustomerId", d));
            }
            args.Add(customers);
            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetProdLevelItems, args, DisplayErrors.No)
                .ContinueWith(t => GetProductsContinuation(t));
        }

        private static IList<ScenarioProduct> GetProductsContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null)
                return new List<ScenarioProduct>();
            return task.Result.Elements("ProdLevelItems").Select(ScenarioProduct.FromXml).ToList();
        }

        public Task<IEnumerable<ComboboxItem>> GetProducts(string appTypeId, string productLevelId, string salesOrgId, IList<string> customerIds,
            bool dummyVariable, string scenarioId)
        {
            var args = CreateArgs("GetProdLevelItems", appTypeId, scenarioId, salesOrgId);
            args.Add(new XElement("ProdLevelID", productLevelId));
            var customers = new XElement("Customers");
            if (customerIds != null && customerIds.Count > 0)
            {
                foreach (var d in customerIds)
                    customers.Add(new XElement("CustomerId", d));
            }
            args.Add(customers);

            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(
                StoredProcedure.Scenarios.GetProdLevelItems, args);
        }

        private static XElement CreateArgs(string name, string appTypeId, string scenarioId, string salesOrgId)
        {
            var args = new XElement(name);
            args.Add(new XElement("UserID", User.CurrentUser.ID));
            args.Add(new XElement("SalesOrgId", salesOrgId));
            args.Add(new XElement("AppTypeID", appTypeId));
            args.Add(new XElement(ScenarioIndexElement, scenarioId));
            return args;
        }

        public Task<IList<ScenarioData>> GetScenariosLong(string salesOrgId, DateTime start, DateTime end)
        {
            const string format = DateFormatNoDashes;
            string arguments = GetScenariosLongTemplate.FormatWith(User.CurrentUser.ID, salesOrgId, start.ToString(format), end.ToString(format));
            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetScenariosLong, XElement.Parse(arguments)).ContinueWith(t => GetScenariosContinuation(t));
        }

        //public Task<IList<ScenarioData>> GetScenariosShort(string salesOrgId, string scenarioTypeId, int scenarioIdx)
        //{
        //    return GetScenarios(salesOrgId, scenarioTypeId, StoredProcedure.Scenarios.GetScenariosShort, scenarioIdx);
        //}

        public XElement GetScenariosArgs(string salesOrgId, string scenarioTypeId, int scenarioIdx)
        {
            string arguments = GetScenariosTemplate.FormatWith(User.CurrentUser.ID, salesOrgId, scenarioTypeId, scenarioIdx);
            return XElement.Parse(arguments);
        }

        private static IList<ScenarioData> GetScenariosContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null)
                return new List<ScenarioData>();
            return task.Result.Elements("Scenario").Select(x => new ScenarioData(x)).ToList();
        }

        public Task<string> DeleteScenarios(IList<string> scenarioIds)
        {
            const string scenarioAction = "DeleteScenarios";
            return PerformActionOnScenarios(scenarioIds, scenarioAction, StoredProcedure.Scenarios.DeleteScenarios);
        }

        public Task<string> CloseScenarios(IList<string> scenarioIds)
        {
            const string scenarioAction = "CloseScenarios";
            return PerformActionOnScenarios(scenarioIds, scenarioAction, StoredProcedure.Scenarios.CloseScenarios);
        }

        public Task<string> CopyScenarios(IList<string> scenarioIds)
        {
            const string scenarioAction = "CopyScenarios";
            return PerformActionOnScenarios(scenarioIds, scenarioAction, StoredProcedure.Scenarios.CopyScenarios);
        }
        
        public Task<string> LastClosedScenarios(DateTime date)
        {

            var argument = new XElement("UpdateLatestClosed");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("UpdateAfterDate", date.ToString("yyyy-MM-dd")));

            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.LastClosedScenarios, argument)
                .ContinueWith(t => PerformActionOnScenariosContinuation(t));


        }

        public Task<string> CreateWorkingScenario(IList<string> scenarioIds)
        {
            const string scenarioAction = "CreateWorkingScenario";
            return PerformActionOnScenarios(scenarioIds, scenarioAction, StoredProcedure.Scenarios.CreateWorkingScenario);
        }

        private string convertDateToSaveFormat(DateTime? date)
        {
            List<string> dateToReturn = new List<string>();
            dateToReturn.Add(date.Value.Year.ToString());
            string monthString = date.Value.Month.ToString();
            if (monthString.Count() < 2)
            {
                monthString = "0" + monthString.ElementAt(0);
            }
            dateToReturn.Add(monthString);

            string dayString = date.Value.Day.ToString();
            if (dayString.Count() < 2)
            {
                dayString = "0" + dayString.ElementAt(0);
            }
            dateToReturn.Add(dayString);

            var thisString = string.Join("", dateToReturn);

            return thisString;
        }
        
        public Task<string> SaveActiveBudgets(Dictionary<int, bool> Scenarios, string salesOrg, DateTime? startDate, DateTime? endDate, object obj)
        {
            const string scenarioAction = "ActiveBudget";

            var argument = new XElement(scenarioAction);
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("SalesOrg_Idx", salesOrg));
            argument.Add(new XElement("Start_Date", convertDateToSaveFormat(startDate)));
            argument.Add(new XElement("End_Date", convertDateToSaveFormat(endDate)));
            //argument.Add(new XElement("", obj));
            var scenarios = new XElement(ScenariosElement);
            if (Scenarios != null)


                foreach (var d in Scenarios)
                {
                    var scenario = new XElement(ScenarioElement);
                    scenario.Add(new XElement(ScenarioIndexElement, d.Key.ToString()));
                    scenario.Add(new XElement("IsActiveBudget", (d.Value ? "1" : "0")));
                    scenarios.Add(scenario);
                }



            argument.Add(scenarios);

            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.SaveActiveBudgets, argument)
                   .ContinueWith(t => PerformActionOnScenariosContinuation(t));

        }

        private static Task<string> PerformActionOnScenarios(IEnumerable<string> scenarioIds, string scenarioAction, string storedProc)
        {
            var argument = CreateScenarioActionArgument(scenarioIds, scenarioAction);
            return WebServiceProxy.CallAsync(storedProc, argument)
                .ContinueWith(t => PerformActionOnScenariosContinuation(t));
        }

        private static string PerformActionOnScenariosContinuation(Task<XElement> task)
        {
            return task.Result.ToString();
        }

        private static XElement CreateScenarioActionArgument(IEnumerable<string> scenarioIds, string scenarioAction)
        {
            var argument = new XElement(scenarioAction);
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));

            var results = new XElement("Results");
            argument.Add(results);

            var scenarios = new XElement("RootItem");


            if (scenarioIds != null)
                foreach (var d in scenarioIds.Where(r => r != null))
                {
                    scenarios.Add(new XElement(ItemIndexElement, d));
                    scenarios.Add(new XElement(ScenarioTypeElement, "Scenario"));

                    var attributes = new XElement("Attributes");
                    scenarios.Add(attributes);

                    var attribute = new XElement("Attribute");

                    var attr1 = new XElement("ColumnCode", "IsSelected");
                    attribute.Add(attr1);
                    var attr2 = new XElement("Value", "true");
                    attribute.Add(attr2);

                    attributes.Add(attribute);
                }

            results.Add(scenarios);
            return argument;
        }

        private XElement GetScenarioStatusesArgument(string salesOrgId, int scenarioId, string scenarioTypeIdx)
        {
            var argument = new XElement("GetScenarioStatus");
            argument.Add(new XElement(UserIdElement, User.CurrentUser.ID));
            argument.Add(new XElement("SalesOrgID", salesOrgId));
            argument.Add(new XElement("Scen_Idx", scenarioId));
            argument.Add(new XElement("Scen_Type_Idx", scenarioTypeIdx));

            return argument;
        }

        public XElement GetScenarioStatusesArgs(string salesOrgId, int scenarioId, string scenarioTypeIdx)
        {
            var arguments = GetScenarioStatusesArgument(salesOrgId, scenarioId, scenarioTypeIdx);
            return arguments;
        }

        public Task<IList<Status>> GetFundingStatuses(string appTypeId)
        {

            if (appTypeId == "1")
            {
                const string getScenarioStatusTemplate = "<GetPromotionStatuses><UserID>{0}</UserID></GetPromotionStatuses>";
                string arguments = getScenarioStatusTemplate.FormatWith(User.CurrentUser.ID);
                return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetPromotionStatuses, XElement.Parse(arguments), DisplayErrors.No)
                    .ContinueWith(t => GetStatusesContinuation(t));

            }
            else
            {
                var args = CreateArgs("GetFilterStatuses", appTypeId, "0","0");
                return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetFundingStatuses, args, DisplayErrors.No)
                    .ContinueWith(t => GetStatusesContinuation(t));
            }           
        }

        public Task<IList<Status>> GetPromotionStatuses()
        {
            const string getScenarioStatusTemplate = "<GetPromotionStatuses><UserID>{0}</UserID></GetPromotionStatuses>";
            string arguments = getScenarioStatusTemplate.FormatWith(User.CurrentUser.ID);
            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetPromotionStatuses, XElement.Parse(arguments), DisplayErrors.No)
                .ContinueWith(t => GetStatusesContinuation(t));
        }

        private static IList<Status> GetStatusesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null)
                return new List<Status>();
            return task.Result.Elements("Status").Select(Status.FromXml).ToList();
        }

        private IList<ScenarioStatus> GetScenarioStatusesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null)
                return new List<ScenarioStatus>();
            return task.Result.Elements("ScenarioStatus").Select(x => new ScenarioStatus(x)).ToList();
        }

        private XElement GetScenarioTypesArgument(string salesOrgId, int scenarioId)
        {
            var argument = new XElement("GetScenarioTypes");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("SalesOrgID", salesOrgId));
            argument.Add(new XElement("Scen_Idx", scenarioId));

            return argument;
        }

        public XElement GetScenarioTypesArgs(string salesOrgId, int scenarioId)
        {
            var argument = GetScenarioTypesArgument(salesOrgId, scenarioId);
            return argument;
        }

        private static IList<ScenarioType> GetScenarioTypesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null)
                return new List<ScenarioType>();
            return task.Result.Elements("ScenarioTypes").Select(x => new ScenarioType(x)).ToList();
        }

        public Task<ScenarioDataDetails> GetScenarioDetails(int scenarioIndex)
        {
            const string xmlTemplate = "<GetScenarioDetails><UserID>{0}</UserID><Scen_Idx>{1}</Scen_Idx></GetScenarioDetails>";
            string arguments = xmlTemplate.FormatWith(User.CurrentUser.ID, scenarioIndex);
            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetScenarioDetails, XElement.Parse(arguments))
                .ContinueWith(t => GetScenarioDetailsContinuation(t));
        }

        private ScenarioDataDetails GetScenarioDetailsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            return new ScenarioDataDetails(task.Result.Elements().FirstOrDefault());
        }

        public Task<IList<UserData>> GetUsers(string salesOrgId)
        {
            const string xmlTemplate = "<GetUserList><SalesOrgID>{0}</SalesOrgID></GetUserList>";
            string arguments = xmlTemplate.FormatWith(salesOrgId);
            try
            {
                return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetUsers, XElement.Parse(arguments)).ContinueWith(t => GetUsersContinuation(t));

            }
            catch (ExceedraDataException)
            {
                return null;
            }
        }

        private IList<UserData> GetUsersContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null)
                return new List<UserData>();
            return task.Result.Elements("Users").Select(x => new UserData(x)).ToList();
        }

        public Task<IEnumerable<ComboboxItem>> GetUsers(string userId, int scenarioId, string salesOrgId, bool dummyVariable)
        {
            const string xmlTemplate = "<GetUserList><User_Idx>{0}</User_Idx><Scen_Idx>{1}</Scen_Idx><SalesOrg_Idx>{2}</SalesOrg_Idx></GetUserList>";
            string arguments = xmlTemplate.FormatWith(userId, scenarioId, salesOrgId);

            var args = XElement.Parse(arguments);

            return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(
                StoredProcedure.Scenarios.GetUsers, args);

            //try
            //{
            //    return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetUsers, XElement.Parse(arguments)).ContinueWith(t => GetUsersContinuation(t));
            //}
            //catch (ExceedraDataException)
            //{
            //    return null;
            //}
        }

        public Task<IList<ComboboxItem>> GetCustomerLevels(string appTypeId, string salesOrgId, int scenarioId)
        {
            XElement args = new XElement("Scenario_Idx", scenarioId);

            return GetComboboxItems(StoredProcedure.Scenarios.GetCustLevels, "GetCustLevels", args);
        }

        public Task<IList<ComboboxItem>> GetProductLevels(string appTypeId, string salesOrgId, int scenarioId)
        {
            XElement args = new XElement("Scenario_Idx", scenarioId);

            return GetComboboxItems(StoredProcedure.Scenarios.GetProdLevels, "GetProdLevels", args);
        }

        private Task<IList<ComboboxItem>> GetComboboxItems(string proc, string rootTag, XElement additionalData = null)
        {
            XElement arguments = new XElement(rootTag);
            arguments.Add(UserIdxElement);
            arguments.Add(additionalData);

            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetComboboxItemsContinuation(t));
        }

        private IList<ComboboxItem> GetComboboxItemsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ComboboxItem> { new ComboboxItem(XElement.Parse("<Results><Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected></Results>")) };

            return task.Result.Elements().Select(n => new ComboboxItem(n)).ToList();
        }

        public IEnumerable<ScenarioResult> SaveScenario(string scenarioId, string salesOrgId, string scenarioName, string scenarioTypeId,
                                               string scenarioStatusId, string startDay, string endDay,
                                               string customerLevelId, string productLevelId, IEnumerable<string> selectedCustomers,
                                               IEnumerable<string> selectedProducts, IEnumerable<string> selectedPromotions,
                                               IEnumerable<string> selectedFunding, IEnumerable<string> selectedUsers, int BaseScenarioID, bool isActiveBudget)
        {
            var arguments = CreateSaveXml(scenarioId, salesOrgId, scenarioName, scenarioTypeId, scenarioStatusId, startDay, endDay,
                customerLevelId,
                productLevelId, selectedCustomers, selectedProducts, selectedPromotions, selectedFunding, selectedUsers, BaseScenarioID, isActiveBudget);
           var res = WebServiceProxy.Call(StoredProcedure.Scenarios.SaveScenario, arguments) ; //.ContinueWith(t => GetSaveScenarioContinuation(t));

            return res.Elements("Scenario").Select(x => new ScenarioResult(x));
        }

        public Task<IList<Rob>> GetRobs(string appTypeId, IEnumerable<string> statusIDs, IEnumerable<string> customerIDs, IEnumerable<string> productIDs, DateTime? start, DateTime? end, string SalesOrgID, string appTypeID, string name, int scenarioID)
        {
            var args = CreateArgs("GetRobs", appTypeId,"0","0");
            //args.Add(new XElement("UserID", User.CurrentUser.ID));
            //args.Add(new XElement("SalesOrgIdx", SalesOrgID));
            //args.Add(new XElement("Scen_Idx", scenarioID));
            var statuses = new XElement("Statuses");
            foreach (var statusId in statusIDs)
            {
                statuses.Add(new XElement("ID", statusId));
            }
            args.Add(statuses);
            var products = new XElement("Products");
            foreach (var productID in productIDs)
            {
                products.Add(new XElement("ID", productID));
            }
            args.Add(products);
            var customers = new XElement("Customers");
            foreach (var customerId in customerIDs)
            {
                customers.Add(new XElement("ID", customerId));
            }
            args.Add(customers);
            if (!start.HasValue)
            {
                start = new DateTime(1970, 1, 1);
            }
            if (!end.HasValue)
            {
                end = new DateTime(2999, 12, 31);
            }
            args.Add(new XElement("Start", start.ToString(DateFormatWithDashes)));
            args.Add(new XElement("End", end.ToString(DateFormatWithDashes)));
            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetFunding, args, DisplayErrors.No)
                .ContinueWith(t => GetRobsContinuation(t));
        }

        private IList<Rob> GetRobsContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null)
                return new List<Rob>();
            var robs = task.Result.Elements("ROB").Select(Rob.FromGetRobsXml).ToList();
            return robs;
        }

        public Task<Rob> GetRob(string appTypeId, string robId)
        {
            var args = CreateArgs("GetROB", appTypeId,"0","0");
            args.Add(new XElement("RobID", robId));
            return WebServiceProxy.CallAsync(StoredProcedure.ROB.GetRob, args)
                .ContinueWith(t => GetRobContinuation(robId, t));
        }

        public Task<IList<Rob>> GetPromotionData(IEnumerable<string> statusIDs, IEnumerable<string> customerIDs, IEnumerable<string> productIDs, DateTime startDate, DateTime endDate, string salesOrgId, string appTypeID, string name, int scenarioID)
        {
            var argument = new XElement("GetPromotions");
            var userNode = new XElement("UserID");
            userNode.SetValue(User.CurrentUser.ID);
            argument.Add(userNode);

            var statuses = new XElement("Statuses");
            argument.Add(statuses);
            var customers = new XElement("Customers");
            argument.Add(customers);

            var products = new XElement("Products");
            argument.Add(products);

            argument.Add(new XElement("SalesOrgIdx", salesOrgId));
            argument.Add(new XElement("Scen_Idx", scenarioID));
            argument.Add(new XElement("Start", startDate.Date.ToString("yyyy-MM-dd")));
            argument.Add(new XElement("End", endDate.Date.ToString("yyyy-MM-dd")));

            foreach (var s in statusIDs)
                statuses.Add(new XElement("ID", s));

            foreach (var c in customerIDs)
                customers.Add(new XElement("ID", c));

            foreach (var p in productIDs)
                products.Add(new XElement("ID", p));

            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetPromotions, argument, DisplayErrors.No, false)
                .ContinueWith(t => GetPromoContinuation(t, appTypeID, name));

        }

        private IList<Rob> GetPromoContinuation(Task<XElement> task, string appTypeID, string name)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null)
                return new List<Rob>();
            var robs = task.Result.Elements("Promotion").Select(Rob.FromGetPromoDataXml).ToList();

            foreach (var r in robs)
            {
                r.Code = appTypeID + "," + name;
            }

            return robs;
        }

        //public Task<string> GetScenarioEnabledControls(string salesOrgId, int scenarioId)
        //{
        //    var arguments = new XElement("GetEnabledControls");
        //    arguments.Add(new XElement("User_Idx", User.CurrentUser.ID));
        //    arguments.Add(new XElement("SalesOrg_Idx", salesOrgId));
        //    arguments.Add(new XElement(ScenarioIndexElement, scenarioId));
        //    return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetScenarioControlIsEnabled, arguments)
        //        .ContinueWith(t => ScenarioControlsEnabledContinuation(t));
        //}

        //private static string ScenarioControlsEnabledContinuation(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted)
        //        return null;
        //    return task.Result.GetValue<string>(ScenarioStateElement);
        //}

        private Rob GetRobContinuation(string robId, Task<XElement> task)
        {
            if (task.IsCanceled) return null;

            // Just use the Result, if faulted, the exception will be rethrown which is the preferred behaviour here.
            var rob = Rob.FromGetRobXml(task.Result.Element("ROB"));
            rob.ID = robId;
            return rob;
        }

        //private IEnumerable<ScenarioResult> GetSaveScenarioContinuation(Task<XElement> task)
        //{
        //    if (task.IsFaulted || task.IsCanceled) return new List<ScenarioResult>();
        //    return task.Result.Elements("Scenario").Select(x => new ScenarioResult(x));
        //}

        private static XElement CreateSaveXml(string scenarioId, string salesOrgId, string scenarioName, string scenarioTypeId,
                                              string scenarioStatusId, string startDay, string endDay,
                                              string customerLevelId, string productLevelId, IEnumerable<string> selectedCustomers,
                                              IEnumerable<string> selectedProducts, IEnumerable<string> selectedPromotions,
                                              IEnumerable<string> selectedFunding, IEnumerable<string> selectedUsers,
                                                int BaseScenarioID, bool isActiveBudget)
        {
            var argsRoot = new XElement("SaveScenarioDetails");
            var args = new XElement("Scenario");
            argsRoot.Add(args);
            args.Add(new XElement("User_Idx", User.CurrentUser.ID));
            args.Add(new XElement("BaseTemplateScen_Idx", BaseScenarioID));
            args.Add(new XElement("SalesOrg_Idx", salesOrgId));
            args.Add(new XElement("Scen_Idx", scenarioId));
            args.Add(new XElement("Scen_Name", scenarioName));
            args.Add(new XElement("Scen_Type_Idx", scenarioTypeId));
            args.Add(new XElement("Scen_Status_Idx", scenarioStatusId));
            args.Add(new XElement("Start_Day_Idx", startDay));
            args.Add(new XElement("IsActiveBudget", (isActiveBudget ? "1" : "0")));
            args.Add(new XElement("End_Day_Idx", endDay));
            args.Add(new XElement("Cust_Level_Idx", customerLevelId));
            args.Add(new XElement("Prod_Level_Idx", productLevelId));
            args.Add(new XElement("SelectedCustomers", selectedCustomers.Select(c => new XElement("Cust_Idx", c))));
            args.Add(new XElement("SelectedProducts", selectedProducts.Select(c => new XElement("Prod_Idx", c))));
            args.Add(new XElement("SelectedPromotions", selectedPromotions.Select(c => new XElement("Promo_Idx", c))));
            args.Add(new XElement("SelectedROBS", selectedFunding.Select(c => new XElement("ROB_Idx", c))));
            args.Add(new XElement("SelectedUsers", selectedUsers.Select(c => new XElement("User_Idx", c))));
            return argsRoot;
        }


        //Get any ROBs for this scenario
        public Task<IList<ApplicableRobResult>> GetApplicableRobs(int SalesOrgID)
        {
            const string xmlTemplate = "<GetApplicableROBs><User_Idx>{0}</User_Idx><SalesOrg_Idx>{1}</SalesOrg_Idx></GetApplicableROBs>";
            string arguments = xmlTemplate.FormatWith(User.CurrentUser.ID, SalesOrgID);

            return WebServiceProxy.CallAsync(StoredProcedure.Scenarios.GetApplicableROBs, XElement.Parse(arguments))
                .ContinueWith(t => GetApplicableRobsContinuation(t));

        }

        private IList<ApplicableRobResult> GetApplicableRobsContinuation(Task<XElement> task)
        {

            //<Results>
            //    <ApplicableROBs>
            //        <AppType_Idx>41</AppType_Idx>
            //        <AppType_Name>Funds</AppType_Name>
            //    </ApplicableROBs>
            //    <ApplicableROBs>
            //        <AppType_Idx>42</AppType_Idx>
            //        <AppType_Name>Terms</AppType_Name>
            //    </ApplicableROBs>
            //</Results>

            if (task.IsCanceled) return null;
            var robs = task.Result.Elements("ApplicableROBs").Select(x => new ApplicableRobResult(x)).ToList();
            robs.Add(new ApplicableRobResult() { ID = "1", Name = "Promotions" });
            return robs;
        }


        public string AddNote(string fundID, string comment)
        {
            XElement argument = new XElement("AddComment");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Scenario_Idx", fundID));
            argument.Add(new XElement("Comment", comment));

            var node = WebServiceProxy.Call(StoredProcedure.Scenario.AddComment, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        public Task<IEnumerable<Note>> GetNotes(string fundID)
        {

            XElement argument = new XElement("GetComments");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Scenario_Idx", fundID));

            return WebServiceProxy.CallAsync(StoredProcedure.Scenario.GetComments, argument, DisplayErrors.No).ContinueWith(t => GetNotesContinuation(t)); ;

        }

        private IEnumerable<Note> GetNotesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null) return new List<Note>();

            return task.Result.Elements("Comment").Select(Note.FromXml).ToList();
        }
    }
}
