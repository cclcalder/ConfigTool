using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ClaimValueRange
    {
        public static readonly string SelectAllId = "[ALL]";
        public string Id { get; set; }
        public string Name { get; set; }
        public string SortOrderId { get; set; }
		public virtual bool IsSelected { get; set; }
        public virtual void SetSelected(bool isSelected)
        {

        }

        public static ClaimValueRange FromXml(XElement element)
        {
            const string idElement = "Claim_Value_Idx";
            const string nameElement = "Claim_Value_Range_Name";
            const string sortOrderIdElement = "Claim_Sort_Order_Idx";
			const string isSelectedElement = "IsSelected";

            return new ClaimValueRange
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                SortOrderId = element.GetValue<string>(sortOrderIdElement),
				IsSelected = element.GetValue<int>(isSelectedElement) == 1
            };
        }
    }
}
