using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    public class PromotionApprovalLevel
    {
        public PromotionApprovalLevel()
        { }

        public PromotionApprovalLevel(XElement el)
        {
            if (el != null)
            {
                ID = el.GetValue<string>("ID");
                Name = el.GetValue<string>("Name");
                Value = el.GetValue<int>("Value");
                Enabled = el.GetValue<int>("IsEnabled") == 1 ? true : false;
            }
        }

        #region ID
        /// <summary>
        /// Gets or sets the ID of this PromotionApprover.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this PromotionApprover.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Enabled
        /// <summary>
        /// Gets or sets the Ednabled of this PromotionApprover.
        /// </summary>
        public bool Enabled { get; set; }
        #endregion

        #region Value
        /// <summary>
        /// Gets or sets the Value of this PromotionApprover.
        /// </summary>
        public int Value { get; set; }
        #endregion

    }
}
