using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.GroupingMenu;

namespace Model.Entity.Canvas
{
    public class Insight : MenuItem
    {
        private const string Parent_Idx = "Parent_Idx";

        private const string Item_Idx = "Item_Idx";
        private const string Item_Name = "Item_Name";

        private const string UrlElement = "URL";

        private const string XmlNumberOfHorizontalCells = "Number_Of_Horizontal_Cells";
        private const string XmlNumberOfVerticalCells = "Number_Of_Vertical_Cells";

        private const string Item_Code = "Item_Code";
        private const string XmlSortOrder = "SortOrder";

        private const string XmlHasFilters = "HasFilters";

        public Insight()
        {

        }
        public Insight(XElement element)
        {
            #region example xml that is coming in
            //   <Item>
            //     <Item_Idx>1</Item_Idx>
            //     <Item_Code>AP01</Item_Code>
            //     <Item_Name>(AP01) Account Plan Product Drill</Item_Name>
            //     <Number_Of_Horizontal_Cells>10</Number_Of_Horizontal_Cells>
            //     <Number_Of_Vertical_Cells>10</Number_Of_Vertical_Cells>
            //     <SortOrder>1</SortOrder>
            //     <Parent_Idx>REPORT_GROUP$3</Parent_Idx>
            //     <HasFilters>1</HasFilters>
            //   </Item>
            #endregion

            ParentId = element.Element(Parent_Idx).MaybeValue() ?? element.Element("ParentIdx").MaybeValue();

            Id = element.Element(Item_Idx).MaybeValue() ?? element.Element("Idx").MaybeValue();
            Header = element.GetValue<string>(Item_Name);

            Code = element.GetValue<string>(Item_Code);
            SortOrder = element.GetValue<int>(XmlSortOrder);

            HorizontalCells = element.GetValue<int>(XmlNumberOfHorizontalCells);
            VerticalCells = element.GetValue<int>(XmlNumberOfVerticalCells);

            HasFilters = element.GetValue<int>(XmlHasFilters) == 1;
            
            Url = element.Element(UrlElement).MaybeValue();
        }

        public string Code { get; set; }
        public int HorizontalCells { get; set; }
        public int VerticalCells { get; set; }
        public bool HasFilters { get; set; }
    }
}
