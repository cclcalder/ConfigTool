using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess;
using Model.Entity.Listings;

namespace Model
{
    [Serializable]
    public class Promotion : ICloneable
    {

        public Promotion()
            : this(null, null)
        {
            IsEditable = true;
            ScenarioIsEditable = true;
            Status = 1;  // = EditMode.IsAdded;
        }

        public Promotion(XElement el, PromotionAccess promotionAccess)
        {
            InitializeCollections();

            if (el != null)
            {
                Id = el.GetValue<string>("ID");
                Name = el.GetValue<string>("Name");
                CodeAndName = el.GetValue<string>("CodeAndName");
                URL = el.GetValue<string>("URL");
                Comments = el.GetValue<string>("Comments");
                CustomerIdx = promotionAccess.GetPromotionWizardCustomers(Id).FirstOrDefault(c => c.ID == el.GetValue<string>("Customer")).ID;

                KeepVolumeOperation = BooleanValue(el, "KeepVolumeOperation");

                Scenarios.AddRange(
                    el.GetElement("Scenarios").MaybeElements()
                        .Select(scenarioElement => scenarioElement.GetValue<string>("ID")));

                Products = promotionAccess.GetAddPromotionProducts(Id);

                //if (el.GetElement("Products") != null)
                //{
                //    foreach (var prodid in el.Element("Products").Elements())
                //    {
                //        //var productId = prodid.Value;
                //        //Products.Add(promotionAccess.GetAddPromotionProducts(Id).FirstOrDefault(p => p.ID == productId));
                //        Products = promotionAccess.GetAddPromotionProducts(Id);//.FirstOrDefault(p => p.ID == productId);
                //        //only add product if it is in the product cache, it may not be if it is a parent?
                //        //if (selP != null)
                //        //    Products.Add(selP);
                //    }
                //}

                WizarStartScreenName = el.GetValue<string>("WizardStartScreenName");


                if (el.GetElement("Approvals") != null)
                {
                    var approvals = promotionAccess.GetApprovalLevels(Id);
                    foreach (var a in el.Element("Approvals").Elements())
                    {
                        var appId = a.GetValue<string>("ID");
                        var appValue = a.GetValue<int>("Value");
                        var approval = approvals.SingleOrDefault(l => l.ID == appId);
                        approval.Value = appValue;
                        Approvers.Add(approval);
                    }
                }

                if (el.GetElement("StatusID") != null)
                    Status = el.GetValue<int>("StatusID");

                const string promoIsEditableElement = "PromoIsEditable";
                const string statusIsEditableElement = "StatusIsEditable";
                const string scenarioIsEditableElement = "ScenarioIsEditable";

                IsEditable = BooleanValue(el, promoIsEditableElement);
                StatusIsEditable = BooleanValue(el, statusIsEditableElement);
                ScenarioIsEditable = BooleanValue(el, scenarioIsEditableElement);
            }

        }

        private static bool BooleanValue(XContainer el, string promoIsEditableElement)
        {
            return el.GetElement(promoIsEditableElement) != null && el.GetValue<int>(promoIsEditableElement) == 1;
        }

        public void InitializeCollections()
        {
            Products = new TreeViewHierarchy();
            Dates = new List<PromotionDate>();
            ProductPrices = new List<PromotionProductPrice>();
            Attributes = new List<PromotionAttribute>();
            FinancialVariables = new List<PromotionFinancial>();
            FinancialProductVariables = new List<PromotionProductPrice>();
            Volumes = new List<PromotionVolume>();
            Approvers = new List<PromotionApprovalLevel>();
            Scenarios = new List<string>();
        }

        #region Id
        /// <summary>
        /// Gets or sets the Id of this Promotion.
        /// </summary>
        public string Id { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this Promotion.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region CodeAndName
        /// <summary>
        /// Gets or sets the Name of this Promotion.
        /// </summary>
        public string CodeAndName { get; set; }
        #endregion

        public string URL { get; set; }
        #region Comments
        /// <summary>
        /// Gets or sets the Comments of this Promotion.
        /// </summary>
        public string Comments { get; set; }
        #endregion

        #region Customer
        /// <summary>
        /// Gets or sets the Customer of this Promotion.
        /// </summary>
        public string CustomerIdx { get; set; }

        public List<string>  SubCustomerIdxs { get; set; }
        #endregion

        #region Products
        /// <summary>
        /// Gets or sets the Products of this Promotion.
        /// </summary>        
        public TreeViewHierarchy Products { get; set; }

        #endregion


        #region Dates
        /// <summary>
        /// Gets or sets the Dates of this Promotion.
        /// </summary>
        public List<PromotionDate> Dates { get; set; }
        #endregion

        #region ProductPrices
        /// <summary>
        /// Gets or sets the ProductPrices of this Promotion.
        /// </summary>
        public List<PromotionProductPrice> ProductPrices { get; set; }
        #endregion

        #region Attributes
        /// <summary>
        /// Gets or sets the Attributes of this Promotion.
        /// </summary>
        public List<PromotionAttribute> Attributes { get; set; }
        #endregion

        #region FinancialVariables
        /// <summary>
        /// Gets or sets the FinancialVariables of this Promotion.
        /// </summary>
        public List<PromotionFinancial> FinancialVariables { get; set; }
        #endregion

        #region FinancialProductVariables
        /// <summary>
        /// Gets or sets the FinancialProductVariables of this Promotion.
        /// </summary>
        public List<PromotionProductPrice> FinancialProductVariables { get; set; }
        #endregion

        #region Volumes
        /// <summary>
        /// Gets or sets the Volumes of this Promotion.
        /// </summary>
        public List<PromotionVolume> Volumes { get; set; }

        public List<PromotionDisplayUnit> DisplayUnits { get; set; }
        public List<PromotionVolume> StealVolumes { get; set; }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the Status of this Promotion.
        /// </summary>
        public int Status { get; set; }
        #endregion

        #region Scenario
        /// <summary>
        /// Gets or sets the Scenarios that apply to this Promotion.
        /// </summary>
        public List<string> Scenarios { get; set; }
        #endregion

        #region DatePeriod
        /// <summary>
        /// Gets or sets the date period that this Promotion is based on.
        /// </summary>
        public PromotionDatePeriod DatePeriod { get; set; }
        #endregion


        #region WizarStartScreenName
        /// <summary>
        /// Gets or sets the WizarStartScreenName of this Promotion.
        /// </summary>
        public string WizarStartScreenName { get; set; }
        #endregion

        #region Approvers
        /// <summary>
        /// Gets or sets the Approvers of this Promotion.
        /// </summary>
        public List<PromotionApprovalLevel> Approvers { get; set; }
        #endregion

        #region IsEditable
        /// <summary>
        /// Gets or sets the IsEditable of this Promotion.
        /// </summary>
        public bool IsEditable { get; set; }

        public bool ScenarioIsEditable { get; set; }

        private bool _statusIsEditable = true;
        public bool StatusIsEditable
        {
            get { return _statusIsEditable; }
            set { _statusIsEditable = value; }
        }

        public bool IsReadOnly
        {
            get { return !IsEditable; }
        }

        public string AttributesComment { get; set; }

        #endregion


        public PromotionProduct getParentPromotions(PromotionProduct childPromotion)
        {
            //List<PromotionProduct> thisCollection = new List<PromotionProduct>();

            //var thisParent =
            //        from p in childPromotion
            //        where p.Parent.ID != null 
            //        select p.Parent;

            //thisCollection.Add(childPromotion.Parent);
           
            return childPromotion.Parent;
        }

        public List<string> getParentIDs(PromotionProduct childPromotion)
        {
            
            //List<string> stringToReturn = new List<string>((from parent in this.Products orderby parent.Parent.ID select new string(parent.Parent.ID).ToList<string>() );
            //PromotionProduct parents = 
            //    from p in childPromotion .Parent
            //    where p.ID != null
            //    select p; 

            List<string> stringToReturn = new List<string>();
            List<PromotionProduct> promoList = new List<PromotionProduct>();
            var thisParent =
                from p in Products.FlatTree
                where p.ParentIdx != null
                select p.Parent;

            if (childPromotion.ParentId != null)
            {
                foreach (var parent in thisParent)
                {
                    if (Products.FlatTree.ContainsAll(parent.Children))
                    {
                        parent.IsSelected = "1";
                        //stringToReturn.Add(parent.ID);
                    }
                    if (parent.Children.Where(a => a.IsSelected == "1").Count() == parent.Children.Count())
                    {
                        parent.IsSelected = "1"; 
                    }
                    if (parent.IsSelected == "1")
                    {
                        stringToReturn.Add(parent.Idx);
                    }


                    TreeViewHierarchy currentParent = parent;
                    bool reachedTopOfTree = false;
                        while (reachedTopOfTree == false)
                    {                       
                        currentParent = currentParent.Parent;
                            if (currentParent.Children.Where(a => a.IsSelected == "1").Count() == currentParent.Children.Count())
                        {
                                currentParent.IsSelected = "1";
                        }

                            if (currentParent.IsSelected == "1")
                        {
                            stringToReturn.Add(currentParent.Idx);
                        }
                        if (currentParent.ParentIdx == null)
                        {
                            reachedTopOfTree = true;
                        }
                        
                    }

                    //foreach (var secondParent in getParentIDs(parent).Distinct())
                    //{
                    //    stringToReturn.Add(secondParent);
                    //}


                    //foreach (var element in parent.)  //getParentIDs(parent).Distinct())
                    //{
                    //    stringToReturn.Add(element .ToString());
                    //}

                }
            }
            else
            {
                return null;
            }

            return stringToReturn;
        }



        /// <summary>
        /// Creates Required Save Argument used in data access.
        /// </summary>
        /// <returns></returns>
        public XElement CreateSaveArgument(string page, DateTime? lastSaved,
            Dictionary<string, bool> selectedProducts = null, XElement grid1 = null, XElement grid2 = null, XElement grid3 = null, string attributesComments = null, XElement attachments = null)
        {

            XElement argument = new XElement("SavePromotions");
            argument.Add(new XElement("UserID", User.CurrentUser.ID)); // REQUIRED
            argument.Add(new XElement("ScreenName", page)); // REQUIRED 
            argument.Add(new XElement("LastSaveDate", (lastSaved.HasValue ? lastSaved.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") : ""))); // REQUIRED 
             
            var rootPromoNode = new XElement("Promotion"); // REQUIRED
           
            rootPromoNode.Add(new XElement("ID", this.Id)); // REQUIRED

            if (page == "Customer")
            {
                 rootPromoNode.Add(new XElement("Name", this.Name));
                 rootPromoNode.Add(new XElement("Customer", CustomerIdx));

                 if (SubCustomerIdxs != null)
                {

                    XElement SubCustomersXElement = new XElement("SubLevelCustomers");

                    foreach (var subCust in SubCustomerIdxs)
                    {
                        SubCustomersXElement.Add(new XElement("SubLevelCustomerID", subCust));
                    }
                 
                    rootPromoNode.Add(SubCustomersXElement);
                }
            }
            else if (page=="Dates")
            {
                var dates = rootPromoNode.AddElement("Dates");
                if (this.Dates != null)
                {
                    foreach (var d in this.Dates)
                    {
                        var dateNode = new XElement("DateType");
                        dateNode.Add(new XElement("ID", d.ID));
                        dateNode.Add(new XElement("StartValue", d.StartDate.Date.ToString("yyyy-MM-dd")));
                        dateNode.Add(new XElement("EndValue", d.EndDate.Date.ToString("yyyy-MM-dd")));
                        dates.Add(dateNode);
                    }
                }

                if (DatePeriod != null)
                {
                    rootPromoNode.Add(new XElement("DatePeriod",
                        new XElement("ID", DatePeriod == null ? string.Empty : DatePeriod.ID)));
                }

            }
            else if (page == "Products")
            {

                if (selectedProducts != null)
                {
                    var ProductNode = new XElement("Products"); 

                    foreach (var productNode in selectedProducts.Distinct().Select(product => 
                        new XElement("Product",
                            new XElement("ID", product.Key),
                            new XElement("IsParentNode", (product.Value==true ? "1":"0")))))
                    {
                        ProductNode.Add(productNode);
                    }
                    rootPromoNode.Add(ProductNode);
                }

                if (this.ProductPrices != null)
                {
                    var productPriceNode = new XElement("ProductPrices");
                    
                    foreach (var pp in this.ProductPrices)
                    {                        
                        var productNode = new XElement("Product",
                            new XElement("ID", pp.ID),
                            new XElement("IsFOC", pp.IsFOC ? "1" : "0"),
                            new XElement("IsDisplay", pp.IsDisplay ? "1" : "0")
                            );

                        var productMeasureNode = new XElement("Measures");

                        if (pp.Measures != null)
                            foreach (var m in pp.Measures)
                            {
                                var measureNode = new XElement("Measure");
                                measureNode.Add(new XElement("ID", m.ID));
                                measureNode.Add(new XElement("Value", Common.RemoveFormatForDecimal(m.Value)));
                                productMeasureNode.Add(measureNode);
                            }

                        productNode.Add(productMeasureNode);

                        productPriceNode.Add(productNode);
                    }

                    rootPromoNode.Add(productPriceNode);
                }

            }
            else if(page=="Attributes")
            {
                 
                //if (this.Attributes != null)
                //{
                      if(grid1!=null)  {rootPromoNode.Add(grid1);}

                      if (attributesComments != null)
                          rootPromoNode.Add(new XElement("AttributeComment", attributesComments));
                       
            }
            else if (page == "Volumes")
            {
                rootPromoNode.Add(new XElement("KeepVolumeOperation", this.KeepVolumeOperation ? "1" : "0"));

               
              if(grid1!=null)  {rootPromoNode.Add(grid1);}

              if (grid3 != null) {rootPromoNode.Add(grid3);}

              if (grid2 != null) {rootPromoNode.Add(grid2);}


            }
            else if (page == "Financials")
            {
                 
              if(grid1!=null)  {rootPromoNode.Add(grid1);}

              if (grid3 != null) {rootPromoNode.Add(grid3);}

              if (grid2 != null) {rootPromoNode.Add(grid2);}

 

            }
            else if (page == "Approvals")
            {
                 
                if (this.Approvers != null)
                {
                    var approvalsNode = new XElement("Approvals");

                    foreach (var a in this.Approvers)
                    {
                        var approverNode = new XElement("Approval");
                        approverNode.Add(new XElement("ID", a.ID));
                        approverNode.Add(new XElement("Value", a.Value));

                        approvalsNode.Add(approverNode);
                    }

                    rootPromoNode.Add(approvalsNode);
                }

            }
            else if (page == "Review" || page == "Final")
            {

                if(this.Comments!="") {rootPromoNode.Add(new XElement("Comments", this.Comments));}

                rootPromoNode.Add(new XElement("Status", new XElement("ID", this.Status.ToString())));

                var scenariosElement = new XElement("Scenarios");
                rootPromoNode.Add(scenariosElement);

                foreach (var scenario in Scenarios)
                    scenariosElement.Add(new XElement("Scenario", new XElement("ID", scenario)));

                if (attachments != null) { rootPromoNode.Add(attachments); }

            }
            else
            {
               throw new Exception("Save promotion error - page not found:" + page);
            }
             
            argument.Add(rootPromoNode); 

            return argument;
        }

//        public XElement PowerEditorCreateSaveArgument(Promotion promo)
//        {
//            XElement saveElement = new XElement("SavePromo");

//            saveElement.Add(new XElement("User_Idx", User.CurrentUser.ID));
            
//            //XElement PromoElement = new XElement("Promotion");

//            if (promo.Id != null)
//                saveElement.Add(new XElement("Promo_Idx", promo.Id));



//            saveElement.Add(new XElement("Customer", promo.Customer.ID));

//            //Hackey test
//            //saveElement.Add(new XElement("ScreenName", "Customer"));

//            if (promo.Name != null)
//            {
//                saveElement.Add(new XElement("Name", promo.Name));
//            }
//            //else
//            //{
//            //    PromoElement.Add(new XElement("Name", "test"));
//            //}


//            //if(promo.SelectedSubCustomers != null)
//            //    foreach (var VARIABLE in promo.SelectedSubCustomers)
//            //    {
                    
//            //    }

//            //saveElement.Add(PromoElement);

//            return saveElement;

////            <SavePromotions>
////  <UserID>10</UserID>
////  <ScreenName>Customer</ScreenName>
////  <LastSaveDate></LastSaveDate>
////  <Promotion>
////    <ID />
////    <Name>kjjjj</Name>
////    <Customer>20043</Customer>
////    <SubLevelCustomers />
////  </Promotion>
////</SavePromotions>
//        }

        public XElement PowerEditorCreateSaveArgument(Promotion promo)
        {
            XElement saveElement = new XElement("SavePromotions");

            saveElement.Add(new XElement("UserID", User.CurrentUser.ID));

            XElement PromoElement = new XElement("Promotion");

            if (promo.Id != null)
                PromoElement.Add(new XElement("ID", promo.Id));



            PromoElement.Add(new XElement("Customer", CustomerIdx));

            //Hackey test
            saveElement.Add(new XElement("ScreenName", "Customer"));

            if (promo.Name != null)
            {
                PromoElement.Add(new XElement("Name", promo.Name));
            }
            else
            {
                PromoElement.Add(new XElement("Name", "test"));
            }


            //if(promo.SelectedSubCustomers != null)
            //    foreach (var VARIABLE in promo.SelectedSubCustomers)
            //    {

            //    }

            saveElement.Add(PromoElement);

            return saveElement;

            //            <SavePromotions>
            //  <UserID>10</UserID>
            //  <ScreenName>Customer</ScreenName>
            //  <LastSaveDate></LastSaveDate>
            //  <Promotion>
            //    <ID />
            //    <Name>kjjjj</Name>
            //    <Customer>20043</Customer>
            //    <SubLevelCustomers />
            //  </Promotion>
            //</SavePromotions>
        }


        private void PopulateStealVolumesElement(XElement stealVolumesElement)
        {
            if (this.StealVolumes != null)
            {
                foreach (var v in this.StealVolumes)
                {
                    var volumeNode = new XElement("StealVolume");
                    volumeNode.Add(new XElement("ProductID", v.ProductId));

                    var measures = new XElement("Measures");

                    foreach (var measure in v.Measures)
                    {
                        measures.Add(new XElement("Measure",
                                                  new XElement("ID", measure.ID),
                                                  new XElement("Value", Common.RemoveFormatForDecimal(measure.Value))));
                    }

                    volumeNode.Add(measures);

                    stealVolumesElement.Add(volumeNode);
                }
            }
        }

        private void PopulateVolumesElement(XElement volumesElement)
        {
            PopulateVolumesElement(volumesElement, Volumes);
        }

        public static void PopulateVolumesElement(XElement element, IEnumerable<PromotionVolume> volumes)
        {
            foreach (var v in volumes)
            {
                var volumeNode = new XElement("Volume");
                volumeNode.Add(new XElement("ProductID", v.ProductId));

                var measures = new XElement("Measures");

                foreach (var measure in v.Measures)
                {
                    measures.Add(new XElement("Measure",
                                              new XElement("ID", measure.ID),
                                              new XElement("Value", Common.RemoveFormatForDecimal(measure.Value))));
                }

                volumeNode.Add(measures);

                element.Add(volumeNode);
            }
        }



        private void PopulateDisplayElement(XElement displayElement)
        {
            if (DisplayUnits != null)
            {
                PopulateDisplayElement(displayElement, DisplayUnits); 
            }
        }

        public static void PopulateDisplayElement(XElement element, IEnumerable<PromotionDisplayUnit> displayunits)
        {
            foreach (var v in displayunits)
            {
                var volumeNode = new XElement("DisplayUnit");
                volumeNode.Add(new XElement("ProductID", v.ProductId));

                var measures = new XElement("Measures");

                foreach (var measure in v.Measures)
                {
                    measures.Add(new XElement("Measure",
                                              new XElement("ID", measure.ID),
                                              new XElement("Value", Common.RemoveFormatForDecimal(measure.Value))));
                }

                volumeNode.Add(measures);

                element.Add(volumeNode);
            }
        }


        /// <summary>
        /// Creates required Create PromotionID argument used in data access
        /// </summary>
        /// <returns></returns>
        public XElement CreatePromotionIDArgument()
        {
            XElement argument = new XElement("CreatePromotion");
            argument.Add(new XElement("UserID", User.CurrentUser.ID));
            argument.Add(new XElement("Promotion"));

            XElement CustomerElement = new XElement("Customer");
            CustomerElement.Add(new XElement("ID", CustomerIdx));

            XElement SubCustomersXElement = new XElement("SubLevelCustomers");

            foreach (var subCust in SubCustomerIdxs)
            {

                    SubCustomersXElement.Add(new XElement("SubLevelCustomerID", subCust));
            }
            CustomerElement.Add(SubCustomersXElement);

            argument.Element("Promotion").Add(CustomerElement);
            argument.Element("Promotion").Add(new XElement("Name", this.Name));
            argument.Element("Promotion").Add(new XElement("Comments", this.Name));

            return argument;
        }


        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }
        public Promotion Clone()
        {
            return CreateDeepCopy();
        }

        /// <summary>
        /// Creates a Deep copy of Current promotion
        /// </summary>
        /// <returns></returns>
        Promotion CreateDeepCopy()
        {
            object result = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                                            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(ms, this);

                ms.Position = 0;
                result = bf.Deserialize(ms);
            }

            return result as Promotion;
        }
        #endregion
        
        public bool KeepVolumeOperation { get; set; }
         
    }
  
 
}