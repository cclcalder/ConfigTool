using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    public class PromotionComment
    {
        public PromotionComment() { }
        public PromotionComment(XElement el)
        {
            if (el != null)
            {
                TimeStamp = el.GetValue<string>("TimeStamp").TryParseAs<DateTime>().Value.ToString("dd-MM-yyyy HH:mm");
                UserName = el.GetValue<string>("UserName");
                Value = el.GetValue<string>("Value");
            }
        }

        #region TimeStamp
        /// <summary>
        /// Gets or sets the DateCreated of this PromotionComment.
        /// </summary>
        public string TimeStamp { get; set; }
        #endregion

        #region UserName
        /// <summary>
        /// Gets or sets the UserName of this PromotionComment.
        /// </summary>
        public string UserName { get; set; }
        #endregion

        #region Value
        /// <summary>
        /// Gets or sets the Description of this PromotionComment.
        /// </summary>
        public string Value { get; set; }
        #endregion

       
        #region Header
        /// <summary>
        /// Gets or sets the Header of this PromotionComment.
        /// </summary>
        public string Header { get { return "[" + TimeStamp + " " + UserName + "]"; } }
        #endregion



    }
}
