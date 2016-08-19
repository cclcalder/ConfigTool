using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.DataAccess
{
    using Model.Entity;
    using Exceedra.Common.Mvvm;
    using System.Threading.Tasks;

    public class PromotionTemplateAccess
    {

        private const string PromotionQueryTemplate = "<{0}><UserID>{1}</UserID><PromotionID>{2}</PromotionID></{0}>";

        #region Cached Collections
        private static readonly AutoCache<string, PromotionData> TemplateDataCache = new AutoCache<string, PromotionData>(pd => pd.ID + pd.ItemType);
        private static readonly AutoCache<string, PromotionCustomer> TemplateCustomersCache = new AutoCache<string, PromotionCustomer>(pc => pc.ID);
        private static readonly AutoCache<string, PromotionStatus> TemplateStatusesCache = new AutoCache<string, PromotionStatus>(ps => ps.ID);
        private static readonly AutoCache<string, PromotionDate> TemplateDatesCache = new AutoCache<string, PromotionDate>(pd => pd.ID);
        private static readonly AutoCache<string, PromotionProduct> TemplateProductsCache = new AutoCache<string, PromotionProduct>(pp => pp.ID);
        private static readonly AutoCache<string, PromotionProductPrice> TemplateProductPricesCache = new AutoCache<string, PromotionProductPrice>(ppp => ppp.ID);
        private static readonly AutoCache<string, PromotionProductPrice> TemplatePromotionProductPricesCache = new AutoCache<string, PromotionProductPrice>(ppp => ppp.ID);
        private static readonly AutoCache<string, PromotionAttribute> TemplateAttributesCache = new AutoCache<string, PromotionAttribute>(pa => pa.ID);
        private static readonly AutoCache<string, PromotionVolume> TemplateVolumesCache = new AutoCache<string, PromotionVolume>(pv => pv.ProductId);

        private static readonly AutoCache<string, PromotionFinancial> TemplateFinancialCache = new AutoCache<string, PromotionFinancial>(pf => pf.ID);

        private static readonly AutoCache<string, Customer> TemplateWizardCustomersCache = new AutoCache<string, Customer>(pc => pc.ID);

        #endregion



        static PromotionTemplateAccess()
        {

            // PromotionDataMapping.SetUp();
        }

        private static void ResetTemplateStaticCaches()
        {
            TemplateDataCache.Clear();
            TemplateCustomersCache.Clear();
            TemplateStatusesCache.Clear();
            TemplateDatesCache.Clear();
            TemplateProductsCache.Clear();
            TemplateProductPricesCache.Clear();
            TemplatePromotionProductPricesCache.Clear();
            TemplateAttributesCache.Clear();
            TemplateVolumesCache.Clear();
            TemplateFinancialCache.Clear();
            TemplateWizardCustomersCache.Clear();
        }

        public void ResetCache()
        {
            ResetTemplateStaticCaches();
        }

        public Task<IList<PromotionData>> GetTemplateDataAsync(IEnumerable<string> statusIDs, IEnumerable<string> customerIDs,
            IEnumerable<string> productsIDs,
             DateTime startDate, DateTime endDate)
        {
            var argument = new XElement("GetTemplates");
            var userNode = new XElement("User_Idx");
            userNode.SetValue(User.CurrentUser.ID);
            argument.Add(userNode);

            var statuses = new XElement("Statuses");
            argument.Add(statuses);

            var customers = new XElement("Customers");
            argument.Add(customers);

            var products = new XElement("Products");
            argument.Add(products);

            argument.Add(new XElement("Start", startDate.Date.ToString("yyyy-MM-dd")));
            argument.Add(new XElement("End", endDate.Date.ToString("yyyy-MM-dd")));

            foreach (var s in statusIDs.Distinct())
            {
                statuses.Add(new XElement("Idx", s));
            }

            foreach (var c in customerIDs.Distinct())
            {
                customers.Add(new XElement("Idx", c));
            }

            foreach (var c in productsIDs.Distinct())
            {
                products.Add(new XElement("Idx", c));
            }

            return WebServiceProxy.CallAsync(StoredProcedure.Template.GetTemplates, argument).ContinueWith(t => GetTemplateDataAsyncContinuation(t));

        }

        public Task<IList<PromotionData>> GetTemplateDataAsync(XElement args)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Template.GetTemplates, args).ContinueWith(t => GetTemplateDataAsyncContinuation(t));
        }

        private IList<PromotionData> GetTemplateDataAsyncContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<PromotionData>();
            return (task.Result.Elements().Select(n => new PromotionData(n))).ToList();

        }
         
        //public Task<IList<PromotionData>> GetPromotionDataAsync(IEnumerable<string> statusIDs, IEnumerable<string> customerIDs,
        //    DateTime startDate, DateTime endDate)
        //{
        //    var argument = new XElement("GetPromotions");
        //    var userNode = new XElement("UserID");
        //    userNode.SetValue(User.CurrentUser.ID);
        //    argument.Add(userNode);

        //    var statuses = new XElement("Statuses");
        //    argument.Add(statuses);
        //    var customers = new XElement("Customers");
        //    argument.Add(customers);

        //    argument.Add(new XElement("Start", startDate.Date.ToString("yyyy-MM-dd")));
        //    argument.Add(new XElement("End", endDate.Date.ToString("yyyy-MM-dd")));

        //    foreach (var s in statusIDs)
        //    {
        //        statuses.Add(new XElement("ID", s));
        //    }

        //    foreach (var c in customerIDs)
        //    {
        //        customers.Add(new XElement("ID", c));
        //    }

        //    return WebServiceProxy.CallAsync(StoredProcedure.Template.GetPromotionData, argument).ContinueWith(t => GetPromotionDataContinuation(t));

        //}

        //private IList<PromotionData> GetPromotionDataContinuation(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //        return new List<PromotionData>();
        //    return TemplateDataCache.CacheRange(task.Result.Elements().Select(n => new PromotionData(n))).ToList();

        //}

        public Task<XElement> GetAddPromotionTemplateCustomersAsync(string promotionId)
        {
            string arguments = "<GetAddPromotionCustomers><UserID>{0}</UserID><PromotionID>{1}</PromotionID><SalesOrg_Idx>{2}</SalesOrg_Idx></GetAddPromotionCustomers>"
                                .FormatWith(User.CurrentUser.ID, promotionId, User.CurrentUser.SalesOrganisationID);

            return WebServiceProxy.CallAsync(StoredProcedure.Template.GetCustomers, XElement.Parse(arguments)).ContinueWith(t => GetAddPromotionCustomersContinuation(t));

            // return result;
        }

        private XElement GetAddPromotionCustomersContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return XElement.Parse("<Result></Result>");

            return task.Result;

        }


        public IEnumerable<Customer> GetPromotionTemplateCustomers(string promotionId)
        {
            return GetPromotionTemplateCustomersCached(promotionId);
        }

        private static IEnumerable<Customer> GetPromotionTemplateCustomersCached(string promotionId)
        {

            string arguments = "<GetAddPromotionCustomers><UserID>{0}</UserID><PromotionID>{1}</PromotionID> </GetAddPromotionCustomers>"
                         .FormatWith(User.CurrentUser.ID, promotionId);

            var result = WebServiceProxy.Call(StoredProcedure.Template.GetCustomers, XElement.Parse(arguments));

            return (result.Elements().Select(c => new Customer(c))).ToList();

        }

        /// <summary>
        /// Return flat list of all sub customers
        /// </summary>
        /// <param name="promotionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Customer> GetSubCustomers(string promotionId, string userId)
        {
            string xml =
                string.Format(
                    @"  <TemplateGetCustomersSubLevel>
                            <User_Idx>{1}</User_Idx>
                            <Promo_Idx>{0}</Promo_Idx>                           
                          </TemplateGetCustomersSubLevel>",
                    promotionId, userId);

            var argument = XElement.Parse(xml);
            var nodes = WebServiceProxy.Call(StoredProcedure.Template.GetCustomersSubLevel, argument, DisplayErrors.No);
            return Customer.FromXml(nodes).ToList();
        }

        /// <summary>
        /// Return flat list of all sub customers
        /// </summary>
        /// <param name="promotionId"></param>
        /// <param name="userId"></param>
        /// /// <param name="customersIds"></param>
        /// <returns></returns>
        public IEnumerable<Customer> GetSubCustomers(string promotionId, string userId, IEnumerable<string> customersIds)
        {
            string xml =
                string.Format(
                    @"  <TemplateGetCustomersSubLevel>
                            <User_Idx>{1}</User_Idx>
                            <Promo_Idx>{0}</Promo_Idx>     
                          </TemplateGetCustomersSubLevel>",
                    promotionId, userId);

            var argument = XElement.Parse(xml);

            if (customersIds.Any())
            {
                XElement xCsvSelectedCustomers = new XElement("CSVSelectedCustomers");
                foreach (var nodeId in customersIds)
                    xCsvSelectedCustomers.Add(new XElement("Cust_Code", nodeId));
                argument.Add(xCsvSelectedCustomers);
            }

            var nodes = WebServiceProxy.Call(StoredProcedure.Template.GetCustomersSubLevel, argument, DisplayErrors.No);
            return Customer.FromXml(nodes).ToList();
        }

        /// <summary>
        /// Retrieves list of Promotion Status from underlaying service manager
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PromotionStatus> GetPromotionStatuses()
        {

            string arguments = "<GetPromotionStatuses><User_Idx>{0}</User_Idx>".FormatWith(User.CurrentUser.ID) +
                               "<IncludeTemplateStatuses>1</IncludeTemplateStatuses>" +
                               "<IncludePromotionStatuses>0</IncludePromotionStatuses>" +
                               "<ReturnAsList>1</ReturnAsList>" +
                               "</GetPromotionStatuses>";


            var nodes = WebServiceProxy.Call(StoredProcedure.Template.GetFilterStatuses, XElement.Parse(arguments)).Elements();
            //var nodes = WebServiceProxy.Call("app.Procast_SP_TEMPLATE_Wizard_Review_GetWorkflowStatuses", XElement.Parse(arguments)).Elements();
            return nodes.Select(n => new PromotionStatus(n)).ToList();

        }
        /// <summary>
        /// Retrieves list of Promotion Dates from underlaying service manager
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PromotionDate> GetPromotionDates(string promotionId, bool useCache = true)
        {

            string arguments = "<GetPromotionDateTypes><UserID>{0}</UserID><PromotionID>{1}</PromotionID></GetPromotionDateTypes>"
                                .FormatWith(User.CurrentUser.ID, promotionId);

            var dateNodes = WebServiceProxy.Call(StoredProcedure.Template.GetDateTypes, XElement.Parse(arguments)).Elements();
            return (dateNodes.Select(d => new PromotionDate(d))).ToList();

        }


        /// <summary>
        /// Retrieves list of Promotion attribute from underlaying service manager
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<PromotionAttribute> GetPromotionAttributes(Promotion promotion, bool useCache = true)
        //{
        //    if (TemplateAttributesCache.Count == 0 || !useCache)
        //    {
        //        string arguments = "<GetAddPromotionAttributesData><UserID>{0}</UserID><PromotionID>{1}</PromotionID></GetAddPromotionAttributesData>"
        //                            .FormatWith(User.CurrentUser.ID, promotion.Id);

        //        var xml = WebServiceProxy.Call(StoredProcedure.Template.GetAddPromotionAttributesData, XElement.Parse(arguments));
        //        var nodes = xml.Elements("Attribute");
        //        TemplateAttributesCache.CacheRange(nodes.Select(n => new PromotionAttribute(n)));
        //        TemplateAttributesCache.SetExtraValue("Comment", xml.Element("Comment").MaybeValue());
        //    }

        //    promotion.AttributesComment = TemplateAttributesCache.GetExtraValue("Comment");
        //    return TemplateAttributesCache;
        //}

        public IEnumerable<PromotionProduct> GetAddPromotionProducts(string promotionId, bool useCache = true)
        {
            if (TemplateProductsCache.Count == 0 || !useCache)
            {
                TemplateProductsCache.Clear();
                string arguments = "<GetAddPromotionProducts><UserID>{0}</UserID><PromotionID>{1}</PromotionID></GetAddPromotionProducts>"
                                    .FormatWith(User.CurrentUser.ID, promotionId);

                var nodes = WebServiceProxy.Call(StoredProcedure.Template.GetProducts, XElement.Parse(arguments), DisplayErrors.No).Elements();

                TemplateProductsCache.CacheRange(nodes.Select(n => new PromotionProduct(n, TemplateProductsCache)));
            }

            return TemplateProductsCache;
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

                var nodes = WebServiceProxy.Call(StoredProcedure.Template.GetProductPrices, argument, DisplayErrors.No).Elements();
                TemplateProductPricesCache.CacheRange(nodes.Select(n => new PromotionProductPrice(n)));
            }

            return TemplateProductPricesCache.Where(ppp => productIdList.Contains(ppp.ID));
        }


        public XElement GetPromotionFinancialPromoMeasures(string promotionId)
        {
            {

                var argument = new XElement("GetFinancialsPromoMeasures");
                argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
                argument.Add(new XElement("Promo_Idx", promotionId));

                return WebServiceProxy.Call(StoredProcedure.Template.GetPromoMeasures, argument, DisplayErrors.No);

            }

        }

        public XElement GetPromotionFinancialParentProductMeasures(string promotionId)
        {
            {

                var argument = new XElement("GetFinancialsPromoMeasures");
                argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
                argument.Add(new XElement("Promo_Idx", promotionId));


                return WebServiceProxy.Call(StoredProcedure.Template.GetParentProductMeasures, argument, DisplayErrors.No);


            }

        }

        public XElement GetFinancialScreenPlanningSkuMeasure(string promotionId)
        {
            {

                var argument = new XElement("GetFinancialsPromoMeasures");
                argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
                argument.Add(new XElement("Promo_Idx", promotionId));


                return WebServiceProxy.Call(StoredProcedure.Template.GetProductMeasures, argument, DisplayErrors.No);

            }

        }



        /// <summary>
        /// Sends created promotionData back to the web service and gets an Url to return to the page
        /// </summary>
        /// <returns></returns>
        public PromotionSaveResults SavePromotion(PromotionTemplate promotion, DateTime? lastSaved, string page,
            Dictionary<string, bool> selectedProducts = null, XElement volumes = null, XElement displayVolumes = null, XElement stealVolumes = null)
        {
            XElement argument = promotion.CreateSaveArgument(page, lastSaved, selectedProducts, volumes, displayVolumes, stealVolumes, promotion.AttributesComment);

            var results = new PromotionSaveResults(WebServiceProxy.Call(StoredProcedure.Template.SaveTemplate, argument)); //Procast_SP_PROMO_SavePromotion

            return results;
        }

        /// <summary>
        /// Use selected IDs to create a promotion from a templates
        /// </summary>
        /// <param name="selectedIds"></param>
        /// <returns></returns>
        public XElement ApplyTemplate(List<string> selectedIds)
        {
            var xdoc3 = new XDocument(
                              new XElement("ApplyTemplate",

                                      new XElement("User_Idx", Model.User.CurrentUser.ID),

                                      new XElement("Templates",
                                          from pp in selectedIds
                                          select new XElement("Template_Idx", pp)
                                              )
                                          )
                                      );

            var argument = xdoc3.ToString();
            var results = WebServiceProxy.Call(StoredProcedure.Template.ApplyTemplate, argument);
            return results;
        }



        /// <summary>
        /// Creates and returns a Promotion based on given Id
        /// </summary>
        /// <param name="promoId"></param>
        /// <param name="page"></param>
        /// <param name="lastSaved"></param>
        /// <returns></returns>
        public PromotionGetResults GetTemplate(string promoId, string page, DateTime? lastSaved)
        {
            XElement argument = new XElement("GetTemplate");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("PromotionID", promoId));
            argument.Add(new XElement("ScreenName", page)); // REQUIRED 
            argument.Add(new XElement("LastSaveDate", (lastSaved.HasValue ? lastSaved.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : ""))); // REQUIRED 

            var promotion = new PromotionGetResults(WebServiceProxy.Call(StoredProcedure.Template.GetTemplate, argument));

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

            var node = WebServiceProxy.Call(StoredProcedure.Template.CopyTemplate, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        public Task<XElement> GetPromotionAttributesAsync(string promotionId, string proc)
        {
            var argument = new XElement("GetPromotionAttributes");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Promo_Idx", promotionId));

            return WebServiceProxy.CallAsync(proc, argument, DisplayErrors.Yes)
                .ContinueWith(t => GetPromotionAttributesAsyncContinuation(t));
        }

        private XElement GetPromotionAttributesAsyncContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            return task.Result;

        }



        public string UpdateStatusPromotions(string[] Ids, string StatusID)
        {
            XElement argument = new XElement("UpdateTemplateStatus");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Target_Status_Idx", StatusID));
            argument.Add(new XElement("Templates"));

            foreach (var id in Ids)
            {
                argument.Element("Templates").Add(new XElement("Promo_Idx", id));
            }

            var node = WebServiceProxy.Call(StoredProcedure.Template.UpdateMultipleTemplateStatus, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        //public Task<PromotionTemplate> GetPromotionAsync(string promoId)
        //{
        //    XElement argument = new XElement("GetTemplate");
        //    argument.Add(new XElement("UserID", User.CurrentUser.ID));
        //    argument.Add(new XElement("PromotionID", promoId));

        //    return WebServiceProxy.CallAsync(StoredProcedure.Template.GetterGetTemplate, argument).ContinueWith(t => GetPromotionContinuation(t));

        //}

        private PromotionTemplate GetPromotionContinuation(Task<XElement> task)
        {

            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new PromotionTemplate();

            var promotion = new PromotionTemplate(task.Result.Elements().FirstOrDefault(), this);

            return promotion;

        }

        public Task<XElement> GetAddPromotionWizardCustomersAsync(string promotionId)
        {
            string arguments = "<GetAddPromotionCustomers><UserID>{0}</UserID><PromotionID>{1}</PromotionID> </GetAddPromotionCustomers>"
                                .FormatWith(User.CurrentUser.ID, promotionId);

            return WebServiceProxy.CallAsync(StoredProcedure.Template.GetCustomers, XElement.Parse(arguments)).ContinueWith(t => GetAddPromotionCustomersContinuation(t));

            // return result;
        }


        //private static IEnumerable<T> GetPromotionElements<T>(PromotionTemplate currentPromotion,
        //    string elementName, string procName, Func<XElement, T> xmlTranslate)
        //{
        //    var currentPromotionId = currentPromotion != null ? currentPromotion.Id : string.Empty;
        //    var arguments = PromotionQueryTemplate.FormatWith(elementName, User.CurrentUser.ID, currentPromotionId);

        //    var nodes = WebServiceProxy.Call(procName, XElement.Parse(arguments), DisplayErrors.No).Elements();
        //    return nodes.Select(xmlTranslate).ToList();
        //}

        /// <summary>
        /// Returns list of added comments for a given Promotion
        /// </summary>
        /// <param name="promoId"></param>
        /// <returns></returns>
        public IEnumerable<PromotionComment> GetPromotionComments(string promoId)
        {
            string arguments = "<GetTemplateComments><User_Idx>{0}</User_Idx><Template_Idx>{1}</Template_Idx></GetTemplateComments>"
                                .FormatWith(User.CurrentUser.ID, promoId);

            try
            {
                var nodes = WebServiceProxy.Call(StoredProcedure.Template.GetTemplateComments, XElement.Parse(arguments), DisplayErrors.No).Elements();
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
            XElement argument = new XElement("AddTemplateComments");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("Template_Idx", promoId));
            argument.Add(new XElement("Comment", comment));

            var node = WebServiceProxy.Call(StoredProcedure.Template.AddComment, argument).Elements().FirstOrDefault();

            return node.Value;
        }

        /// <summary>
        /// Deletes a promotion by given Id
        /// </summary>
        /// <param name="promoId"></param>
        /// <returns></returns>
        public string DeleteTemplatePromotion(string[] Ids)
        {
            XElement argument = new XElement("DeletePromotion");


            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("Promotions"));

            foreach (var id in Ids)
            {
                argument.Element("Promotions").Add(new XElement("ID", id));
            }



            //DisplayErrors displayErrors = DisplayErrors.No;

            var node = WebServiceProxy.Call(StoredProcedure.Template.DeleteTemplates, argument, DisplayErrors.No).Elements().FirstOrDefault();

            return node.Value;
        }

        public Task<XElement> GetTemplateDashboardXAsync(string promotionId, string proc, string element = "GetPromotionPandLGrid", string tag = "Promo_Idx")
        {
            var argument = new XElement(element);
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement(tag, promotionId));

            return WebServiceProxy.CallAsync(proc, argument, DisplayErrors.No)
                .ContinueWith(t => GetPromotionTemplateXContinuation(t));
        }

        private XElement GetPromotionTemplateXContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            return task.Result;

        }

        /// <summary>
        /// Returns Workflow Statuses to be used on Promotion wizard
        /// </summary>
        /// <param name="promotion">The currently selected promotion; may be <c>null</c>.</param>
        /// <returns></returns>
        public IEnumerable<PromotionStatus> GetPromotionWorkflowStatuses(PromotionTemplate promotion)
        {
            const string elementName = "GetPromotionStatuses";
            return GetPromotionElements(promotion, elementName, StoredProcedure.Template.GetWorkflowStatuses, n => new PromotionStatus(n));
        }

        private static IEnumerable<T> GetPromotionElements<T>(PromotionTemplate currentPromotion,
         string elementName, string procName, Func<XElement, T> xmlTranslate)
        {
            var currentPromotionId = currentPromotion != null ? currentPromotion.Id : string.Empty;
            var arguments = PromotionQueryTemplate.FormatWith(elementName, User.CurrentUser.ID, currentPromotionId);

            var nodes = WebServiceProxy.Call(procName, XElement.Parse(arguments), DisplayErrors.No).Elements();
            return nodes.Select(xmlTranslate).ToList();
        }


        public IEnumerable<ConstraintType> GetModalProductDataList(string promotionId, List<string> skuIds)
        {

            //<GetConstraintTypes>
            //  <User_Idx>2</User_Idx>
            //  <Template_Idx>2</Template_Idx>
            //</GetConstraintTypes>


            XElement skus = new XElement("Products");
            foreach (string Id in skuIds)
            {
                skus.Add(new XElement("Sku_Idx", Id));
            }

            string arguments = "<GetConstraintTypes><User_Idx>{0}</User_Idx><Template_Idx>{1}</Template_Idx>{2}</GetConstraintTypes>"
                .FormatWith(User.CurrentUser.ID, promotionId, skus);

            var productResults = WebServiceProxy.Call(StoredProcedure.Template.GetConstraintTypes, XElement.Parse(arguments)).Elements();

            return new ObservableCollection<ConstraintType>(productResults.Select(n => new ConstraintType(n)));


        }

        public XElement GetDynamicConstraintsGrid(string PromoId, string ConstraintType_Idx)
        {
            //<GetConstraints>
            //<User_Idx>2</User_Idx>
            //<Template_Idx>2</Template_Idx>
            //<ConstraintType_Idx>1</ConstraintType_Idx>
            //</GetConstraints>

            string arguments = "<GetConstraints><User_Idx>{0}</User_Idx><Template_Idx>{1}</Template_Idx><ConstraintType_Idx>{2}</ConstraintType_Idx></GetConstraints>"
                .FormatWith(User.CurrentUser.ID, PromoId, ConstraintType_Idx);

            return WebServiceProxy.Call(StoredProcedure.Template.GetConstraints, XElement.Parse(arguments));
        }

        public XElement ModalDynaicGridSave(string promoId, string recordVmXelements)
        {
            string arguments = "<SaveConstraints><User_Idx>{0}</User_Idx><Template_Idx>{1}</Template_Idx>{2}</SaveConstraints>"
                .FormatWith(User.CurrentUser.ID, promoId, recordVmXelements);

            var result = WebServiceProxy.Call(StoredProcedure.Template.SaveConstraints, XElement.Parse(arguments), DisplayErrors.Yes);

            CommandManager.InvalidateRequerySuggested();

            return result;
        }

    }
}
