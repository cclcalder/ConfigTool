using System;
using System.Xml.Linq;
using Exceedra.Common;
using Telerik.Windows.Controls;

namespace Model.Entity.Generic

{
    
    public class ExceedraRadComboBoxItem : RadComboBoxItem
    {
        public DateTime? DelistingsDate { get; set; }
        public string Idx { get; set; }
        public string Colour { get; set; }
        public string Name { get; set; }

        public ExceedraRadComboBoxItem(XElement xml)
        {
            Content = Name = xml.Element("Name").MaybeValue();
            Idx = xml.Element("Idx").MaybeValue() ?? xml.Element("ID").MaybeValue();
            IsSelected = xml.Element("IsSelected").MaybeValue() == "1";
            IsEnabled = (xml.Element("IsEnabled").MaybeValue() ?? "1") == "1";
            Colour = xml.Element("Colour").MaybeValue();
            DelistingsDate = GetDelisingsDate(xml);
        }

        public ExceedraRadComboBoxItem()
        {
        }

        private DateTime? GetDelisingsDate(XElement element)
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
}