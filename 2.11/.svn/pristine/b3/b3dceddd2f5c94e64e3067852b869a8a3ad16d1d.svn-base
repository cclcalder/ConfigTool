using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.Entity;
using System.Diagnostics;
using System.Windows;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.Generic;
using Exceedra.Common.Mvvm;
using Comment = Model.Entity.ROBs.Comment;
using Exceedra.Common;

namespace Model.DataAccess
{
    public class ConditionAccess
    {
        #region consts

        public static XElement UserIdxElement = new XElement("User_Idx", User.CurrentUser.ID);
        public static XElement SalesOrgIdxElement = new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID);

        private const string GetByUserIdTemplate = @"<{0}><UserID>{1}</UserID></{0}>";
        private const string GetReasonsTemplate = @"<{0}><UserID>{1}</UserID><ConditionId>{2}</ConditionId></{0}>";
        private const string GetItemsByUserIdTemplate = @"<{0}><UserID>{1}</UserID><{2}>{3}</{2}></{0}>";
        private const string CustomersTemplate = @"<{0}><User_Idx>{1}</User_Idx><SalesOrg_Idx>{2}</SalesOrg_Idx></{0}>";
        private const string GetItemsByUserIdSalesOrgTemplate = @"<{0}><UserID>{1}</UserID><{2}>{3}</{2}><{4}>{5}</{4}></{0}>";
        private const string GetConditionTemplate = @"<GetCondition><UserId>{0}</UserId><ConditionId>{1}</ConditionId></GetCondition>";
        private const string GetConditionStatusTemplate = @"<GetConditionStatuses><UserId>{0}</UserId><Cond_Idx>{1}</Cond_Idx><Scen_Idx>{2}</Scen_Idx></GetConditionStatuses>";
        private const string GetConditionCommentsTemplate = @"<GetConditionComment><Cond_User_Idx>{0}</Cond_User_Idx><Cond_Idx>{1}</Cond_Idx></GetConditionComment>";
        private const string GetScenariosTemplate = "<GetConditionScenarios><UserId>{0}</UserId><SalesOrgId>{1}</SalesOrgId><Cond_Idx>{2}</Cond_Idx></GetConditionScenarios>";
        private const string GetConditionApplyToScenariosTemplate = "<GetConditionApplyToScenarios><UserId>{0}</UserId><SalesOrgId>{1}</SalesOrgId><StartDayId>{2}</StartDayId><EndDayId>{3}</EndDayId><Cond_Idx>{4}</Cond_Idx></GetConditionApplyToScenarios>";

        private const string GetProductLevelItemsTemplate = @"<{0}><UserID>{1}</UserID><SalesOrgId>{2}</SalesOrgId><ProdLevelID>{3}</ProdLevelID><Customers>{4}</Customers></{0}>";
        
        private const string SaveConditionElement = "SaveCondition";
        private const string CondStateElement = "State";
        private const string CondIdxElement = "Cond_Idx";
        private const string CustIdxElement = "Cust_Idx";
        private const string ProdIdxElement = "Prod_Idx";
        private const string CondUserIdxElement = "Cond_User_Idx";
        private const string OutcomeElement = "Outcome";
        private const string CondSalesorgIdxElement = "Cond_SalesOrg_Idx";
        private const string CondStartDateElement = "Cond_Start_Date";
        private const string CondEndDateElement = "Cond_End_Date";
        private const string CustomerIdsElement = "CustomerIDs";
        private const string CondStatusesElement = "Cond_Statuses";
        private const string DateFormatNoHyphens = "yyyyMMdd";
        private const string CondStatusIdxElement = "Cond_Status_Idx";
        private const string CustLevelIdxElement = "CustLevel_Idx";
        private const string ProdLevelIdxElement = "ProdLevel_Idx";
        private const string CondReasonIdxElement = "Cond_Reason_Idx";
        private const string SkuCustMeasureIdxElement = "SkuCustMeasure_Idx";
        private const string CondNameElement = "Cond_Name";
        private const string CondNewPriceElement = "NewPrice";
        private const string CondShowChildSelectionElement = "ShowChildSelections";
        private const string ProductIdsElement = "ProductIDs";
        private const string ScenarioIdsElement = "ScenarioIDs";
        private const string ScenIdxElement = "Scen_Idx";
        private const string CustIdxElementNoUnderscore = "CustIdx";
        private const string ProdIdxElementNoUnderscore = "ProdIdx";
        private const string StartDateElement = "StartDate";
        private const string EndDateElement = "EndDate";
        private const string OriginalStartDateElement = "Original_Start_Date";
        private const string OriginalEndDateElement = "Original_End_Date";
        private const string OldConditionElement = "OldCondition";
        private const string NewConditionElement = "NewCondition";
        private const string MeasuresElement = "Measures";
        private const string DateFormatWithHyphens = "yyyy-MM-dd";
        private const string UserIdElement = "UserId";
        private const string ConditionIdElement = "ConditionId";
        private const string CommentTextElement = "CommentText";
        private const string AddConditionCommentElement = "AddConditionComment";
        private const string ChangeFixedElement = "ChangeFixed";
        private const string ChangeDeltaElement = "ChangeDelta";
        private const string ChangePercentageElement = "ChangePercentage";
        private const string ValidationResultElement = "ValidationResult";
        private const string ValidationMessageElement = "ValidationMessage";
        private const string ConditionsElement = "Conditions";
        private const string GetFilterStatusesElement = "GetFilterStatuses";
        private const string DeleteConditionElement = "MarkedForDeletion";
        private const string ConditionTypeIndicator = "ConditionTypeIndicator";

        private readonly AutoCache<string, Customer> _customerCache = new AutoCache<string, Customer>(so => so.ID);

        #endregion

        public string GetFilterStatusesProc()
        {
            return StoredProcedure.Conditions.GetFilterStatuses;
        }

        public Task<IList<ComboboxItem>> GetConditionStatuses(string conditionId, string scenarioIdx)
        {
            var arguments = GetConditionStatusTemplate.FormatWith(User.CurrentUser.ID, conditionId, scenarioIdx);
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetConditionStatuses, XElement.Parse(arguments))
                    .ContinueWith(t => GetConditionStatusesContinuation(t));
        }

        private static IList<ComboboxItem> GetConditionStatusesContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ComboboxItem>();
            return task.Result.Elements().Select(e => new ComboboxItem(e)).ToList();
        }

        public Task<IList<ComboboxItem>> GetReasons(string conditionId)
        {
            const string getConditionReasons = "GetConditionReasons";
            var arguments = GetReasonsTemplate.FormatWith(getConditionReasons, User.CurrentUser.ID, conditionId);
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetConditionReasons, XElement.Parse(arguments))
                    .ContinueWith(t => GetReasonsContinuation(t));
        }

        private static IList<ComboboxItem> GetReasonsContinuation(Task<XElement> task)
        {
            const string conditionReasons = "ConditionReasons";
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ComboboxItem>();
            return task.Result.MaybeElement(conditionReasons).Elements().Select(e => new ComboboxItem(e)).ToList();
        }

        public Task<IList<ComboboxItem>> GetCustomerLevels(string conditionId)
        {
            var args = new XElement("Condition_Idx", conditionId);
            
            return GetComboboxItems(StoredProcedure.Conditions.GetCustomerLevels, "GetCustLevels", args);
        }
        
        public Task<IList<ComboboxItem>> GetProductLevels(string conditionId)
        {
            var args = new XElement("Condition_Idx", conditionId);

            return GetComboboxItems(StoredProcedure.Conditions.GetProductLevels, "GetProdLevels", args);
        }

        private Task<IList<ComboboxItem>> GetComboboxItems(string proc, string rootTab, XElement additionalData)
        {
            XElement arguments = new XElement(rootTab);
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

        public Task<IList<ComboboxItem>> GetCustomerLevelItems(string customerLevelId, string salesOrgIDx = null, string conditionId = null)
        {
            const string getCustLevelItems = "GetCustLevelItems";
            const string custLevelId = "CustLevelID";
            const string salesOrgId = "SalesOrgId";
            string arguments;
            if (salesOrgIDx == null)
                arguments = GetItemsByUserIdTemplate.FormatWith(getCustLevelItems, User.CurrentUser.ID, custLevelId, customerLevelId);
            else
                arguments = GetItemsByUserIdSalesOrgTemplate.FormatWith(getCustLevelItems, User.CurrentUser.ID, custLevelId, customerLevelId, salesOrgId, salesOrgIDx);

            var args = XElement.Parse(arguments);
            if(conditionId != null)
                args.AddElement("Condition_Idx", conditionId);

            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetCustomerLevelItems, args)
                .ContinueWith(t => GetCustomerLevelItemsContinuation(t));
        }

        private static IList<ComboboxItem> GetCustomerLevelItemsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ComboboxItem> { new ComboboxItem(XElement.Parse("<Results><Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected></Results>")) };

            return task.Result.Elements().Select(n => new ComboboxItem(n)).ToList();
        }

        //private const string GetProductLevelItemsTemplate = @"<{0}><UserID>{1}</UserID><SalesOrgId>{2}</SalesOrgId><ProdLevelID>{3}</ProdLevelID><Customers>{4}</Customers></{0}>";
        //private const string CustomerTemplate = @" <CustomerId>{0}</CustomerId>";

        public Task<IList<ComboboxItem>> GetProductLevelItems(string productLevelId, IEnumerable<string> customerIds, string salesOrgID, string conditionId = null, string scenarioId = null)
        {
            const string getProdLevelItems = "GetProdLevelItems";
            string customers = string.Join("", (from c in customerIds select new XElement("CustomerId", c)));
            string arguments = GetProductLevelItemsTemplate.FormatWith(getProdLevelItems, User.CurrentUser.ID, salesOrgID, productLevelId, customers);

            var args = XElement.Parse(arguments);

            if (conditionId != null)
                args.AddElement("Condition_Idx", conditionId);

            if (!string.IsNullOrEmpty(scenarioId))
                args.AddElement("Scenario_Idx", scenarioId);

            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetProductLevelItems, args)
                .ContinueWith(t => GetProductLevelItemsContinuation(t));
        }

        public Task<ConditionMeasures> GetMeasures(ConditionType conditionType, DateTime start, DateTime end, IEnumerable<ComboboxItem> customers, IEnumerable<ComboboxItem> products, string salesOrgID, bool showChildSelections, string conditionId, string scenID)
        {
            const string getConditionSelection = "GetConditionSelection";
            var parameters = new XElement(getConditionSelection, GetMeasureParameters(conditionType, start, end, customers, products, showChildSelections, salesOrgID, scenID, conditionId));
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetConditionSelection, parameters)
                .ContinueWith(t => GetMeasuresContinuation(t));
        }

        private static IEnumerable<XElement> GetMeasureParameters(ConditionType conditionType, DateTime start, DateTime end,
            IEnumerable<ComboboxItem> customers, IEnumerable<ComboboxItem> products,
            bool showChildSelections, string salesOrgID, string scenID, string conditionId = null)
        {
            var results = new[] 
            {
                string.IsNullOrEmpty(conditionId) ? null : new XElement(CondIdxElement, conditionId),
                new XElement(CondUserIdxElement, User.CurrentUser.ID),
                new XElement(SkuCustMeasureIdxElement, conditionType.Idx),
                new XElement(ConditionTypeIndicator, conditionType.ConditionTypeIndicator),
                new XElement(CondStartDateElement, start.ToString(DateFormatWithHyphens)),
                new XElement(CondEndDateElement, end.ToString(DateFormatWithHyphens)),
                new XElement(CondShowChildSelectionElement, showChildSelections?"1":"0"), 
                new XElement(CustomerIdsElement, customers.Select(c => new XElement(CustIdxElement, c.Idx))),
                new XElement(ProductIdsElement, products.Select(p => new XElement(ProdIdxElement, p.Idx))),
                SalesOrgIdxElement,
                new XElement(ScenIdxElement, scenID)
            };
            return results.Where(r => r != null);
        }

        public Task<IList<ConditionMeasure>> GetConditionMissingPrices(ConditionType conditionType, DateTime start, DateTime end, string customerLevel1Id, string productLevel1Id, IEnumerable<string> customerIdxs, IEnumerable<string> productIdxs, string conditionId, string defaultPrice, bool showChildSelections, int ScenarioID, int SalesOrgID)
        {
            const string getConditionMissingPrices = "GetConditionMissingPrices";
            var parameters = new XElement(getConditionMissingPrices, GetConditionMissingPricesParameters(conditionType, start, end, customerLevel1Id, productLevel1Id, customerIdxs, productIdxs, conditionId, defaultPrice, showChildSelections, ScenarioID, SalesOrgID));
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetConditionMissingPrices, parameters)
                .ContinueWith(t => GetConditionMissingPricesContinuation(t));
        }

        public Task<bool> GetConditionControlsEnabled(string salesOrgId, string conditionId)
        {
            const string getEnabledControls = "GetEnabledControls";
            var arguments = new XElement(getEnabledControls,
                UserIdxElement,
                SalesOrgIdxElement,
                new XElement(CondIdxElement, conditionId));
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetConditionControlIsEnabled, arguments)
                .ContinueWith(t => ConditionControlsEnabledContinuation(t));
        }

        private static bool ConditionControlsEnabledContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return false;
            return false;
            return task.Result.GetValue<int>(CondStateElement) == 1;
        }

        private static IEnumerable<XElement> GetConditionMissingPricesParameters(ConditionType conditionType, DateTime start, DateTime end, string customerLevel1Id, string productLevel1Id, IEnumerable<string> customerIdxs, IEnumerable<string> productIdxs, string conditionId, string newPrice, bool showChildSelections,
            int ScenarioID, int SalesOrgID)
        {
            var results = new[] 
            {
                string.IsNullOrEmpty(conditionId) ? null : new XElement(CondIdxElement, conditionId),
                new XElement(CondUserIdxElement, User.CurrentUser.ID),
                new XElement(SkuCustMeasureIdxElement, conditionType.Idx),
                new XElement("ConditionTypeIndicator", conditionType.ConditionTypeIndicator),
                new XElement(CondStartDateElement, start.ToString(DateFormatWithHyphens)),
                new XElement(CondEndDateElement, end.ToString(DateFormatWithHyphens)),
                new XElement(CondNewPriceElement, newPrice),
                new XElement(CondShowChildSelectionElement,showChildSelections),
                new XElement(CustLevelIdxElement, customerLevel1Id), 
                new XElement(ProdLevelIdxElement, productLevel1Id), 
                new XElement(InputConverter.ToList(CustomerIdsElement, CustIdxElement, customerIdxs)),
                new XElement(InputConverter.ToList(ProductIdsElement, ProdIdxElement, productIdxs)),
                new XElement("Scen_Idx", ScenarioID), 
                new XElement("SalesOrg_Idx", SalesOrgID)
            };
            

            return results.Where(r => r != null);
        }

        public Task<ValidationResult> Validate(string conditionId, string customerLevel1Id, string productLevel1Id,
            string salesOrgId, string statusId, string reasonId, string conditionTypeId, string name, DateTime startDate, DateTime endDate,
            IEnumerable<string> customerIds, IEnumerable<string> productIds, string scenarioIds,
            IEnumerable<ConditionMeasure> measures, string percentChange, string absoluteChange, string newValue,
            bool showChildSelections, IEnumerable<string> comments, string conditionTypeIndicator)
        {
            var arguments = new XElement(SaveConditionElement,
                GetSaveParameters(conditionId, customerLevel1Id, productLevel1Id, salesOrgId,
                    statusId, reasonId, conditionTypeId, name, startDate, endDate,
                    customerIds, productIds, scenarioIds, measures, percentChange, absoluteChange, newValue, showChildSelections,
                    comments, conditionTypeIndicator));
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.ValidateCondition, arguments)
                .ContinueWith(t => ValidateContinuation(t));
        }

        private static ValidationResult ValidateContinuation(Task<XElement> task)
        {
            const string successOutcome = "1";
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                const string theValidationFailed = "The validation failed.";
                return new ValidationResult(ValidationStatus.Error, theValidationFailed);
            }

            var outcomeElement = task.Result.Element(OutcomeElement);
            var outcome = outcomeElement.GetValue<string>(ValidationResultElement);
            var message = outcomeElement.GetValue<string>(ValidationMessageElement);
            return outcome.Equals(successOutcome)
                ? new ValidationResult(ValidationStatus.Success, message)
                : new ValidationResult(ValidationStatus.Error, message);
        }

        public Task<string> Save(string conditionId, string customerLevel1Id, string productLevel1Id,
            string salesOrgId, string statusId, string reasonId, string conditionTypeId, string name, DateTime startDate, DateTime endDate,
            IEnumerable<string> customerIds, IEnumerable<string> productIds, string scenarioIds,
            IEnumerable<ConditionMeasure> measures, string percentChange, string absoluteChange, string newValue,
            bool showChildSelections, IEnumerable<string> comments, string conditionTypeIndicator)
        {
            var arguments = new XElement(SaveConditionElement,
                GetSaveParameters(conditionId, customerLevel1Id, productLevel1Id, salesOrgId,
                    statusId, reasonId, conditionTypeId, name, startDate, endDate,
                    customerIds, productIds, scenarioIds, measures, percentChange, absoluteChange, newValue, showChildSelections, comments, conditionTypeIndicator));
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.SaveCondition, arguments)
                .ContinueWith(t => SaveContinuation(t));
        }

        private static string SaveContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return "0";
            return task.Result.GetValue<string>(CondIdxElement);
        }

        private static IList<ComboboxItem> GetProductLevelItemsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ComboboxItem> { new ComboboxItem(XElement.Parse("<Results><Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected></Results>")) };

            return task.Result.Elements().Select(n => new ComboboxItem(n)).ToList();
        }


        private static IEnumerable<XElement> GetSaveParameters(string conditionId, string customerLevel1Id, string productLevel1Id,
            string salesOrgId, string statusId, string reasonId, string conditionTypeId, string name, DateTime startDate, DateTime endDate,
            IEnumerable<string> customerIds, IEnumerable<string> productIds, string scenarioIds,
            IEnumerable<ConditionMeasure> measures, string percentChange, string absoluteChange, string newValue,
            bool showChildSelections, IEnumerable<string> comments, string conditionTypeIndicator)
        {
            var parameters = new List<XElement>
            {
                new XElement(CondUserIdxElement, User.CurrentUser.ID), 
                new XElement(CondIdxElement, conditionId), 
                new XElement(CustLevelIdxElement, customerLevel1Id), 
                new XElement(ProdLevelIdxElement, productLevel1Id), 
                SalesOrgIdxElement, 
                new XElement(CondStatusIdxElement, statusId), 
                reasonId == null ? null : new XElement(CondReasonIdxElement, reasonId), 
                new XElement(SkuCustMeasureIdxElement, conditionTypeId), 
                new XElement(CondNameElement, name), 
                new XElement(CondShowChildSelectionElement, showChildSelections?"1":"0"), 
                new XElement(CondStartDateElement, startDate.ToString(DateFormatNoHyphens)), 
                new XElement(CondEndDateElement, endDate.ToString(DateFormatNoHyphens)), 
                new XElement(CustomerIdsElement, customerIds.Select(id => new XElement(CustIdxElement, id))), 
                new XElement(ProductIdsElement, productIds.Select(id => new XElement(ProdIdxElement, id))), 
                new XElement(ScenarioIdsElement, new XElement(ScenIdxElement, scenarioIds)),
                string.IsNullOrEmpty(percentChange) ? null : new XElement(ChangePercentageElement, percentChange),
                string.IsNullOrEmpty(absoluteChange) ? null : new XElement(ChangeDeltaElement, absoluteChange),
                string.IsNullOrEmpty(conditionTypeIndicator) ? null : new XElement(ConditionTypeIndicator , conditionTypeIndicator)
            };

            parameters.AddRange(measures.Select(m =>
                new XElement(MeasuresElement,
                    new XElement(CustIdxElementNoUnderscore, m.CustomerId),
                    new XElement(ProdIdxElementNoUnderscore, m.ProductId),
                    new XElement(StartDateElement, m.StartDate),
                    new XElement(EndDateElement, m.EndDate),
                    new XElement(OriginalStartDateElement, m.OriginalStartDate),
                    new XElement(OriginalEndDateElement, m.OriginalEndDate),
                    new XElement(OldConditionElement, m.OldValue),
                    new XElement(NewConditionElement, m.NewValue),
                    new XElement(DeleteConditionElement, m.MarkedForDeletion ? "1" : "0")
                    )));

            if (conditionId == "0" || conditionId == "-1")
            {
                parameters.AddRange(comments.Select(c =>
                    new XElement("Comments", new XElement("Comment", c))));
            }

            return parameters.Where(e => e != null);
        }

        private static ConditionMeasures GetMeasuresContinuation(Task<XElement> task)
        {
            var measureElements = task.Result.MaybeElements().ToList();
            var firstMeasureElement = measureElements.FirstOrDefault();
            //const string displayColumnsAttribute = "DisplayColumns";
            //const char comma = ',';

            try
            {
                var m = new ConditionMeasures();
                m.Measures = measureElements.Select(ConditionMeasure.FromXml).ToList();
                m.DisplayColumns = null;// firstMeasureElement == null ? new List<string>() : firstMeasureElement.Attribute(displayColumnsAttribute).Value.Split(comma).Select(c => c.Trim()).ToList();

                return m;
            }
            catch// (Exception ex)
            {
                return null;
            }
        }

        public Task<XElement> GetConditionTypes()
        {
            const string getProdLevels = "GetConditionTypes";
            string arguments = GetByUserIdTemplate.FormatWith(getProdLevels, User.CurrentUser.ID);
            return DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Conditions.GetConditionTypes,
                XElement.Parse(arguments));               
        }

        public Task<string> AddComment(string conditionId, string commentText)
        {
            var arguments = new XElement(AddConditionCommentElement,
                new XElement(UserIdElement, User.CurrentUser.ID),
                new XElement(ConditionIdElement, conditionId),
                new XElement(CommentTextElement, commentText));
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.AddComment, arguments)
                .ContinueWith(t => AddCommentContinuation(t));
        }


        //public string GetSaveDefaultsProc()
        //{
        //    return StoredProcedure.Conditions.SaveUserPreferences;
        //}

        public Task<IEnumerable<ComboboxItem>> GetScenarios(string salesOrgId, string conditionID)
        {

            string arguments = GetScenariosTemplate.FormatWith(User.CurrentUser.ID, salesOrgId, conditionID);
            return
                DynamicDataAccess.GetDynamicDataAsync(
                    StoredProcedure.Conditions.GetConditionScenarios, XElement.Parse(arguments)).ContinueWith(t => t.Result.Element("ConditionScenarios").Elements().Select(e => new ComboboxItem(e)));
        }

        //public Task<IList<ConditionScenario>> GetScenariosToApply(string salesOrgId, DateTime startDate, DateTime endDate, int conditionID)
        //{

        //    string arguments = GetConditionApplyToScenariosTemplate.FormatWith(User.CurrentUser.ID, salesOrgId,
        //        startDate.ToString(DateFormatNoHyphens), endDate.ToString(DateFormatNoHyphens), conditionID);
        //    return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetConditionApplyToScenarios, XElement.Parse(arguments))
        //        .ContinueWith(t => GetScenariosContinuation(t));
        //}

        //private IList<ConditionScenario> GetScenariosContinuation(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted)
        //        return new List<ConditionScenario>();
        //    return task.Result.MaybeElement("ConditionScenarios").MaybeElements("ConditionScenario")
        //        .Select(ConditionScenario.FromXml).OrderBy(s => s.Name).ToList();
        //}

        private static string AddCommentContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return string.Empty;
            return task.Result.GetValue<string>(OutcomeElement);
        }

        //public Task<IList<Customer>> GetCustomers(string salesOrganizationId, IEnumerable<string> ProductsIDs)
        //{
        //    Debug.WriteLine("Start GetCustomers" + DateTime.Now.ToString());
        //    Debug.WriteLine("Start ClearCustomerCache" + DateTime.Now.ToString());
        //      _customerCache.Clear();
        //      Debug.WriteLine("End ClearCustomerCache" + DateTime.Now.ToString());
        //    return GetCustomersInternal(salesOrganizationId, ProductsIDs);

        //}

        //private Task<IList<Customer>> GetCustomersInternal(string salesOrganizationId, IEnumerable<string> productsIDs)
        //{
        //    Debug.WriteLine("Start GetCustomersInternal" + DateTime.Now.ToString());
        //    const string getFilterCustomers = "GetCustomerTree";
        //    //string prods = string.Join("", (from c in productsIDs select new XElement("Prod_Idx", c)));
        //    var arguments = CustomersTemplate.FormatWith(getFilterCustomers, User.CurrentUser.ID, salesOrganizationId);
        //    Task<IList<Customer>> VarToReturn = (_customerCache.Count == 0 ?
        //              GetCustomersFromDatabaseAndPopulateCache(arguments, salesOrganizationId, productsIDs)
        //             : Task.Factory.FromResult((IList<Customer>)_customerCache.ToList()));

        //    Debug.WriteLine("End GetCustomersInteral" + DateTime.Now.ToString());
        //    return VarToReturn;
        //}

        //private Task<IList<Customer>> GetCustomersFromDatabaseAndPopulateCache(string arguments, string salesOrgID, IEnumerable<string> productsIDs)
        //{
        //    Debug.WriteLine("Start GetCustomersFromDataBaseAndPopulateCache" + DateTime.Now.ToString());
        //    Task<IList<Customer>> VarToReturn = WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetFilterCustomers, XElement.Parse(arguments), DisplayErrors.Yes, true, false)
        //        .ContinueWith(t => GetCustomersFromDatabaseAndPopulateCacheContinuation(t, salesOrgID, productsIDs));
        //    Debug.WriteLine("End GetCustomersFromDataBaseAndPopulateCache" + DateTime.Now.ToString());
        //    return VarToReturn;
        //    //return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetFilterCustomers, XElement.Parse(arguments),DisplayErrors.Yes,true,false)
        //    //    .ContinueWith(t => GetCustomersFromDatabaseAndPopulateCacheContinuation(t, salesOrgID,productsIDs));
        //}

        private static readonly object CustomerSyncObject = new object();

        //private IList<Customer> GetCustomersFromDatabaseAndPopulateCacheContinuation(Task<XElement> task, string salesOrganizationId, IEnumerable<string> productsIDs)
        //{
        //    Debug.WriteLine("Start GetCustomersFromDataBaseAndPopulateCacheContinuation" + DateTime.Now.ToString());
        //    if (_customerCache.Count == 0 && task.Result != null)
        //        lock (CustomerSyncObject)
        //            if (_customerCache.Count == 0)
        //            {
        //                const string customersElement = "Customers";
        //                if (task.IsCanceled || task.IsFaulted)
        //                    return new List<Customer>();
        //                var customers = task.Result.Elements(customersElement)
        //                    .Select(c => new Customer(c, () => GetCustomersInternal(salesOrganizationId, productsIDs).Result)).ToList();
        //                _customerCache.CacheRange(customers);
        //            }
        //    Debug.WriteLine("End GetCustomersFromDataBaseAndPopulateCacheContinuation" + DateTime.Now.ToString());
        //    return _customerCache.ToList();
        //}

        public string GetFilterDatesProc()
        {
            return StoredProcedure.Conditions.GetFilterDates;
        }

        public Task CopyConditions(IList<string> selectedConditionIds)
        {
            const string containerName = "CopyCondition";
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.CopyConditions, GetUserIdCommand(containerName, selectedConditionIds));
        }

        public Task<XElement> DeleteConditions(IList<string> selectedConditionIds)
        {
            const string containerName = "DeleteCondition";
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.DeleteConditions,
                GetUserIdCommand(containerName, selectedConditionIds))
                .ContinueWith(t => DeleteConditionsContinuation(t));
        }

        private XElement DeleteConditionsContinuation(Task<XElement> task)
        {
            if(task.Result != null)
                MessageBoxShow(task.Result.Value);

            return task.Result;
        }

        private static XElement GetUserIdCommand(string containerName, IEnumerable<string> conditionIds)
        {
            return new XElement(containerName,
                new XElement(CondUserIdxElement, User.CurrentUser.ID),
                new XElement(ConditionsElement, conditionIds.Select(id => new XElement(CondIdxElement, id))));
        }

        public Task<ConditionDetail> GetCondition(string conditionId)
        {
           // WebServiceProxy.LogMessageToFile("WS GetCondition: " + DateTime.Now.ToString());
            var arguments = GetConditionTemplate.FormatWith(User.CurrentUser.ID, conditionId);
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetCondition, XElement.Parse(arguments))
                .ContinueWith(t => GetConditionContinuation(t));
        }

        private static ConditionDetail GetConditionContinuation(Task<XElement> task)
        {
          //  WebServiceProxy.LogMessageToFile("GetConditionContinuation GetFilterStatuses: " + DateTime.Now.ToString());
            const string condition = "Condition";
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;
            return ConditionDetail.FromXml(task.Result.Element(condition));
        }

        public Task<IList<Comment>> GetConditionComments(string conditionId)
        {
            var arguments = GetConditionCommentsTemplate.FormatWith(User.CurrentUser.ID, conditionId);
            return WebServiceProxy.CallAsync(StoredProcedure.Conditions.GetConditionComments, XElement.Parse(arguments), DisplayErrors.No)
                .ContinueWith(t => GetConditionCommentsContinuation(t));
        }

        private static IList<Comment> GetConditionCommentsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return null;
            return task.Result.MaybeElements().Select(Comment.FromXml).ToList();
        }



        private static IList<ConditionMeasure> GetConditionMissingPricesContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return null;
            return task.Result.MaybeElements().Select(ConditionMeasure.FromXml).ToList();
        }

        protected static void MessageBoxShow(string message, string title = null,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
        {
            switch (image)
            {
                case MessageBoxImage.Error:
                    Messages.Instance.PutError(message);
                    break;
                case MessageBoxImage.Warning:
                    Messages.Instance.PutWarning(message);
                    break;
                default:
                    Messages.Instance.PutInfo(message);
                    break;
            }
        }
        
    }
}