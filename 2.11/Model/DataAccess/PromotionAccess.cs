using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Xml;
using Model.Entity.Generic;
using Model.Entity.PowerPromotion;
using Model.Entity;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Model.DataAccess.Generic;
using Model.Entity.Listings;

namespace Model.DataAccess
{

    public class PromotionAccess       
    {
        private const string PromotionQueryTemplate = "<{0}><UserID>{1}</UserID><PromotionID>{2}</PromotionID></{0}>";
        private const string NewPromotionQueryTemplate = "<{0}><User_Idx>{1}</User_Idx><Promo_Idx>{2}</Promo_Idx><Cust_Idx>{3}</Cust_Idx><IsPowerEditorMode>{4}</IsPowerEditorMode></{0}>";

        #region Cached Collections
        private static readonly AutoCache<string, PromotionData> DataCache = new AutoCache<string, PromotionData>(pd => pd.ID + pd.ItemType);
        private static readonly AutoCache<string, PromotionCustomer> CustomersCache = new AutoCache<string, PromotionCustomer>(pc => pc.ID);
        private static readonly AutoCache<string, PromotionStatus> StatusesCache = new AutoCache<string, PromotionStatus>(ps => ps.ID);
        private static readonly AutoCache<string, PromotionDate> DatesCache = new AutoCache<string, PromotionDate>(pd => pd.ID);
        private static readonly AutoCache<string, PromotionDatePeriod> DatePeriodsCache = new AutoCache<string, PromotionDatePeriod>(pd => pd.ID);
        private static readonly AutoCache<string, PromotionProduct> ProductsCache = new AutoCache<string, PromotionProduct>(pp => pp.ID);
        private static readonly AutoCache<string, PromotionProductPrice> ProductPricesCache = new AutoCache<string, PromotionProductPrice>(ppp => ppp.ID);
        private static readonly AutoCache<string, PromotionProductPrice> PromotionProductPricesCache = new AutoCache<string, PromotionProductPrice>(ppp => ppp.ID);
        private static readonly AutoCache<string, PromotionAttribute> AttributesCache = new AutoCache<string, PromotionAttribute>(pa => pa.ID);
        private static readonly AutoCache<string, PromotionVolume> VolumesCache = new AutoCache<string, PromotionVolume>(pv => pv.ProductId);
       // private static readonly AutoCache<string, PromotionDisplayUnit> DisplayUnitCache = new AutoCache<string, PromotionDisplayUnit>(pv => pv.ProductId);
        private static readonly AutoCache<string, PromotionFinancial> FinancialCache = new AutoCache<string, PromotionFinancial>(pf => pf.ID);

       // private static readonly AutoCache<string, ScheduleData> ScheduleCache = new AutoCache<string, ScheduleData>(pd => pd.ID + pd.ItemType);
       // private static readonly AutoCache<string, ScheduleCustomer> CustomersSCHDCache = new AutoCache<string, ScheduleCustomer>(pc => pc.ID);

        private static readonly AutoCache<string, PromotionWizardCustomer> WizardCustomersCache = new AutoCache<string, PromotionWizardCustomer>(pc => pc.ID);

        #endregion

        private static void ResetStaticCaches()
        {
            DataCache.Clear();
            CustomersCache.Clear();
            StatusesCache.Clear();
            DatesCache.Clear();
            ProductsCache.Clear();
            ProductPricesCache.Clear();
            PromotionProductPricesCache.Clear();
            AttributesCache.Clear();
            VolumesCache.Clear();
            FinancialCache.Clear();
            DatePeriodsCache.Clear();
            //CustomersSCHDCache.Clear();
            WizardCustomersCache.Clear();
        }

        public void ResetCache()
        {
            ResetStaticCaches();
        }

        private IList<PromotionData> GetPromotionDataContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<PromotionData>();
            return DataCache.CacheRange(task.Result.Elements().Select(n => new PromotionData(n))).ToList();

        }

        public Task<XElement> GetPromotionChartDataAsync(XElement args)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetPromotionChartData, args, DisplayErrors.No).ContinueWith(t => GetPromotionChartDataContinuation(t));
        }

        private XElement GetPromotionChartDataContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            return task.Result;

        }


        /// <summary>
        /// Retrieves list of Promotion Customers from underlaying service manager
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PromotionCustomer> GetPromotionFilterCustomers(string promotionId)
        {
            return GetPromotionFilterCustomersCached(promotionId);
        }

        private static IEnumerable<PromotionCustomer> GetPromotionFilterCustomersCached(string promotionId)
        {
            if (CustomersCache.Count == 0)
            {
                string arguments = "<GetPromotionsFilterCustomers><UserID>{0}</UserID><PromotionID>{1}</PromotionID> </GetPromotionsFilterCustomers>"
                    .FormatWith(User.CurrentUser.ID, promotionId);

                var customerNodes =
                    WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotionsFilterCustomers, XElement.Parse(arguments)).Elements();
                return CustomersCache.CacheRange(customerNodes.Select(c => new PromotionCustomer(c)));
            }

            return CustomersCache;
        }

        public Task<XElement> GetAddPromotionWizardCustomersAsync(string promotionId)
        {
            string arguments = "<GetAddPromotionCustomers><User_Idx>{0}</User_Idx><PromotionID>{1}</PromotionID><SalesOrg_Idx>{2}</SalesOrg_Idx></GetAddPromotionCustomers>"
                                .FormatWith(User.CurrentUser.ID, promotionId, User.CurrentUser.SalesOrganisationID);

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetAddPromotionsCustomers, XElement.Parse(arguments));

        }

        public IEnumerable<PromotionWizardCustomer> GetPromotionWizardCustomers(string promotionId)
        {
            return GetPromotionWizardCustomersCached(promotionId);
        }

        private static IEnumerable<PromotionWizardCustomer> GetPromotionWizardCustomersCached(string promotionId)
        {
                string arguments = "<GetAddPromotionCustomers><User_Idx>{0}</User_Idx><PromotionID>{1}</PromotionID> </GetAddPromotionCustomers>"
                             .FormatWith(User.CurrentUser.ID, promotionId);

                var result = WebServiceProxy.Call(StoredProcedure.Promotion.GetAddPromotionsCustomers, XElement.Parse(arguments));

                WizardCustomersCache.CacheRange(result.Elements().Select(c => new PromotionWizardCustomer(c)));
            

            return WizardCustomersCache;
        }


        public IEnumerable<PromotionChart> GetPromotionChartList()
        {
            string arguments = "<GetPromotion_Graphs><User_Idx>{0}</User_Idx></GetPromotion_Graphs>".FormatWith(User.CurrentUser.ID);

            var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotionChartList, XElement.Parse(arguments),DisplayErrors.No).Elements();
            //IEnumerable<XElement> dummyXml = XElement.Parse("<Results><Chart><Idx>1</Idx><Name>Test1</Name><IsSelected>0</IsSelected></Chart><Chart><Idx>2</Idx><Name>Test2</Name><IsSelected>1</IsSelected></Chart></Results>").Elements();
            return nodes.Select(n => new PromotionChart(n)).ToList();
        }

        /// <summary>
        /// Retrieves list of Promotion Dates from underlaying service manager
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PromotionDate> GetPromotionDates(string custID = null, string promotionId = null, bool isPowerEditor = false, bool useCache = true)
        {
            if (DatesCache.Count == 0 || !useCache)
            {
                string arguments = "";
                if (isPowerEditor == false)
                {
                    arguments = "<GetPromotionDateTypes><UserID>{0}</UserID><PromotionID>{1}</PromotionID></GetPromotionDateTypes>"
                        .FormatWith(User.CurrentUser.ID, promotionId);
                }
                else
                {
                    arguments = "<GetPromotionDateTypes><UserID>{0}</UserID><Customer>{1}</Customer><IsPowerEditorMode>1</IsPowerEditorMode></GetPromotionDateTypes>"
                        .FormatWith(User.CurrentUser.ID, custID);
                }

                var dateNodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotionDateTypes, XElement.Parse(arguments)).Elements();
                DatesCache.CacheRange(dateNodes.Select(d => new PromotionDate(d)));
            }

            return DatesCache;
        }

        public IEnumerable<PromotionDatePeriod> GetPromotionDatePeriods(Promotion promotion, bool useCache = true)
        {
            //if (!useCache || DatePeriodsCache.Count == 0)
            //{
            const string elementName = "GetPromotionDatePeriods";
            if (promotion.CustomerIdx != null)
                return NewGetPromotionElements(promotion, elementName, StoredProcedure.Promotion.GetPromotionDatePeriods, n => new PromotionDatePeriod(n), promotion.CustomerIdx, true);

            return NewGetPromotionElements(promotion, elementName, StoredProcedure.Promotion.GetPromotionDatePeriods, n => new PromotionDatePeriod(n));
            //    DatePeriodsCache.CacheRange(datePeriods);
            //}

            //return DatePeriodsCache;
        }

        /// <summary>
        /// Returns default date values for Promotion filter
        /// </summary>
        /// <returns></returns>        
        public PromotionDate GetDefaultPromotionFilterDates()
        {
            string arguments = "<GetPromotionDates><UserID>{0}</UserID></GetPromotionDates>".FormatWith(User.CurrentUser.ID);
            var dateNodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotionDefaultFilterDates, XElement.Parse(arguments)).Elements().ToList();

            var dates = new PromotionDate
            {
                StartDate = dateNodes.FirstOrDefault().GetValue<DateTime>("Start"),
                EndDate = dateNodes.FirstOrDefault().GetValue<DateTime>("End")
            };

            return dates;
        }


        public PromotionDate GetDefaultScheduleFilterDates()
        {
            string arguments = "<GetPromotionDates><UserID>{0}</UserID></GetPromotionDates>".FormatWith(User.CurrentUser.ID);
            var dateNodes = WebServiceProxy.Call(StoredProcedure.Schedule.GetDates, XElement.Parse(arguments)).Elements().ToList();

            var dates = new PromotionDate
            {
                StartDate = dateNodes.FirstOrDefault().GetValue<DateTime>("Start"),
                EndDate = dateNodes.FirstOrDefault().GetValue<DateTime>("End")
            };

            return dates;
        }

        public TreeViewHierarchy GetAddPromotionProducts(string promotionId, bool useCache = true)
        {
            string arguments = "<GetAddPromotionProducts><UserID>{0}</UserID><PromotionID>{1}</PromotionID></GetAddPromotionProducts>"
                    .FormatWith(User.CurrentUser.ID, promotionId);

            return DynamicDataAccess.GetGenericItem<TreeViewHierarchy>(StoredProcedure.Promotion.GetAddPromotionProducts, XElement.Parse(arguments));           

        }
        /// <summary>
        /// Retrieves list of Promotion Product Prices from underlaying service manager
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PromotionProductPrice> GetPromotionProductPrices(string promotionId, IEnumerable<string> productIDs)
        {
            if (promotionId == null) throw new ArgumentNullException("promotionId");
            var productIdList = new HashSet<string>(productIDs ?? Enumerable.Empty<string>());

            if (productIdList.Any())
            {
                var argument = new XElement("GetAddPromotionProductPrices");
                argument.Add(new XElement("UserID", User.CurrentUser.ID));
                argument.Add(new XElement("PromotionID", promotionId));
                argument.Add(new XElement("Products"));
                foreach (var p in productIdList)
                {
                    argument.Element("Products").Add(new XElement("ID", p));
                }

                var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetAddPromotionProductPrices, argument, DisplayErrors.No).Elements();
                ProductPricesCache.CacheRange(nodes.Select(n => new PromotionProductPrice(n)));
            }

            return ProductPricesCache.Where(ppp => productIdList.Contains(ppp.ID));
        }

        public StagedPromotion GetStagedPromotion(string promotionId, IEnumerable<string> measureIDs, string stagingType)
        {
            var argument = new XElement("GetProductDailyVolumes");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("PromotionID", promotionId));
            argument.Add(new XElement("TimeLevel", stagingType));
            argument.Add(new XElement("Measures"));

            if (measureIDs != null)
            {
                foreach (var measure in measureIDs)
                {
                    argument.Element("Measures").Add(new XElement("ID", measure));
                }
            }

            var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetProductDailyVolumes, argument);
            return new StagedPromotion(nodes);
        }

        public string SaveStagedPromotion(StagedPromotion promotion)
        {
            XElement argument = promotion.CreateSaveArgument();

            XElement node = WebServiceProxy.Call(StoredProcedure.Promotion.SaveProductDailyVolumes, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        public string ValidatePromotionVolumesDaily(StagedPromotion promotion)
        {
            XElement argument = promotion.CreateSaveArgument();

            XElement node = WebServiceProxy.Call(StoredProcedure.Promotion.ValidatePromotionVolumesDaily, argument, DisplayErrors.No).Elements().FirstOrDefault();

            return node.Value;
        }

        public XElement GetPromotionVolumesX(string promotionId, IEnumerable<string> productIDs)
        {
            // No Caching as long as data are dependened on variable input parameters
            //if (_PromotionVolumes == null)
            //{

            var argument = new XElement("GetProductVolumes");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("PromotionID", promotionId));
            argument.Add(new XElement("Products"));

            if (productIDs != null)
            {
                foreach (var p in productIDs)
                {
                    argument.Element("Products").Add(new XElement("ID", p));
                }
            }

            var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetProductVolumes, argument, DisplayErrors.No);

            //}

            return nodes;
        }

        public IEnumerable<PromotionVolume> ApplyPromotionVolumeOperation(string promotionId,
                                                                          IEnumerable<PromotionVolume> currentVolumes,
                                                                          PromotionVolumeOperation operation)
        {
            var argument = new XElement("ApplyPromotionVolumeOperation");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("PromotionID", promotionId));
            argument.Add(new XElement("Value", decimal.Parse(operation.Value, CultureInfo.CurrentCulture.NumberFormat)));
            argument.Add(new XElement("Volumes"));
            Promotion.PopulateVolumesElement(argument.Element("Volumes"), currentVolumes);

            var xml = WebServiceProxy.Call(operation.StoredProc, argument);
            var nodes = xml.Elements();
            VolumesCache.CacheRange(nodes.Select(n => new PromotionVolume(n)));
            return VolumesCache;
        }

        public XElement GetPromotionDisplayUnitsX(string promotionId, IEnumerable<string> productIDs)
        {


            var argument = new XElement("GetProductDisplayVolumes");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("PromotionID", promotionId));
            argument.Add(new XElement("Products"));

            if (productIDs != null)
            {
                foreach (var p in productIDs)
                {
                    argument.Element("Products").Add(new XElement("ID", p));
                }
            }

            var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetProductDisplaytUnits, argument, DisplayErrors.No);

            return nodes;
        }

        public XElement GetPromotionStealVolumesX(string promotionId, IEnumerable<string> productIDs)
        {
            // No Caching as long as data are dependened on variable input parameters
            //if (_PromotionVolumes == null)
            {

                var argument = new XElement("GetProductStealVolumes");
                argument.Add(new XElement("UserID", User.CurrentUser.ID));
                argument.Add(new XElement("PromotionID", promotionId));
                var productsElement = new XElement("Products");
                argument.Add(productsElement);

                if (productIDs != null)
                {
                    foreach (var p in productIDs)
                    {
                        productsElement.Add(new XElement("ID", p));
                    }
                }

                try
                {
                    return WebServiceProxy.Call(StoredProcedure.Promotion.GetProductStealVolumes, argument, DisplayErrors.No);
                }
                catch (ExceedraDataException)
                {
                    return null;
                }

            }


        }

        public XElement GetPromotionFinancialPromoMeasures(string promotionId)
        {
            {

                var argument = new XElement("GetFinancialsPromoMeasures");
                argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
                argument.Add(new XElement("Promo_Idx", promotionId));

                return WebServiceProxy.Call(StoredProcedure.Promotion.GetFinancialPromoMeasures, argument, DisplayErrors.No);
                 
            }

        }

        public XElement GetPromotionFinancialParentProductMeasures(string promotionId)
        {
            {

                var argument = new XElement("GetFinancialsPromoMeasures");
                argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
                argument.Add(new XElement("Promo_Idx", promotionId));


                return WebServiceProxy.Call(StoredProcedure.Promotion.GetFinancialParentProductMeasues, argument, DisplayErrors.No);

            }

        }

        public XElement GetFinancialScreenPlanningSkuMeasure(string promotionId)
        {
            {

                var argument = new XElement("GetFinancialsPromoMeasures");
                argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
                argument.Add(new XElement("Promo_Idx", promotionId));


                return WebServiceProxy.Call(StoredProcedure.Promotion.GetFinancialProductMeasures, argument, DisplayErrors.No);

            }

        }

        /// <summary>
        /// Sends created promotionData back to the web service and gets an Url to return to the page
        /// </summary>
        /// <returns></returns>
        public PromotionSaveResults SavePromotion(Promotion promotion, DateTime? lastSaved, string page,
            Dictionary<string, bool> selectedProducts = null, XElement volumes = null, XElement displayVolumes = null, XElement stealVolumes = null, XElement attachments = null)
        {
            XElement argument = promotion.CreateSaveArgument(page, lastSaved, selectedProducts, volumes, displayVolumes, stealVolumes, promotion.AttributesComment, attachments);

            var results = new PromotionSaveResults(WebServiceProxy.Call(StoredProcedure.Promotion.SavePromotion, argument)); //Procast_SP_PROMO_SavePromotion

            return results;
        }
 
        public PromotionSaveResults SavePromotionPhasing(Promotion promotion, DateTime? lastSaved, string page, XElement phasing)
        {
            XElement argument = new XElement("SavePromotions");
            argument.Add(new XElement("UserID", User.CurrentUser.ID)); // REQUIRED
            argument.Add(new XElement("ScreenName", page)); // REQUIRED 
            argument.Add(new XElement("LastSaveDate", (lastSaved.HasValue ? lastSaved.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") : ""))); // REQUIRED 

            var rootPromoNode = new XElement("Promotion"); // REQUIRED

            rootPromoNode.Add(new XElement("ID", promotion.Id)); // REQUIRED

            rootPromoNode.Add(phasing);

            var results = new PromotionSaveResults(WebServiceProxy.Call(StoredProcedure.Promotion.SavePromotion, argument));

            return results;
        }

        /// <summary>
        /// Creates and returns a Promotion based on given Id
        /// </summary>
        /// <param name="promoId"></param>
        /// <param name="page"></param>
        /// <param name="lastSaved"></param>
        /// <returns></returns>
        public PromotionGetResults GetPromotion(string page, DateTime? lastSaved, string promoId = null, string custId = null, bool isPowerEditor = false)
        {
            XElement argument = new XElement("GetPromotion");
            argument.Add(UserIdxElement);
            argument.Add(new XElement("PromotionID", promoId));
            argument.Add(new XElement("ScreenName", page)); // REQUIRED 
            argument.Add(new XElement("LastSaveDate", (lastSaved.HasValue ? lastSaved.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : ""))); // REQUIRED 

            var promotion = new PromotionGetResults(WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotion, argument));

            return promotion;

        }
        /// <summary>
        /// Creates a copy of specified promotion
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public string CopyPromotion(string[] Ids)
        {
            XElement argument = new XElement("CopyPromotion");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("Promotions"));

            foreach (var id in Ids)
            {
                argument.Element("Promotions").Add(new XElement("ID", id));
            }

            var node = WebServiceProxy.Call(StoredProcedure.Promotion.CopyPromotion, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        public string UpdateStatusPromotions(string[] Ids, string StatusID)
        {
            XElement argument = new XElement("UpdatePromotionStatus");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Target_Status_Idx", StatusID));
            argument.Add(new XElement("Promotions"));

            foreach (var id in Ids)
            {
                argument.Element("Promotions").Add(new XElement("Promo_Idx", id));
            }

            var node = WebServiceProxy.Call(StoredProcedure.Promotion.UpdateMultiplePromotionStatus, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        /// <summary>
        /// Gets All Promotion Approvers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PromotionApprovalLevel> GetApprovalLevels(string promoId)
        {
            string arguments = "<GetApprovalLevels><UserID>{0}</UserID><PromotionID>{1}</PromotionID></GetApprovalLevels>"
                                .FormatWith(User.CurrentUser.ID, promoId);

            var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetApprovalLevels, XElement.Parse(arguments)).Elements();
            var approverList = new List<PromotionApprovalLevel>();
            approverList.AddRange(nodes.Select(n => new PromotionApprovalLevel(n)));

            return approverList;
        }

        /// <summary>
        /// Gets All Promotion Approvers
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<PromotionApprovalLevel>> GetApprovalLevelsAsync(string promoId)
        {
            string arguments = "<GetApprovalLevels><UserID>{0}</UserID><PromotionID>{1}</PromotionID></GetApprovalLevels>"
                              .FormatWith(User.CurrentUser.ID, promoId);

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetApprovalLevels, XElement.Parse(arguments)).ContinueWith(t => GetApprovalLevelsContinuation(t));
        }

        private IEnumerable<PromotionApprovalLevel> GetApprovalLevelsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<PromotionApprovalLevel>();

            var approverList = new List<PromotionApprovalLevel>();
            approverList.AddRange(task.Result.Elements().Select(n => new PromotionApprovalLevel(n)));

            return approverList;
        }

        /// <summary>
        /// Returns Workflow Statuses to be used on Promotion wizard
        /// </summary>
        /// <param name="promotion">The currently selected promotion; may be <c>null</c>.</param>
        /// <returns></returns>
        public IEnumerable<PromotionStatus> GetPromotionWorkflowStatuses(Promotion promotion)
        {
            const string elementName = "GetPromotionStatuses";
            return GetPromotionElements(promotion, elementName, StoredProcedure.Promotion.GetPromotionWorkflowStatuses, n => new PromotionStatus(n));
        }

        /// <summary>
        /// Returns Workflow Scenarios to be used on Promotion wizard
        /// </summary>
        /// <param name="currentPromotion">The currently selected promotion; may be <c>null</c>.</param>
        /// <returns></returns>
        public IEnumerable<ComboboxItem> GetPromotionScenarios(Promotion currentPromotion)
        {
            const string elementName = "GetPromotionScenarios";
            var currentPromotionId = currentPromotion != null ? currentPromotion.Id : string.Empty;
            var arguments = XElement.Parse(PromotionQueryTemplate.FormatWith(elementName, User.CurrentUser.ID, currentPromotionId));

            return DynamicDataAccess.GetGenericEnumerable<ComboboxItem>(StoredProcedure.Promotion.GetPromotionScenarios, arguments);
        }

        private static IEnumerable<T> GetPromotionElements<T>(Promotion currentPromotion,
            string elementName, string procName, Func<XElement, T> xmlTranslate)
        {
            var currentPromotionId = currentPromotion != null ? currentPromotion.Id : string.Empty;
            var arguments = PromotionQueryTemplate.FormatWith(elementName, User.CurrentUser.ID, currentPromotionId);

            var nodes = WebServiceProxy.Call(procName, XElement.Parse(arguments), DisplayErrors.No).Elements();
            return nodes.Select(xmlTranslate).ToList();
        }

        //uses Idx
        private static IEnumerable<T> NewGetPromotionElements<T>(Promotion currentPromotion,
            string elementName, string procName, Func<XElement, T> xmlTranslate, string custID = null, bool isPowerEditor = false)
        {
            var currentPromotionId = currentPromotion.Id ?? string.Empty;
            var powerEditorMode = isPowerEditor ? "1" : "0";
            var arguments = NewPromotionQueryTemplate.FormatWith(elementName, User.CurrentUser.ID, currentPromotionId, custID, powerEditorMode);

            var nodes = WebServiceProxy.Call(procName, XElement.Parse(arguments), DisplayErrors.No).Elements();
            return nodes.Select(xmlTranslate).ToList();
        }

        /// <summary>
        /// Returns list of added comments for a given Promotion
        /// </summary>
        /// <param name="promoId"></param>
        /// <returns></returns>
        public IEnumerable<PromotionComment> GetPromotionComments(string promoId)
        {
            string arguments = "<GetPromotionComments><UserID>{0}</UserID><PromotionID>{1}</PromotionID></GetPromotionComments>"
                                .FormatWith(User.CurrentUser.ID, promoId);

            try
            {
                var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotionComments, XElement.Parse(arguments), DisplayErrors.No).Elements();
                var commentList = new List<PromotionComment>();
                commentList.AddRange(nodes.Select(n => new PromotionComment(n)));

                return commentList;
            }
            catch (Model.ExceedraDataException)
            {
                return Enumerable.Empty<PromotionComment>();
            }
        }
        /// <summary>
        /// Creates a new comment for a given Promotion
        /// </summary>
        /// <param name="promoId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public string AddPromotionComment(string promoId, string comment)
        {
            XElement argument = new XElement("AddPromotionComments");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("PromotionID", promoId));
            argument.Add(new XElement("Comment", comment));

            var node = WebServiceProxy.Call(StoredProcedure.Promotion.AddPromotionComment, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        /// <summary>
        /// Deletes a promotion by given Id
        /// </summary>
        /// <param name="promoId"></param>
        /// <returns></returns>
        public string DeletePromotion(string[] Ids)
        {
            XElement argument = new XElement("DeletePromotion");


            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("Promotions"));

            foreach (var id in Ids)
            {
                argument.Element("Promotions").Add(new XElement("ID", id));
            }



            //DisplayErrors displayErrors = DisplayErrors.No;

            var node = WebServiceProxy.Call(StoredProcedure.Promotion.DeletePromotion, argument, DisplayErrors.No).Elements().FirstOrDefault();

            return node.Value;
        }



        public XElement GetSubCustomers(string promotionId, string userId, string customerId)
        {
            string xml =
                string.Format(
                    @"<GetPromoCustomersSubLevel>
                                    <Promo_Idx>{0}</Promo_Idx>
                                    <User_Idx>{1}</User_Idx>
                                    <ParentCust_Idx>{2}</ParentCust_Idx>
                                  </GetPromoCustomersSubLevel>",
                    promotionId, userId, customerId);

            var argument = XElement.Parse(xml);

            return WebServiceProxy.Call(StoredProcedure.Promotion.GetAddPromotionCustomersSubLevel, argument);
            //var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetAddPromotionCustomersSubLevel, argument, DisplayErrors.No).Elements();
            //var dict = nodes.Select(n => new PromotionWizardCustomer(n)).ToDictionary(c => c.ID);
            //foreach (var customer in dict.Values)
            //{
            //    if (!string.IsNullOrWhiteSpace(customer.ParentId))
            //    {
            //        customer.Parent = dict[customer.ParentId];
            //        customer.Parent.AddChild(customer);
            //    }
            //}

            //return dict.Values.Where(c => string.IsNullOrWhiteSpace(c.ParentId));
        }

        public XElement GetSubCustomers(string promotionId, string userId, string customerId, IEnumerable<string> customersIds)
        {
            string xml =
                string.Format(
                    @"<GetPromoCustomersSubLevel>
                                    <Promo_Idx>{0}</Promo_Idx>
                                    <User_Idx>{1}</User_Idx>
                                    <ParentCust_Idx>{2}</ParentCust_Idx>
                                  </GetPromoCustomersSubLevel>",
                    promotionId, userId, customerId);

            var argument = XElement.Parse(xml);

            if (customersIds.Any())
            {
                XElement xCsvSelectedCustomers = new XElement("CSVSelectedCustomers");
                foreach (var nodeId in customersIds)
                    xCsvSelectedCustomers.Add(new XElement("Cust_Code", nodeId));
                argument.Add(xCsvSelectedCustomers);
            }

            return WebServiceProxy.Call(StoredProcedure.Promotion.GetAddPromotionCustomersSubLevel, argument);

            //var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetAddPromotionCustomersSubLevel, argument, DisplayErrors.No).Elements();
            //var dict = nodes.Select(n => new PromotionWizardCustomer(n)).ToDictionary(c => c.ID);
            //foreach (var customer in dict.Values)
            //{
            //    if (!string.IsNullOrWhiteSpace(customer.ParentId))
            //    {
            //        customer.Parent = dict[customer.ParentId];
            //        customer.Parent.AddChild(customer);
            //    }
            //}

            //return dict.Values.Where(c => string.IsNullOrWhiteSpace(c.ParentId));
        }

        public XElement GetPowerEditorSubCustomers(string promotionId, string userId, string customerId, IEnumerable<string> customersIds)
        {
            string xml =
                string.Format(
                    @"<GetPromoCustomersSubLevel>
                                                <Promo_Idx>{0}</Promo_Idx>
                                                <User_Idx>{1}</User_Idx>
                                                <ParentCust_Idx>{2}</ParentCust_Idx>
                                              </GetPromoCustomersSubLevel>",
                                                promotionId, userId, customerId);

            var argument = XElement.Parse(xml);

            if (customersIds.Any())
            {
                XElement xCsvSelectedCustomers = new XElement("CSVSelectedCustomers");
                foreach (var nodeId in customersIds)
                    xCsvSelectedCustomers.Add(new XElement("Cust_Code", nodeId));
                argument.Add(xCsvSelectedCustomers);
            }
            return WebServiceProxy.Call(StoredProcedure.Promotion.GetAddPromotionCustomersSubLevel, argument, DisplayErrors.No);
        }

 

        /// <summary>
        /// Saves user preferences for planning
        /// </summary>
        /// <returns></returns>
        public string SaveUserPrefsSchedule(PromotionPreferenceDTO preferenceToSave)
        {
            var argument = new XElement("SaveDefaults");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement(XMLNode.Nodes.Screen_Code.ToString(), "Schedule"));
            argument.Add(new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID));

            argument.AddElement("ListingsGroup_Idx", preferenceToSave.ListingsGroupIdx);

            argument.Add(new XElement("Customers"));
            if (preferenceToSave.Customers != null)
            {
                foreach (var cId in preferenceToSave.Customers)
                {
                    argument.Element("Customers").Add(new XElement("Idx", cId));
                }
            }
             
            argument.Add(new XElement("Products"));
            if (preferenceToSave.Products != null)
            {
                foreach (var pId in preferenceToSave.Products)
                {
                    argument.Element("Products").Add(new XElement("Idx", pId));
                }
            }

            //all selected type statuses

            if (preferenceToSave.ScheduleStatuses != null)
            {
                argument.Add(new XElement("Items"));
                foreach (var item in preferenceToSave.ScheduleStatuses)
                {
                    var ItemType = new XElement("ItemType");
                    ItemType.Add(new XElement("Type_Idx", item.ID));


                    var Statuses = new XElement("Statuses");
                    foreach (var s in item.Statuses)
                    {
                        Statuses.Add(new XElement("Status_Idx", s.ID));
                    }
                    ItemType.Add(Statuses);

                    argument.Element("Items").Add(ItemType);
                }
            }

            argument.Add(new XElement("Dates"));
            argument.Element("Dates").Add(new XElement("Start", preferenceToSave.DateStart));
            argument.Element("Dates").Add(new XElement("End", preferenceToSave.DateEnd));


            var node = WebServiceProxy.Call(StoredProcedure.Shared.SaveDefaults, argument).Elements().FirstOrDefault();

            // Remove Filter Caches
            CustomersCache.Clear();
            StatusesCache.Clear();

            return node.Value;
        }


        //public IEnumerable<ScheduleCustomer> GetScheduleFilterCustomers(string promotionId)
        //{
        //    return GetPromotionFilterScheduleCached(promotionId);
        //}

        //private static IEnumerable<ScheduleCustomer> GetPromotionFilterScheduleCached(string promotionId)
        //{
        //    if (CustomersSCHDCache.Count == 0)
        //    {
        //        string arguments = "<GetPromotionsFilterCustomers><UserID>{0}</UserID><PromotionID>{1}</PromotionID> </GetPromotionsFilterCustomers>"
        //            .FormatWith(User.CurrentUser.ID, promotionId);

        //        var customerNodes =
        //            WebServiceProxy.Call(StoredProcedure.Schedule.GetCustomers, XElement.Parse(arguments)).Elements();
        //        return CustomersSCHDCache.CacheRange(customerNodes.Select(c => new ScheduleCustomer(c)));
        //    }

        //    return CustomersSCHDCache;
        //}

        public Task<XElement> GetPromotionDocumentsAsync(string promotionId, string proc)
        {
            var argument = new XElement("GetPromotionDocuments");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Promo_Idx", promotionId));

            return WebServiceProxy.CallAsync(proc, argument, DisplayErrors.No)
                .ContinueWith(t => GetPromotionDashboardXContinuation(t));
        }

        public Task<XElement> GetPromotionDashboardXAsync(string promotionId, string proc)
        {
            var argument = new XElement("GetPromotionPandLGrid");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Promo_Idx", promotionId));

            return WebServiceProxy.CallAsync(proc, argument, DisplayErrors.No)
                .ContinueWith(t => GetPromotionDashboardXContinuation(t));
        }

        private XElement GetPromotionDashboardXContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            return task.Result;

        }

        public Task<XElement> GetPromotionAttributesAsync(string proc, string promotionId = null, string customerId = null, bool isPowerEditor = false)
        {
            var argument = new XElement("GetPromotionAttributes");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));

            if (promotionId != null && (customerId == null || isPowerEditor == false))
            {
                argument.Add(new XElement("Promo_Idx", promotionId));
                argument.Add(new XElement("IsPowerEditorMode", "0"));
            }
            else
            {
                if (customerId != null && isPowerEditor)
                {
                    argument.Add(new XElement("Customer", customerId));
                    argument.Add(new XElement("IsPowerEditorMode", "1"));
                }
            }

            return WebServiceProxy.CallAsync(proc, argument, DisplayErrors.Yes)
                .ContinueWith(t => GetPromotionAttributesAsyncContinuation(t));
        }

        public XElement ReCalc(string promotionId)
        {
            var argument = new XElement("RecalculatePandL");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("PromotionID", promotionId));

            return WebServiceProxy.Call(StoredProcedure.Promotion.RecalculatePandL, argument);
        }

        private XElement GetPromotionAttributesAsyncContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            return task.Result;

        }


        public IEnumerable<PromotionProduct> GetModalProductDataList(string promotionId, List<string> skuIds)
        {
            XElement skus = new XElement("Products");
            foreach (string Id in skuIds)
            {
                skus.Add(new XElement("Sku_Idx", Id));
            }

            string arguments = "<GetProductsList><User_Idx>{0}</User_Idx><Promo_Idx>{1}</Promo_Idx>{2}</GetProductsList>"
                .FormatWith(User.CurrentUser.ID, promotionId, skus);


            var productResults = WebServiceProxy.Call(StoredProcedure.Promotion.GetProductsList, XElement.Parse(arguments)).Elements();

            ObservableCollection<PromotionProduct> theseObservableProducts = new ObservableCollection<PromotionProduct>();

            theseObservableProducts.AddRange(productResults.Select(n => new PromotionProduct(n)));

            return theseObservableProducts;
        }

        public XElement GetDynamicProductGrid(string PromoId, string SkuId)
        {
            string arguments = "<GetProductsList><User_Idx>{0}</User_Idx><Promo_Idx>{1}</Promo_Idx><Sku_Idx>{2}</Sku_Idx></GetProductsList>"
                .FormatWith(User.CurrentUser.ID, PromoId, SkuId);

            return WebServiceProxy.Call(StoredProcedure.Promotion.GetProductsReferenceData, XElement.Parse(arguments));
        }

        public XElement ModalDynaicGridSave(string promoId, string SkuId, XElement RecordVMXelement)
        {

            string arguments = "<GetProductsList><User_Idx>{0}</User_Idx><Promo_Idx>{1}</Promo_Idx><Sku_Idx>{2}</Sku_Idx>{3}</GetProductsList>"
                .FormatWith(User.CurrentUser.ID, promoId, SkuId, RecordVMXelement);

            var result = WebServiceProxy.Call(StoredProcedure.Promotion.SaveProductsReferenceData, XElement.Parse(arguments), DisplayErrors.Yes);


            CommandManager.InvalidateRequerySuggested();

            return result;
        }


        #region Power Editor

        public XElement UserIdxElement = new XElement("User_Idx", User.CurrentUser.ID);
        public XElement UserIdElement = new XElement("UserID", User.CurrentUser.ID);
        public XElement SalesOrgIdxElement = new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID);
        public XElement PowerEditorElement = new XElement("IsPowerEditorMode", "1");

        //public XElement SavePowerEditorPage1Task(Promotion promotion, DateTime? LastSaveDate, XElement datesElement,
        //    List<string> productIDs, XElement attributesElement, List<string> subCustomerIDs = null)
        //{
        //    XElement argument = new XElement("SavePromo");
        //    argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
        //    argument.Add(new XElement("Promo_Idx", promotion.Id));
        //    argument.Add(new XElement("Promo_Name", promotion.Name));
        //    argument.Add(new XElement("Customer", promotion.Customer.ID));
        //    argument.Add(new XElement("LastSaveDate", LastSaveDate.HasValue ? LastSaveDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") : ""));
        //    argument.Add(new XElement("DatePeriod_Idx", promotion.DatePeriod.ID.HasValue() ? promotion.DatePeriod.ID : ""));

        //    argument.Add(datesElement);

        //    if (subCustomerIDs != null)
        //    {
        //        XElement subcustomers = new XElement("SubCustomer");
        //        foreach (var subCust in subCustomerIDs)
        //        {
        //            subcustomers.Add(new XElement("SubCustomer_Idx"), subCust);
        //        }

        //        argument.Add(subcustomers);
        //    }

        //    XElement products = new XElement("Products");
        //    foreach (var prod in productIDs)
        //    {
        //        products.Add(new XElement("Product_Idx", prod));
        //    }

        //    argument.Add(products);

        //    argument.Add(attributesElement);

        //    var result = WebServiceProxy.Call("app.Procast_SP_PROMO_PowerEditor_SavePromoPage1", argument);

        //    DisplayMessage(result.Descendants("Msg").FirstOrDefault());

        //    return result;
        //}

        public IEnumerable<PromotionDate> GetPowerPromotionDates(string custIdx = null, string promotionIdx = null)
        {
            XElement arguments = new XElement("GetPromotionDateTypes");
            arguments.Add(UserIdxElement);
            arguments.AddElement("PromoID", promotionIdx);
            arguments.AddElement("Customer", custIdx);
            arguments.Add(PowerEditorElement);

            var dateNodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotionDateTypes, arguments).Elements();

            return dateNodes.Select(d => new PromotionDate(d));
        }

        public XElement SavePowerEditorPage1Task(PowerPromotion promotion, DateTime? lastSaved)
        {
            XElement argument = new XElement("SavePromo");
            argument.Add(UserIdxElement);
            argument.Add(PowerEditorElement);
            argument.Add(new XElement("Promo_Idx", promotion.Idx));
            argument.Add(new XElement("Promo_Name", promotion.Name));
            argument.Add(new XElement("Customer", promotion.SelectedCustomer));
            argument.Add(new XElement("LastSaveDate", lastSaved.HasValue ? lastSaved.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") : ""));
            argument.Add(new XElement("DatePeriod_Idx", promotion.DatePeriodIdx.HasValue() ? promotion.DatePeriodIdx : ""));
            argument.Add(promotion.DatesXml);
            argument.Add(promotion.AttributesXml);

            if (promotion.SelectedSubCustomers != null)
            {
                XElement subcustomers = new XElement("SubCustomer");
                foreach (var subCust in promotion.SelectedSubCustomers)
                {
                    subcustomers.Add(new XElement("SubCustomer_Idx", subCust));
                }

                argument.Add(subcustomers);
            }

            XElement products = new XElement("Products");
            foreach (var prod in promotion.SelectedProducts)
            {
                products.Add(new XElement("Product_Idx", prod));
            }
            argument.Add(products);

            var result = WebServiceProxy.Call(StoredProcedure.Promotion.SavePromoPage1, argument);

            DisplayMessage(result.Descendants("Msg").FirstOrDefault());

            return result;
        }

        public XElement SavePowerEditorPage2Task(PowerPromotion promotion, DateTime? lastSaved)
        {
            XElement argument = new XElement("SavePromo");
            argument.Add(UserIdxElement);
            argument.Add(PowerEditorElement);
            argument.Add(new XElement("Promo_Idx", promotion.Idx));
            argument.Add(new XElement("Promo_Name", promotion.Name));
            argument.Add(new XElement("LastSaveDate", lastSaved.HasValue ? lastSaved.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") : ""));
            argument.Add(new XElement("Status_Idx", promotion.SelectedStatus));
            argument.Add(new XElement("PandLGrid", promotion.PAndLXml.Elements()));
            argument.Add(new XElement("FinancialMeasure", promotion.ProductMeasuresXml.Elements()));

            if (promotion.SelectedScenarios != null)
            {
                XElement scenarios = new XElement("Scenarios");
                foreach (var scenIdx in promotion.SelectedScenarios)
                {
                    scenarios.Add(new XElement("Scenario_Idx", scenIdx));
                }

                argument.Add(scenarios);
            }

            var result = WebServiceProxy.Call(StoredProcedure.Promotion.SavePromoPage2, argument);

            DisplayMessage(result.Descendants("Msg").FirstOrDefault());

            return result;
        }

        public XElement GetPowerEditorSubCustomers(string userId, string customerId, string promotionId = null)
        {
            string xml =
                string.Format(
                    @"<GetPromoCustomersSubLevel>
                                    <Promo_Idx>{0}</Promo_Idx>
                                    <User_Idx>{1}</User_Idx>
                                    <ParentCust_Idx>{2}</ParentCust_Idx>
                                  </GetPromoCustomersSubLevel>",
                    promotionId, userId, customerId);

            var argument = XElement.Parse(xml);

            var nodes = WebServiceProxy.Call(StoredProcedure.Promotion.GetCustomersSubLevel, argument, DisplayErrors.No);
            return nodes;
        }

        public IEnumerable<PromotionDatePeriod> GetPowerPromotionDatePeriods(string custIdx, string promoIdx = null)
        {
            XElement arguments = new XElement("GetPromotionDatePeriods");
            arguments.Add(UserIdxElement);
            arguments.Add(PowerEditorElement);
            arguments.AddElement("Cust_Idx", custIdx);
            arguments.AddElement("Promo_Idx", promoIdx);

            var periods = WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotionDatePeriods, arguments, DisplayErrors.No).Elements();

            return periods.Select(p => new PromotionDatePeriod(p));
        }

        public Task<XElement> GetPowerPromotionAttributesAsync(PowerPromotion promotion, bool loadingPromoData = false)
        {
            var argument = new XElement("GetPromotionAttributes");
            argument.Add(UserIdxElement);
            argument.Add(PowerEditorElement);

            //If we are loading the saved promo attributes, all we need is promo_Idx
            //If we are loading new attributes then send in all the data, but no promo_Idx
            if (loadingPromoData && promotion.Idx != null)
                argument.Add(new XElement("Promo_Idx", promotion.Idx));
            else
            {
                argument.Add(new XElement("Customer", promotion.SelectedCustomer));
                argument.Add(new XElement("DatePeriod_Idx", promotion.DatePeriodIdx.HasValue() ? promotion.DatePeriodIdx : ""));
                argument.Add(promotion.DatesXml);

                if (promotion.SelectedSubCustomers != null)
                {
                    XElement subcustomers = new XElement("SubCustomer");
                    foreach (var subCust in promotion.SelectedSubCustomers)
                    {
                        subcustomers.Add(new XElement("SubCustomer_Idx"), subCust);
                    }

                    argument.Add(subcustomers);
                }

                XElement products = new XElement("Products");
                foreach (var prod in promotion.SelectedProducts)
                {
                    products.Add(new XElement("Product_Idx", prod));
                }
                argument.Add(products);
            }


            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetAttributes, argument)
                .ContinueWith(t => GetPromotionAttributesAsyncContinuation(t));
        }

        public PromotionGetResults GetPowerPromotion(string promoId)
        {
            XElement argument = new XElement("GetPromotion");
            argument.Add(UserIdxElement);
            //argument.Add(PowerEditorElement);
            argument.Add(new XElement("PromotionID", promoId));

            var promotion = new PromotionGetResults(WebServiceProxy.Call(StoredProcedure.Promotion.GetPromotion, argument));

            return promotion;
        }
        //Procast_SP_PROMO_PowerEditor_GetPandLGrid
        //StoredProcedure.Promotion.GetPromotionWorkflowStatuses

        public Task<XElement> GetPowerPromotionPAndLGrid(string promoIdx)
        {
            XElement arguments = new XElement("GetPandLGrid");
            arguments.Add(UserIdxElement);
            arguments.Add(PowerEditorElement);
            arguments.AddElement("Promo_Idx", promoIdx);

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetPandLGrid, arguments).ContinueWith(t => GetPowerPromotionPAndLGridContinuation(t));
        }

        private XElement GetPowerPromotionPAndLGridContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return XElement.Parse("<Results></Results>");

            return task.Result;

            //return XElement.Load(@"C:\Users\ExceedraAdmin\Documents\My Received Files\powerPROMOpANDl.txt");
        }

        public Task<XElement> GetPowerPromotionFinancialPromoMeasures(string promotionId)
        {
            var argument = new XElement("GetFinancialsPromoMeasures");
            argument.Add(UserIdxElement);
            argument.Add(new XElement("Promo_Idx", promotionId));
            argument.Add(new XElement("IsPowerEditorMode", "1"));

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetFinancialPromoMeasures, argument, DisplayErrors.No).ContinueWith(t => GetPowerPromotionGenericContinuation(t));
        }

        private XElement GetPowerPromotionGenericContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return XElement.Parse("<Results></Results>");

            return task.Result;
        }


        public Task<XElement> GetPowerPromotion_SKULevelMeasures(string promotionId)
        {
            var argument = new XElement("GetSKULevelMeasures");
            argument.Add(UserIdxElement);
            argument.Add(new XElement("Promo_Idx", promotionId));
            argument.Add(new XElement("IsPowerEditorMode", "1"));

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetPowerPromotionSKULevelMeasures, argument, DisplayErrors.No).ContinueWith(t => GetPowerPromotionGenericContinuation(t));
        }

        public Task<XElement> GetPowerPromotion_BaseVolume(string promotionId)
        {
            var argument = new XElement("GetBaseVolume");
            argument.Add(UserIdxElement);
            argument.Add(new XElement("Promo_Idx", promotionId));
            argument.Add(new XElement("IsPowerEditorMode", "1"));

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetPowerPromotionBaseVolume, argument, DisplayErrors.No).ContinueWith(t => GetPowerPromotionGenericContinuation(t));
        }

        public Task<XElement> GetPowerPromotion_PromoVolume(string promotionId)
        {
            var argument = new XElement("GetPromoVolume");
            argument.Add(UserIdxElement);
            argument.Add(new XElement("Promo_Idx", promotionId));
            argument.Add(new XElement("IsPowerEditorMode", "1"));

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetPowerPromotionVolume, argument, DisplayErrors.No).ContinueWith(t => GetPowerPromotionGenericContinuation(t));
        }

        public Task<XElement> GetPowerPromotion_IncrementalVolume(string promotionId)
        {
            var argument = new XElement("GetIncrementalVolume");
            argument.Add(UserIdxElement);
            argument.Add(new XElement("Promo_Idx", promotionId));
            argument.Add(new XElement("IsPowerEditorMode", "1"));

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetPowerPromotionIncrementalVolume, argument, DisplayErrors.No).ContinueWith(t => GetPowerPromotionGenericContinuation(t));
        }

        public Task<XElement> GetPowerPromotion_Totals(string promotionId)
        {
            var argument = new XElement("GetTotals");
            argument.Add(UserIdxElement);
            argument.Add(new XElement("Promo_Idx", promotionId));
            argument.Add(new XElement("IsPowerEditorMode", "1"));

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetPowerPromotionTotals, argument, DisplayErrors.No).ContinueWith(t => GetPowerPromotionGenericContinuation(t));
        }

        public Task<List<ComboboxItem>> GetPowerPromotionWorkflowStatuses(string promoIdx)
        {
            XElement arguments = new XElement("GetPromotionStatuses");
            arguments.Add(UserIdElement);
            arguments.Add(PowerEditorElement);
            arguments.AddElement("PromotionID", promoIdx);

            return GetComboboxItems(StoredProcedure.Promotion.GetPromotionWorkflowStatuses, arguments);
        }

        public Task<List<ComboboxItem>> GetPowerPromotionScenarios(string promoIdx)
        {
            XElement arguments = new XElement("GetPromotionScenarios");
            arguments.Add(UserIdxElement);
            arguments.Add(PowerEditorElement);
            arguments.AddElement("Promo_Idx", promoIdx);

            return GetComboboxItems(StoredProcedure.Promotion.GetPromotionScenarios, arguments);
        }

        #region Comboboxes
        private Task<List<ComboboxItem>> GetComboboxItems(string proc, XElement arguments)
        {
            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetComboboxItemsContinuation(t));
        }

        private List<ComboboxItem> GetComboboxItemsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ComboboxItem>() { new ComboboxItem(XElement.Parse("<Results><Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected></Results>")) };

            return task.Result.Elements().Select(n => new ComboboxItem(n)).ToList();
        }
        #endregion

        #region Display Message

        private void DisplayMessage(XElement result)
        {
            if (result == null || result.Value == null || result.Value.IsEmpty()) return;

            bool success = result.ToString().ToLower().Contains("success");
            var mess = result.Value;
            MessageBox.Show(mess, (success ? "Success" : "Error"), MessageBoxButton.OK, (success ? MessageBoxImage.Information : MessageBoxImage.Error));
        }


        #endregion

        #endregion

    }
}
