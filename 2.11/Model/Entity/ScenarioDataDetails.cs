using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity
{
    public class ScenarioDataDetails
    {
        public ScenarioDataDetails()
        {
        }

        public ScenarioDataDetails(XElement el)
        {
            Id = el.GetValue<string>("Scen_Idx");
            Name = el.GetValue<string>("Scen_Name");
            if (el.Element("Start_Day_Idx") != null)
            {
                var startDay = DateTime.ParseExact(el.GetValue<string>("Start_Day_Idx"),
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
                StartDate = startDay;
            }
            if (el.Element("End_Day_Idx") != null)
            {
                var endDay = DateTime.ParseExact(el.GetValue<string>("End_Day_Idx"),
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
                EndDate = endDay;
            }
            if (el.Element("Scen_Type_Name") != null)
            {
                ItemType = el.GetValue<string>("Scen_Type_Name");
            }
            if (el.Element("Scen_Status_Name") != null)
            {
                StatusName = el.GetValue<string>("Scen_Status_Name");
            }

            StatusIdx = el.Element("Scen_Status_Idx").MaybeValue();
            IsEditable = el.Element("IsEditable").MaybeValue();
            ItemTypeIdx = el.Element("Scen_Type_Idx").MaybeValue();
            //if (el.Element("Cust_Level_Idx") != null)
            //{
            //    CustomerLevelId = el.GetValue<string>("Cust_Level_Idx");
            //}
            //if (el.Element("Prod_Level_Idx") != null)
            //{
            //    ProductLevelId = el.GetValue<string>("Prod_Level_Idx");
            //}
            if (el.Element("IsEditable") != null)
            {
                ScenarioEditMode = el.GetValue<string>("IsEditable");
            }

            if (el.Element("IsActiveBudget") != null)
            {
                IsActiveBudget = el.GetValue<string>("IsActiveBudget") == "1";
            }

            //if (el.Element("SelectedCustomers") != null)
            //{
            //    SelectedCustomers = el.Elements("SelectedCustomers").Elements("SelectedCustomers").Select(m => m.GetValue<string>("Cust_Idx")).ToList();
            //}
            //if (el.Element("SelectedProducts") != null)//TODO: 2.9 will have some changes brought forward for 2.7 to turn all Prod_Idx to Sku_Idx, this will need to be changed here:
            //{
            //    SelectedProducts = el.Elements("SelectedProducts").Elements("SelectedProducts").Select(m => m.GetValue<string>("Prod_Idx")).ToList();
            //}

            //if (el.Element("SelectedPromotions") != null)
            //{
            //    SelectedPromotions = el.Elements("SelectedPromotions").Select(m => m.GetValue<int>("Promo_Idx")).ToList();
            //}
            //if (el.Element("SelectedROBs") != null)
            //{
            //    SelectedFundings = el.Elements("SelectedROBs").Select(m => m.GetValue<int>("ROB_Idx")).ToList();
            //}
            if(el.Element("SelectedUsers") != null)
            {
                SelectedUsers = el.Elements("SelectedUsers").Elements("SelectedUsers").Select(m => m.GetValue<string>("User_Idx")).ToList();
            }
        }
         
        public DateTime EndDate { get; set; }

        public DateTime StartDate { get; set; }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this PromotionData.
        /// </summary>
        public string Id { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this PromotionData.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region StatusName
        /// <summary>
        /// Gets or sets the StatusName of this PromotionData.
        /// </summary>
        public string StatusName { get; set; }
        #endregion

        public string StatusIdx { get; set; }

        #region ItemType
        public string ItemType { get; set; }
        #endregion

        public string ItemTypeIdx { get; set; }

        //public string CustomerLevelId { get; set; }

        //public string ProductLevelId { get; set; }

        public string ScenarioEditMode { get; set; }

        public bool IsActiveBudget { get; set; }

        //private List<string> _selectedCustomers = new List<string>();
        //public List<string> SelectedCustomers
        //{
        //    get
        //    {
        //        return _selectedCustomers;
        //    }
        //    set
        //    {
        //        if (_selectedCustomers != value)
        //            _selectedCustomers = value.ToList();
        //    }
        //}

        //private List<string> _selectedProducts = new List<string>();
        //public List<string> SelectedProducts
        //{
        //    get
        //    {
        //        return _selectedProducts;
        //    }
        //    set
        //    {
        //        if (_selectedProducts != value)
        //            _selectedProducts = value.ToList();
        //    }
        //}

        private List<int> _selectedPromotions;
        public List<int> SelectedPromotions
        {
            get
            {
                return _selectedPromotions;
            }
            set
            {
                if (_selectedPromotions != value)
                    _selectedPromotions = value.ToList();
            }
        }

        private List<int> _selectedFundings;
        public List<int> SelectedFundings
        {
            get
            {
                return _selectedFundings;
            }
            set
            {
                if (_selectedFundings != value)
                    _selectedFundings = value.ToList();
            }
        }

        private List<string> _selectedUsers;
        public List<string> SelectedUsers
        {
            get
            {
                return _selectedUsers;
            }
            set
            {
                if (_selectedUsers != value)
                    _selectedUsers = value.ToList();
            }
        }

        public string IsEditable { get; set; }
    }
}
