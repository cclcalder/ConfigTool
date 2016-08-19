using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.Annotations;

namespace Model.Entity.Generic
{
    public class ComboboxItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Idx { get; set; }

        public int SortOrder { get; set; }

        private string _colour;
        public string Colour { get { return _colour ?? "#00ffffff"; } set { _colour = value; } }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (IsSelected == value) return;

                _isSelected = value;
                PropertyChanged.Raise(this, "IsSelected");
            }
        }
         
        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                PropertyChanged.Raise(this, "IsEnabled");
            }
        }
         
        public DateTime? DelistingsDate { get; set; }

        public ComboboxItem()
        {
            Name = "Name not set";
            Idx = "0";
            IsSelected = false;
            IsEnabled = false;
        }

        public ComboboxItem(ComboboxItem item)
        {
            Name = item.Name;
            Idx = item.Idx;
            Colour = item.Colour;
            IsSelected = item.IsSelected;
            IsEnabled = item.IsEnabled;
            DelistingsDate = item.DelistingsDate;
        }

        public ComboboxItem(XElement xml)
        {
            Name = xml.Element("Name").MaybeValue();
            Idx = xml.Element("Idx").MaybeValue() ?? xml.Element("ID").MaybeValue() ?? xml.Element("ConditionReason").MaybeValue() ?? xml.Element("ConditionTypeId").MaybeValue() ?? xml.Element("ScenarioId").MaybeValue();
            IsSelected = (xml.Element("IsSelected").MaybeValue() ?? xml.Attribute("IsSelected").MaybeValue()) == "1" ;

            if (xml.Element("IsDefault") != null)
            {
               IsSelected = xml.Element("IsDefault").MaybeValue() == "1";
            }

            IsEnabled = (xml.Element("IsEnabled") != null ? xml.Element("IsEnabled").MaybeValue() : "1") == "1";
            Colour = xml.Element("Colour").MaybeValue();
            DelistingsDate = GetDelisingsDate(xml);

            Name = Name ?? xml.Elements().FirstOrDefault(x => x.Name.ToString().ToLower().Contains("name")).MaybeValue() ?? xml.Attribute("Name").MaybeValue();  
            
            Idx = Idx ?? xml.Elements().FirstOrDefault(x => x.Name.ToString().ToLower().Contains("idx")).MaybeValue() ?? xml.Attribute("Idx").MaybeValue();

            SortOrder = xml.GetValue<int>("SortOrder");
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}