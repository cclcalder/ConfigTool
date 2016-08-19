using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity.Admin
{
    public class Pattern1ValidationCode
    {
        private string m_itemCode = "Item_Code";
        public string ItemCode { get; set; }

        public Pattern1ValidationCode(XElement element)
        {
            ItemCode = element.GetValue<string>(m_itemCode);
        }

    }
}
