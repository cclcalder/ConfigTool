using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity.Admin
{
    public class Pattern1Grid
    {
        private string m_itemID = "GridItem_Idx";
        public string ItemID { get; set; }

        private string m_itemCode = "GridItem_Code";
        public string ItemCode { get; set; }

        private string m_itemName = "GridItem_Name";
        public string ItemName { get; set; }

        private string m_sortOrder = "GridItem_SortOrder";
        public string SortOrder { get; set; }

        private string m_foreColour = "GridItem_ForeColour";
        public string ForeColour { get; set; }

        private string m_borderColour = "GridItem_BorderColour";
        public string BorderColour { get; set; }

        private string m_isEditableString = "GridItem_IsEditable";
        public string IsEditableString { get; set; }
        public bool IsEditable { get; set; }

        private string m_control = "GridItem_ControlType";
        public string Control { get; set; }

        private string m_dataSource = "GridItem_DataSource";
        public string DataSource { get; set; }

        public Pattern1Grid(XElement element)
        {
            ItemID = element.GetValue<string>(m_itemID);
            ItemCode = element.GetValue<string>(m_itemCode);
            ItemName = element.GetValue<string>(m_itemName);
            SortOrder = element.GetValue<string>(m_sortOrder);
            ForeColour = element.GetValue<string>(m_foreColour);
            BorderColour = element.GetValue<string>(m_borderColour);

            IsEditableString = element.GetValue<string>(m_isEditableString);
            if (IsEditableString != null)
            {
                if(IsEditableString == "1")
                {
                    IsEditable = true;
                }
                else
                {
                    IsEditable = false;
                }
            }

            Control = element.GetValue<string>(m_control);
            DataSource = element.GetValue<string>(m_dataSource);

        }

    }
}
