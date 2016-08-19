using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq; 
using Exceedra.DynamicGrid.Models;
using Exceedra.MultiSelectCombo.ViewModel;

namespace Exceedra.Controls.DynamicRow.Models
{
    public class RowProperty : PropertyBase, INotifyPropertyChanged
    {

        public string ParentIDx { get; set; }

        private int? _maxWidth;
        public int? MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                _maxWidth = value;
                OnPropertyChanged("MaxWidth");
            }
        }

        private ObservableCollection<Option> _values;
        public override ObservableCollection<Option> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
                // SelectedItems = new ObservableCollection<Option>();
                var newlist = _values.Where(r => r.IsSelected == true).ToList();

                switch (ControlType.ToLowerInvariant())
                {
                    case "dropdown":
                        SelectedItem = newlist.FirstOrDefault();
                        break;
                    case "multiselectdropdown":
                        SelectedItems = newlist.Any() ? new ObservableCollection<Option>(newlist) : new ObservableCollection<Option>();
                        Items = new MultiSelectViewModel(Values);
                        break;
                }

                OnPropertyChanged("Values");
            }
        }

        private MultiSelectViewModel _items;
        public MultiSelectViewModel Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        } 

        public string SelectedItemsText
        {
            get
            {
                if (Values == null || !Values.Any(v => v.IsSelected)) 
                    return "";

                var items = "";
                Values.Where(v => v.IsSelected).Do(v => items += v.Item_Name + ", ");
                return items.Substring(0, items.Length - 2);
            }
        }

        public override bool HasValue()
        {
            var values = GetXmlSelectedItems();
            if (
                (values == null || !values.Elements("Value").Any() || values.Elements("Value").Any(valueElement => string.IsNullOrEmpty(valueElement.Value)))
                && string.IsNullOrEmpty(Value))
                return false;

            return true;
        }


        public string _innerValue;

        /// <summary>
        /// Gets or sets the Value of this PromotionExtraData.
        /// Values can be either actual or calculated
        /// values that start with '=' are calculated
        /// values are formatted using the StringFormat value
        /// </summary>
        public override string Value
        {
            get { return _innerValue; }
            set
            {
                _innerValue = FormatValue(value);
                OnPropertyChanged("Value");
            }
        }

        public bool IsReadOnly { get { return !IsEditable; } }

        public bool TemplateEditableCheckbox { get; set; }

        public int TemplateEditableCheckboxINT
        {
            get { return TemplateEditableCheckbox ? 1 : 0; }
        }

        public bool ShowCheckBoxColumn { get; set; }


        private List<string> _dependentColumns;

        public List<string> DependentColumns
        {
            get { return _dependentColumns ?? (_dependentColumns = new List<string>()); }
            set { _dependentColumns = value; }
        }

        private ObservableCollection<Option> _selectedItems;
        public override ObservableCollection<Option> SelectedItems
        {
            get
            {
                // we have to be sure that the SelectedItems collection will be initialized (won't be null)
                // beucase if it's null some events are not being called 
                // (like the OnSelectedItemsChanged in the ComboCheckBoxesWithSelectAll)
                // which causes defects (like breaking dependencies between dropdowns)
                if (_selectedItems == null) _selectedItems = new ObservableCollection<Option>();
                if (ControlType.ToLowerInvariant() == "multiselectdropdown" && Items != null)
                {
                    var selectedItems = Items.Items.Where(i => i.IsSelected).Select(i => i.Idx);
                    _selectedItems = new ObservableCollection<Option>(Values.Where(v => selectedItems.Contains(v.Item_Idx)));
                }
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                OnPropertyChanged("SelectedItems");
                OnPropertyChanged("SelectedItemsText");
            }
        }

        private Option _selectedItem;
        public override Option SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                Value = (value == true ? "1" : "0");
                OnPropertyChanged("IsChecked");
            }
        }

        public XElement GetCorrectlyFormatedValue()
        {
            var p = this;
            var v = p.Value;

            //if (!string.IsNullOrEmpty(p.Value))
            {
                if (p.ControlType.ToLower().Contains("drop") == true && p.SelectedItem != null &&
                    p.SelectedItem.Item_Idx != null)
                {
                    v = p.SelectedItem.Item_Idx;
                }
                else if (p.StringFormat != null && p.StringFormat.ToLower().Contains("c") && !string.IsNullOrEmpty(v))
                {
                    v = double.Parse(p.Value, NumberStyles.Currency).ToString(CultureInfo.InvariantCulture);
                }

                else if ((p.ControlType.ToLower() == "datepicker" || (p.StringFormat != null && p.StringFormat.ToLower() == "shortdate")) && v != "")
                {
                    //change to yyyy-mm-ddd
                    DateTime dt;

                    dt = Convert.ToDateTime(v);

                    v = dt.ToString("yyyy-MM-dd");


                }
                else if (p.StringFormat != null && p.StringFormat.ToLower().Contains("p"))
                {
                    v = p.Value.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    v = FixValue(v);
                }
                else
                {
                    v = FixValue(v);
                }
            }



            return new XElement("Value", v);
        }

        public XElement GetXmlSelectedItems()
        {
            var isComboMultiple = (ControlType.ToLower().Contains("multi"));
            var isComboSingle = (ControlType.ToLower() == "dropdown");
            var isDate = (ControlType.ToLowerInvariant() == "datepicker");
            var hasItem = (SelectedItem != null);
            var hasValue = !string.IsNullOrEmpty(Value);

            var valuesNode = new XElement("Values");
            if (isComboMultiple)
            {
                if (SelectedItems != null)
                    foreach (var p in SelectedItems)
                    {
                        valuesNode.Add(new XElement("Value", FixValue(p.Item_Idx)));
                    }
            }
            else if (isComboSingle)
            {
                if (SelectedItem != null) valuesNode.Add(new XElement("Value", FixValue(SelectedItem.Item_Idx)));
            }
            else if (isDate)
            {
                DateTime thisDate;
                valuesNode.Add(new XElement("Value", FixValue(DateTime.TryParse(Value, out thisDate) ? thisDate.ToString("yyyy-MM-dd") : Value)));
            }
            else if (hasValue)
            {
                // all single values are passed as items in array of values
                valuesNode.Add(new XElement("Value", FixValue(Value)));
            }
            else if (hasItem)
            {
                // selected item are saved as array of value 
                valuesNode.Add(new XElement("Value", FixValue(SelectedItem.Item_Idx)));
            }

            return valuesNode;
        }

        private string FixValue(string val)
        {
            if (string.IsNullOrEmpty(val)) return string.Empty;

            var isNum = false;
            decimal d;
            //rip out the localised %
            val = val.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");

            if (CultureInfo.CurrentCulture.IetfLanguageTag != ("en-GB"))
            {
                isNum = decimal.TryParse(val, NumberStyles.Number, CultureInfo.CurrentCulture, out d);
            }
            else
            {
                isNum = decimal.TryParse(val, NumberStyles.Any, CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"), out d);
            }

            if (isNum == true)
            {
                var x = d.ToString(CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"));
                return x;
            }
            else
            {
                return val;
            }
        }
    }
}





