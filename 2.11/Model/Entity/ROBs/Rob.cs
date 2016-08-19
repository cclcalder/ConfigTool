using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.Entity.Generic;

namespace Model.Entity.ROBs
{
    public class Rob : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeID { get; set; }
        public string SubTypeID { get; set; }
        public string CustomerLevelIdx { get; set; }
        public string ProductLevelIdx { get; set; }
        public List<string> CustomerIDs { get; set; }
        public List<ComboboxItem> Customers { get; set; }
        public List<string> ProductIDs { get; set; }
        public List<string> SelectedProductIdxs { get; set; }
        public string StatusIdx { get; set; }
        public string StatusName { get; set; }
        public SolidColorBrush StatusColour { get; set; }
        public ICollection<RobAttribute> Attributes { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public IList<Option> ImpactOptions { get; set; }
        public List<string> SelectedScenarioIdxs { get; set; }
        public string FileLocation { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                PropertyChanged.Raise(this, "IsSelected");
            }
        }
        public string CustomerNames { get; private set; }
        public List<string> Recipients { get; set; }
        public string Recipient;

        public static Rob FromGetRobsXml(XElement xml)
        {
            return new Rob
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Code = xml.Element("Code").MaybeValue(),
                ItemType = xml.Element("ItemType").MaybeValue(),
                Start = TryGetDate(xml, "Start Date"),
                End = TryGetDate(xml, "End Date"),
                CustomerNames = xml.Element("Customer").MaybeElement("Name").MaybeValue(),
                StatusName = xml.Element("Status").MaybeElement("Name").MaybeValue(),
                StatusColour = (SolidColorBrush)new BrushConverter().ConvertFrom(xml.Element("Status").MaybeElement("Color").MaybeValue()),
                Attributes = xml.Element("Attributes")
                    .MaybeElements("Attribute")
                    .Select(RobAttribute.FromXml)
                    .ToList(),
                Recipient = xml.Element("RecipientID").MaybeValue(),
                IsSelected = xml.GetValue<int>("IsSelected") == 1 ? true : false,
                ShowInfoGrid = xml.GetValue<int>("ShowInfoGrid") == 1 ? true : false,
                FileLocation = xml.Element("File_Location").MaybeValue()
            };
        }

        public static Rob FromGetRobXml(XElement xml)
        {
            var rob = new Rob
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
               Code = xml.Element("Code").MaybeValue(),
                IsEditable = xml.Element("IsEditable").MaybeValue() == "1",
                ShowInfoGrid = xml.GetValue<int>("ShowInfoGrid") == 1 ? true : false,
                Start = DateTime.Parse(xml.Element("Dates").Element("Start").Value),
                End = DateTime.Parse(xml.Element("Dates").Element("End").Value),
                FileLocation = xml.Element("File_Location").MaybeValue()
            };
            return rob;
        }

        private static decimal TryParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            decimal amount;
            decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
            return amount;
        }

        public bool IsEditable { get; set; }
        public bool ShowInfoGrid { get; set; }
        public string StatusID { get; set; }

        private static DateTime TryGetDate(XElement xml, string name)
        {
            var attribute =
                xml.Element("Attributes")
                    .MaybeElements("Attribute")
                    .FirstOrDefault(x => x.MaybeElement("Name").MaybeValue() == name)
                    .MaybeElement("Value")
                    .MaybeValue();
            return TryParseDate(attribute);
        }

        private static DateTime TryParseDate(string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return default(DateTime);
            DateTime date;
            return DateTime.TryParse(source, CultureInfo.CurrentCulture, DateTimeStyles.None, out date) ? date : default(DateTime);
        }

        public static Rob FromGetPromoDataXml(XElement el)
        {
            var r = new Rob
            { 
                ID = el.GetValue<string>("ID"),
                Name = el.GetValue<string>("Name"),
                CustomerNames = el.Element("Customer").GetValue<string>("Name"),
                ItemType = el.Element("ItemType").MaybeValue(),
                IsSelected = el.GetValue<int>("IsSelected") == 1 ? true : false,
                StatusName = el.Element("Status").MaybeElement("Name").MaybeValue(),
                StatusColour = (SolidColorBrush)new BrushConverter().ConvertFrom(el.Element("Status").MaybeElement("Color").MaybeValue())

            };
            var ed = new List<PromotionExtraData>();
            if (el.Element("Attributes") != null)
            {
                ed = el.Element("Attributes").Elements().Select(a => new PromotionExtraData(a)).ToList();
            }

            if (ed.Any())
            {
                DateTime d;
                var s = ed.SingleOrDefault(t => t.Name.ToLower().Contains("start")).Value;
                var xx = DateTime.TryParse(s, out d);
                r.Start = d;

                DateTime d2;
                var s2 = ed.SingleOrDefault(t => t.Name.ToLower().Contains("end")).Value;
                var xx2 = DateTime.TryParse(s2, out d2);
                r.End = d2;
            }

            if (el.Element("ItemType") != null)
            {
                r.SubTypeID = el.Element("ItemType").Value;
            }
             
            return r;

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}