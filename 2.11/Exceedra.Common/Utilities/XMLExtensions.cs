using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Exceedra.Common
{
    public static class XMLExtension
    {
        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        /// <returns>Returns true if the value of xml is "1" or "true" (case insensitive).
        /// Returns false if xml or its value is null or the value is different than "1" and "true" (case insensitive)</returns>
        public static bool IsTrue(this XElement xml)
        {
            if (xml == null || string.IsNullOrEmpty(xml.Value)) return false;

            var xmlValueToLower = xml.Value.ToLower();
            if (xmlValueToLower == "true" || xmlValueToLower == "1") return true;

            return false;
        }
    }

    public class XmlComparer
    {
        private static class Xsi
        {
            public static XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

            public static XName schemaLocation = xsi + "schemaLocation";
            public static XName noNamespaceSchemaLocation = xsi + "noNamespaceSchemaLocation";
        }

        public XDocument Normalize(XDocument source, XmlSchemaSet schema = null)
        {
            bool havePSVI = false;
            // validate, throw errors, add PSVI information
            if (schema != null)
            {
                source.Validate(schema, null, true);
                havePSVI = true;
            }
            return new XDocument(
                source.Declaration,
                source.Nodes().Select(n =>
                {
                    // Remove comments, processing instructions, and text nodes that are
                    // children of XDocument.  Only white space text nodes are allowed as
                    // children of a document, so we can remove all text nodes.
                    if (n is XComment || n is XProcessingInstruction || n is XText)
                        return null;
                    XElement e = n as XElement;
                    if (e != null)
                        return NormalizeElement(e, havePSVI);
                    return n;
                }
                    )
                );
        }

        public bool DeepEquals(XElement x1, XElement x2, XmlSchemaSet schemaSet = null)
        {
            XDocument xDoc1 = new XDocument(x1);
            XDocument xDoc2 = new XDocument(x2);

            return DeepEquals(xDoc1, xDoc2, schemaSet);
        }

        public bool DeepEquals(XDocument doc1, XDocument doc2,
            XmlSchemaSet schemaSet = null)
        {
            XDocument d1 = Normalize(doc1, schemaSet);
            XDocument d2 = Normalize(doc2, schemaSet);
            return XNode.DeepEquals(d1, d2);
        }

        private static IEnumerable<XAttribute> NormalizeAttributes(XElement element,
            bool havePSVI)
        {
            return element.Attributes()
                .Where(a => !a.IsNamespaceDeclaration &&
                            a.Name != Xsi.schemaLocation &&
                            a.Name != Xsi.noNamespaceSchemaLocation)
                .OrderBy(a => a.Name.NamespaceName)
                .ThenBy(a => a.Name.LocalName)
                .Select(
                    a =>
                    {
                        if (havePSVI)
                        {
                            var dt = a.GetSchemaInfo().SchemaType.TypeCode;
                            switch (dt)
                            {
                                case XmlTypeCode.Boolean:
                                    return new XAttribute(a.Name, (bool)a);
                                case XmlTypeCode.DateTime:
                                    return new XAttribute(a.Name, (DateTime)a);
                                case XmlTypeCode.Decimal:
                                    return new XAttribute(a.Name, (decimal)a);
                                case XmlTypeCode.Double:
                                    return new XAttribute(a.Name, (double)a);
                                case XmlTypeCode.Float:
                                    return new XAttribute(a.Name, (float)a);
                                case XmlTypeCode.HexBinary:
                                case XmlTypeCode.Language:
                                    return new XAttribute(a.Name,
                                        ((string)a).ToLower());
                            }
                        }
                        return a;
                    }
                );
        }

        private static XNode NormalizeNode(XNode node, bool havePSVI)
        {
            // trim comments and processing instructions from normalized tree
            if (node is XComment || node is XProcessingInstruction)
                return null;
            XElement e = node as XElement;
            if (e != null)
                return NormalizeElement(e, havePSVI);
            // Only thing left is XCData and XText, so clone them
            return node;
        }

        private static XElement NormalizeElement(XElement element, bool havePSVI)
        {
            if (havePSVI)
            {
                var dt = element.GetSchemaInfo();
                switch (dt.SchemaType.TypeCode)
                {
                    case XmlTypeCode.Boolean:
                        return new XElement(element.Name,
                            NormalizeAttributes(element, havePSVI),
                            (bool)element);
                    case XmlTypeCode.DateTime:
                        return new XElement(element.Name,
                            NormalizeAttributes(element, havePSVI),
                            (DateTime)element);
                    case XmlTypeCode.Decimal:
                        return new XElement(element.Name,
                            NormalizeAttributes(element, havePSVI),
                            (decimal)element);
                    case XmlTypeCode.Double:
                        return new XElement(element.Name,
                            NormalizeAttributes(element, havePSVI),
                            (double)element);
                    case XmlTypeCode.Float:
                        return new XElement(element.Name,
                            NormalizeAttributes(element, havePSVI),
                            (float)element);
                    case XmlTypeCode.HexBinary:
                    case XmlTypeCode.Language:
                        return new XElement(element.Name,
                            NormalizeAttributes(element, havePSVI),
                            ((string)element).ToLower());
                    default:
                        return new XElement(element.Name,
                            NormalizeAttributes(element, havePSVI),
                            element.Nodes().Select(n => NormalizeNode(n, havePSVI))
                            );
                }
            }
            else
            {
                return new XElement(element.Name,
                    NormalizeAttributes(element, havePSVI),
                    element.Nodes().Select(n => NormalizeNode(n, havePSVI))
                    );
            }
        }
    }
}
