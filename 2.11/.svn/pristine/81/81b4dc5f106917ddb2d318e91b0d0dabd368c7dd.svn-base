using System.Collections.Generic;
using System.Linq;
 

namespace Exceedra.Common
{
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    public static class XElementEx
    {
        public static XElement MaybeElement(this XElement element, XName name)
        {
            if (element == null) return null;
            return element.Element(name);
        }

        public static IEnumerable<XElement> MaybeElements(this XElement element)
        {
            if (element == null) return Enumerable.Empty<XElement>();
            return element.Elements();
        }
    
        public static IEnumerable<XElement> MaybeElements(this XElement element, XName name)
        {
            if (element == null) return Enumerable.Empty<XElement>();
            return element.Elements(name);
        }

        public static string MaybeValue(this XElement element)
        {
            return element == null ? null : element.Value;
        }

        public static string MaybeValue(this XElement element, string defaultValue)
        {
            return element == null || element.Value == string.Empty
                ? defaultValue
                : element.Value;
        }

        public static string MaybeValue(this XAttribute attribute)
        {
            return attribute == null ? null : attribute.Value;
        }

        public static string MaybeValue(this XAttribute attribute, string defaultValue)
        {
            return attribute == null || attribute.Value == string.Empty 
                ? defaultValue 
                : attribute.Value;
        }
    }

    public static class XContainerEx
    {
        public static XElement AddElement(this XContainer element, XName name)
        {
            var newElement = new XElement(name);
            element.Add(newElement);
            return newElement;
        }
        
        public static XElement AddElement(this XContainer element, XName name, object content)
        {
            var newElement = new XElement(name, content);
            element.Add(newElement);
            return newElement;
        }
    }
}
