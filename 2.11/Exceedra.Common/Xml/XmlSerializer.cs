using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Exceedra.Common.Xml
{
    public static class XmlSerializer
    {
        //public static XDocument ToCoreXml(RowRecord)
        //{
        //    XDocument xdoc = new XDocument(
        //        from r in Records
        //        select new XElement("RootItem",
        //            new XElement("Item_Type", r.Item_Type),
        //            new XElement("Item_Idx", r.Item_Idx),
        //            new XElement("Attributes",
        //                from p in r.Properties
        //                select new XElement("Attribute",
        //                    new XElement("ColumnCode", p.ColumnCode),
        //                    p.GetCorrectFormatDateValue()
        //                    )
        //                    )
        //                    )
        //                    );


        //    return xdoc;
        //}

        //public static XDocument ToAttributeXml()
        //{
        //    var xdoc = new XDocument(
        //        new XElement("Attributes",
        //            from rt in Records
        //            select new XElement("RootItem",
        //                new XElement("Item_Type", rt.Item_Type),
        //                new XElement("Item_Idx", rt.Item_Idx),
        //                new XElement("Attributes",
        //                    from pp in rt.Properties
        //                    select new XElement("Attribute",
        //                        new XElement("ColumnCode", pp.ColumnCode),
        //                        pp.GetXmlSelectedItems()
        //                        )
        //                        )
        //                        )
        //                        )
        //                        );
        //    return xdoc;
        //}
    }
}
