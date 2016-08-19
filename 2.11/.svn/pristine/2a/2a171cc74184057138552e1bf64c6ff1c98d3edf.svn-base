using System;
using System.Globalization;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.Generic
{
    public class DropdownItem 
    {
     
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public DateTime? DelistingsDate { get; set; }

        public static DropdownItem FromXml(XElement xml)
        {
            return new DropdownItem
                {
                    ID = xml.Element("Idx").MaybeValue(),
                    Name = xml.Element("Name").MaybeValue(),
                    IsSelected = xml.Element("IsSelected").MaybeValue() == "1",
                    DelistingsDate = GetDelisingsDate(xml)
                };
        }

        private static DateTime? GetDelisingsDate(XElement element)
        {
            var date = element.Element("Date_End").MaybeValue();

            DateTime? delistingsDate = null;

            if (date != null)
            {
                delistingsDate = DateTime.Parse(date);
            }

            return delistingsDate;
        }
    }

    public class Status
    {
        public string Name { get; set; }
        public string Colour { get; set; }
        public string SortOrder { get; set; }
        public string ID { get; set; }
        public bool IsSelected { get; set; }
        public bool IsEnabled { get; set; }

        public static Status FromXml(XElement xml)
        {
            if (xml == null) return null;
            return new Status
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Colour = xml.Element("Colour").MaybeValue() ?? "#ffffff",
                IsSelected = xml.Element("IsSelected").MaybeValue() == "1",
                IsEnabled = xml.Element("IsEnabled").MaybeValue() == "1",
                SortOrder = xml.Element("SortOrder").MaybeValue()
            };
        }
    }

    public class Note
    {
        public Note() { }
        public static Note FromXml(XElement el)
        { 
             if (el == null) return null;
            return new Note()
            {
                TimeStamp = el.GetValue<string>("TimeStamp").TryParseAs<DateTime>().Value.ToString("dd-MM-yyyy HH:mm"),
                UserName = el.GetValue<string>("UserName"),
                Value = el.GetValue<string>("Value"),
                Colour = el.GetValue<string>("Colour")
            };

        }

        #region TimeStamp
        /// <summary>
        /// Gets or sets the DateCreated of this PromotionComment.
        /// </summary>
        public string TimeStamp { get; set; }
        #endregion

        public string Colour { get; set; }

        #region UserName
        /// <summary>
        /// Gets or sets the UserName of this PromotionComment.
        /// </summary>
        public string UserName { get; set; }
        #endregion

        #region Value
        /// <summary>
        /// Gets or sets the Description of this PromotionComment.
        /// </summary>
        public string Value { get; set; }
        #endregion


        #region Header
        /// <summary>
        /// Gets or sets the Header of this PromotionComment.
        /// </summary>
        public string Header { get { return "[" + TimeStamp + " " + UserName + "]"; } }
        #endregion



    }

    public class Comment
    {
        public string ID { get; set; }
        public string Value { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        //public string CommentType { get; set; }
        //public bool CanDelete { get; set; }

        public static Comment FromXml(XElement xml)
        {
            if (xml == null) return null;

            return new Comment
            {
                ID = xml.Element("ID").MaybeValue(),
                Value = xml.Element("Value").MaybeValue(),
                UserName = xml.Element("UserName").MaybeValue(),
                TimeStamp = TryParseDate(xml.Element("TimeStamp").MaybeValue()),
                //CommentType = xml.Element("CommentType").MaybeValue(),
                //CanDelete = xml.Element("CanDelete") != null && xml.Element("CanDelete").MaybeValue() == "1",
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