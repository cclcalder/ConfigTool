using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.Entity.ROBs;


namespace Model.Entity.Funds
{
    public class Fund : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeID { get; set; }
        public string SubTypeID { get; set; }
        public string CustomerLevelID { get; set; }
        public CustomerLevel CustomerLevel { get; set; }
        public string ProductLevelID { get; set; }
        public ProductLevel ProductLevel { get; set; }
        public List<string> CustomerIDs { get; set; }
        public List<RobCustomer> Customers { get; set; }
        public List<string> ProductIDs { get; set; }
        public List<RobProduct> Products { get; set; }
        public Entity.ROBs.Status Status { get; set; }
        public ICollection<RobAttribute> Attributes { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public IList<Option> ImpactOptions { get; set; }
        public List<Scenario> Scenarios { get; set; }
        public List<string> ScenarioIDs { get; set; }
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
        public string Recipient { get; set; }

        public static Fund FromGetFundsXml(XElement xml)
        {
            return new Fund
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Code = xml.Element("Code").MaybeValue(),
                ItemType = xml.Element("ItemType").MaybeValue(),
                Start = TryGetDate(xml, "Start Date"),
                End = TryGetDate(xml, "End Date"),
                CustomerNames = xml.Element("Customer").MaybeElement("Name").MaybeValue(),
                Status = Entity.ROBs.Status.FromXml(xml.Element("Status")),
                Attributes = xml.Element("Attributes")
                    .MaybeElements("Attribute")
                    .Select(RobAttribute.FromXml)
                    .ToList(),
                Recipient = xml.Element("RecipientID").MaybeValue(),
                IsSelected = xml.GetValue<int>("IsSelected") == 1 ? true : false
            };
        }

        public static Fund FromGetFundXml(XElement xml)
        {
            var Fund = new Fund
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Code = xml.Element("Code").MaybeValue(),
                ProductLevelID = xml.Element("ProdLevel").MaybeElement("ID").MaybeValue(),
                ProductIDs = xml.Element("Products").MaybeElements().Select(x => x.MaybeElement("ID").MaybeValue()).ToList(),
                CustomerLevelID = xml.Element("CustLevel").MaybeElement("ID").MaybeValue(),
                CustomerIDs = xml.Element("Customers").MaybeElements().Select(x => x.MaybeElement("ID").MaybeValue()).ToList(),
                ItemTypeID = xml.Element("Type").MaybeValue(),
                SubTypeID = xml.Element("SubType").MaybeValue(),
                Start = TryParseDate(xml.Element("Dates").MaybeElement("Start").MaybeValue()),
                End = TryParseDate(xml.Element("Dates").MaybeElement("End").MaybeValue()),
                StatusID = xml.Element("StatusID").MaybeValue(),
                ScenarioIDs = xml.Element("Scenarios").MaybeElements().Select(x => x.MaybeElement("ID").MaybeValue()).ToList(),
                IsEditable = xml.Element("IsEditable").MaybeValue() == "1",
                ImpactOptions =
                    xml.Element("ImpactOptions").MaybeElements("Option")
                    .Select(e => new Option { ControlID = e.MaybeElement("ControlID").MaybeValue(), ID = e.MaybeElement("ID").MaybeValue(), Value = TryParseDecimal(e.MaybeElement("Value").MaybeValue()) })
                        .ToList(),
                Recipient = xml.Element("RecipientID").MaybeValue()
            };
            return Fund;
        }

        private static decimal TryParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            decimal amount;
            decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
            return amount;
        }

        public bool IsEditable { get; set; }

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

        public static Fund FromGetPromoDataXml(XElement el)
        {
            var r = new Fund
            {
                
                ID = el.GetValue<string>("ID"),
                Name = el.GetValue<string>("Name"),
                CustomerNames = el.Element("Customer").GetValue<string>("Name"),
                Status = Entity.ROBs.Status.FromXml(el.Element("Status")),
                IsSelected = el.GetValue<int>("IsSelected") == 1 ? true : false

            };
            var ed = new List<PromotionExtraData>();
            if (el.Element("Attributes") != null)
            {
                ed = el.Element("Attributes").Elements().Select(a => new PromotionExtraData(a)).ToList();
            }

            if (ed.Count() > 0)
            {
                DateTime d;
                var s = ed.Where(t => t.Name == "Buy-in start").SingleOrDefault().Value;
                var xx = DateTime.TryParse(s, out d);
                r.Start = d;

                DateTime d2;
                var s2 = ed.Where(t => t.Name == "Buy-in end").SingleOrDefault().Value;
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