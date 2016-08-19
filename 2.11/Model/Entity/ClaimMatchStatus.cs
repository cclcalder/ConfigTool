using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ClaimMatchStatus
    {
        public static readonly string SelectAllId = "[ALL]";

        public string Id { get; set; }
        public string Name { get; set; }
        public virtual bool IsSelected { get; set; }

        public virtual void SetSelected(bool isSelected)
        {
           
        }

        public static ClaimMatchStatus FromXml(XElement element)
        {
            const string idElement = "Claim_Matching_Status_Idx";
            const string nameElement = "Claim_Matching_Status_Name";
            const string isSelectedElement = "IsSelected";

            return new ClaimMatchStatus
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                IsSelected = element.GetValue<int>(isSelectedElement) == 1
            };
        }
    }
}
