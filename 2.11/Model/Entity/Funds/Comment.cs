using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity.Funds
{
    public class Comment
    {
        public string ID { get; set; }
        public string Value { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        public string CommentType { get; set; }
        public bool CanDelete { get; set; }

        public static Comment FromXml(XElement xml)
        {
            if (xml == null) return null;

            return new Comment
                       {
                           ID = xml.Element("ID").MaybeValue(),
                           Value = xml.Element("Value").MaybeValue(),
                           UserName = xml.Element("UserName").MaybeValue(),
                           TimeStamp = TryParseDate(xml.Element("TimeStamp").MaybeValue()),
                           CommentType = xml.Element("CommentType").MaybeValue(),
                           CanDelete = xml.Element("CanDelete") != null && xml.Element("CanDelete").MaybeValue() == "1",
                       };
        }

        private static DateTime TryParseDate(string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return default(DateTime);
            DateTime date;
            return DateTime.TryParse(source, CultureInfo.CurrentCulture, DateTimeStyles.None, out date) ? date : default(DateTime);
        }
    }
}
