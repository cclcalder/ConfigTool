using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    [Serializable()]
    public class PromotionAttributeOption : IEquatable<PromotionAttributeOption>
    {
        public bool Equals(PromotionAttributeOption other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Text, other.Text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PromotionAttributeOption) obj);
        }

        public override int GetHashCode()
        {
            return (Text != null ? Text.GetHashCode() : 0);
        }

        public static bool operator ==(PromotionAttributeOption left, PromotionAttributeOption right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PromotionAttributeOption left, PromotionAttributeOption right)
        {
            return !Equals(left, right);
        }

        public PromotionAttributeOption()
        {
            
        }
        public PromotionAttributeOption(XElement el)
        {
            Text = el.GetValue<string>("Text");
            Value = el.GetValue<string>("Value");
            IsSelected = el.GetValue<int>("IsSelected") == 1 ? true : false;
        }

        #region Text
        /// <summary>
        /// Gets or sets the Text of this PromotionAttributeOption.
        /// </summary>
        public string Text { get; set; }
        #endregion

        #region Value
        /// <summary>
        /// Gets or sets the Value of this PromotionAttributeOption.
        /// </summary>
        public string Value { get; set; }
        #endregion

        #region IsSelected
        /// <summary>
        /// Gets or sets the IsSelected of this PromotionAttribute.
        /// </summary>
        public bool IsSelected { get; set; }
        #endregion

    }
}
