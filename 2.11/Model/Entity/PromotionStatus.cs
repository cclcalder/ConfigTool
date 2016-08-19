using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    [Serializable()]
    public class PromotionStatus
    {
        public PromotionStatus(XElement el)
        {
            ID = el.GetValue<string>("ID");
            ParentId = el.GetValue<string>("ParentId");
            Name = el.GetValue<string>("Name");
            IsSelected = el.GetValue<int>("IsSelected") == 1 ? true : false;
            IsEnabled = el.GetValue<int>("IsEnabled") == 1 ? true : false;
            Colour = el.GetValue<string>("Colour");
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this PromotionStatus.
        /// </summary>
        public string ID { get; set; }
        public string ParentId { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this PromotionStatus.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region IsSelected
        /// <summary>
        /// Gets or sets the IsSelected of this PromotionStatus.
        /// </summary>
        public bool IsSelected { get; set; }
        #endregion

        #region IsEnabled
        /// <summary>
        /// Gets or sets the IsEnabled of this PromotionStatus.
        /// </summary>
        public bool IsEnabled { get; set; }
        #endregion

       public string Colour { get; set; }
    }
}
