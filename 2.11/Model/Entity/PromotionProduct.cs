using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess;
using Telerik.Windows.Controls;

namespace Model
{
    [Serializable()]
    public class PromotionProduct
    {
        /// <summary>
        /// Creates a new Product instance.
        /// </summary>
        private AutoCache<string, PromotionProduct> _promotionList;

        public PromotionProduct(XElement c, AutoCache<string, PromotionProduct> promotionList)
        {
            ID = c.GetValue<string>("Idx");
            DisplayName = c.GetValue<string>("Name");
            ParentId = c.GetValue<string>("ParentIdx");
            IsSelected = c.GetValue<string>("IsSelected");
            IsParentNode = c.Element("IsParentNode").MaybeValue() == "1";
            _promotionList = promotionList;
            MissingData = c.GetValue<string>("Mising_Data");
            if (MissingData != null)
            {
                if (MissingData == "0")
                {
                    MissingDataBool = false;
                }
                if (MissingData == "1")
                {
                    MissingDataBool = true;
                }
            }
        }

        public PromotionProduct(XElement c)
        {
            ID = c.GetValue<string>("ID");
            DisplayName = c.GetValue<string>("DisplayName");
            ParentId = c.GetValue<string>("ParentID");
            IsSelected = c.GetValue<string>("IsSelected");
            IsParentNode = c.Element("IsParentNode").MaybeValue() == "1";

            if (ID == null)
            {
                ID = c.GetValue<string>("Sku_Idx");
            }
            DisplayName = c.GetValue<string>("DisplayName");
            if (DisplayName == null)
            {
                DisplayName = c.GetValue<string>("Sku_Name");
            }
            ParentId = c.GetValue<string>("ParentID");
            IsSelected = c.GetValue<string>("IsSelected");
            MissingData = c.GetValue<string>("Missing_Data");
            if (MissingData != null)
            {
                if (MissingData == "0")
                {
                    MissingDataBool = false;
                    CompleteDataEnum = CompleteData.On;
                }
                if (MissingData == "1")
                {
                    MissingDataBool = true;
                    CompleteDataEnum = CompleteData.Off;
                }
            }
        }

        private CompleteData m_completeDataEnum;
        public CompleteData CompleteDataEnum
        {
            get { return m_completeDataEnum; }
            set { m_completeDataEnum = value; }
        }

        public enum CompleteData
        {
            Off = 0,
            On = 1,
            Intermediate = 2
        };

        public PromotionProduct()
        {
        }


        public AutoCache<string, PromotionProduct> PromotionList
        {
            get { return _promotionList; }
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this Product.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region DisplayName
        /// <summary>
        /// Gets or sets the DisplayName of this Product.
        /// </summary>
        public string DisplayName { get; set; }
        #endregion

        #region ParentId
        /// <summary>
        /// Gets or sets the ParentId of this Product.
        /// </summary>
        public string ParentId { get; set; }
        #endregion

        #region IsSelected
        /// <summary>
        /// Gets or sets the IsSelected of this Product.
        /// </summary>
        private string m_isSelected;
        public string IsSelected
        {
            get { return m_isSelected;}
            set { m_isSelected = value; } 
        }
        #endregion

        #region Children
        /// <summary>
        /// Gets or sets the Children of this Customer.
        /// </summary>
        private List<PromotionProduct> _Children;
        public List<PromotionProduct> Children
        {
            get
            {

                var products = PromotionList.EmptyIfNull();
                products = products.Where(p => p.ParentId == ID);
                _Children = products.ToList();
                return _Children;
            }
        }
        #endregion

        #region Parent
        /// <summary>
        /// Gets or sets the Parent of this Customer.
        /// </summary>
        private PromotionProduct _parent;
        public PromotionProduct Parent
        {
            get
            {
                if (_parent == null && ID != "0")
                {
                    _parent = new PromotionProduct();
                    _parent = PromotionList.SingleOrDefault(p => p.ID == ParentId);
                }
                return _parent;
            }
            set { _parent = value; }
        }

        public bool IsParentNode { get; set; }

        #endregion

        #region Missing Data

        private string m_missingData;
        public string MissingData
        {
            get { return m_missingData; }
            set { m_missingData = value; }
        }

        private bool m_MissingDataBool;
        public bool MissingDataBool
        {
            get { return m_MissingDataBool; }
            set { m_MissingDataBool = value; }
        }
        #endregion
    }
}
