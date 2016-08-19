using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Converters;
using System.Xml.Linq;

namespace Model
{    
    public class PromotionData
    {
        public PromotionData()
        {
        }

        public PromotionData(XElement el)
        {
            ID = el.GetValue<string>("ID");
            Name = el.GetValue<string>("Name");
            Comments = el.GetValue<string>("Comments");
            CustomerName = el.Element("Customer").GetValue<string>("Name");
            CustomerColor = el.Element("Customer").GetValue<string>("Color");
            StatusName = el.Element("Status").GetValue<string>("Name");
            StatusColor = el.Element("Status").GetValue<string>("Color");

            
            if (el.Element("Attributes") != null)
            {
                ExtraDataList = el.Element("Attributes").Elements().Select(a => new PromotionExtraData(a)).ToList();
            }

            if (el.Element("ItemType") != null)
            {
                ItemType = el.Element("ItemType").Value;
            }
            CanOpenUsingPowerEditor = el.GetValue<string>("CanOpenUsingPowerEditor") == "1";
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this PromotionData.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this PromotionData.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region CustomerName
        /// <summary>
        /// Gets or sets the CustomerName of this PromotionData.
        /// </summary>
        public string CustomerName { get; set; }
        #endregion

        #region CustomerColor
        /// <summary>
        /// Gets or sets the CustomerColor of this PromotionData.
        /// </summary>
        public string CustomerColor { get; set; }
        #endregion

        #region StatusName
        /// <summary>
        /// Gets or sets the StatusName of this PromotionData.
        /// </summary>
        public string StatusName { get; set; }
        #endregion

        public string ItemType { get; set; }

        #region StatusColor
        /// <summary>
        /// Gets or sets the StatusColor of this PromotionData.
        /// </summary>
        public string StatusColor { get; set; }
        #endregion

        #region ExtraData  
        /// <summary>
        /// Gets or sets the Attributes of this PromotionData.
        /// </summary>
        private List<PromotionExtraData> _extraDataList;
        public List<PromotionExtraData> ExtraDataList
        {
            get
            {
                return _extraDataList;
            }
            set
            {
                if (_extraDataList != value)
                {
                    _extraDataList = value.ToList();
                }
            }
        }
        #endregion       

        #region Comments
        /// <summary>
        /// Gets or sets the Comments of this PromotionData.
        /// </summary>
        public string Comments { get; set; }
        #endregion

        public bool IsSelected { get; set; }

        public bool CanOpenUsingPowerEditor { get; set; }
    }
}
