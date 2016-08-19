using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Controls.ViewModels;
using Exceedra.DynamicGrid.Models;
using Model.Entity.Generic;

namespace Exceedra.MultiSelectCombo.ViewModel
{
    public class MultiSelectViewModel : Base
    {
        public MultiSelectViewModel()
        {
            
        }

        public MultiSelectViewModel(XElement xml)
        {
            Items = new List<ComboboxItem>();

            var items = xml.Elements().Select(e => new ComboboxItem(e)).ToList();

            Items.Add(GetAllNode(items.All(v => v.IsSelected)));
            items.Do(i => i.PropertyChanged += ItemOnPropertyChanged);
            Items.AddRange(items);
            SelectedItems = Items.Skip(1).Where(i => i.IsSelected).ToList();

        }

        public MultiSelectViewModel(IEnumerable<ComboboxItem> items)
        {
            SetItems(items);
        }

        public MultiSelectViewModel(ObservableCollection<Option> values)
        {
            Items = new List<ComboboxItem>();

            Items.Add(GetAllNode(values.All(v => v.IsSelected)));

            foreach (var v in values)
            {
                var item = new ComboboxItem
                {
                    Idx = v.Item_Idx,
                    Name = v.Item_Name,
                    IsSelected = v.IsSelected,
                    IsEnabled = IsEnabled != false
                };
                item.PropertyChanged += ItemOnPropertyChanged;
                Items.Add(item);
            }

            SelectedItems = Items.Where(t=>t.Idx != "-1" && t.IsSelected).ToList();

        }

        private bool _selectingAll;
        private bool _settingSelectAll;

        private bool _selectionsAreRunning;

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "IsEnabled") return;

            _selectionsAreRunning = true;

            var item = (ComboboxItem) sender;
            if (item.Name == "[Select All]" && !_settingSelectAll)
            {
                _selectingAll = true;
                Items.Skip(1).Do(i => i.IsSelected = item.IsSelected);
                _selectingAll = false;
            }
            else if (!_selectingAll)
            {
                if (Items.Skip(1).All(i => i.IsSelected))
                {
                    _settingSelectAll = true;
                    if (!Items[0].IsSelected) Items[0].IsSelected = true;
                    _settingSelectAll = false;
                }
                else if (Items.Skip(1).Any(i => !i.IsSelected))
                {
                    _settingSelectAll = true;
                    if (Items[0].IsSelected) Items[0].IsSelected = false;
                    _settingSelectAll = false;
                }
            }

            //if true, then it's actually false but now we can raise the selectedItems as changing
            //We need this to prevent multiple SelectedItems notifications firing.
            if (_selectionsAreRunning && !_selectingAll && !_settingSelectAll)
            {
                _selectionsAreRunning = false;
                SelectedItems = Items.Skip(1).Where(i => i.IsSelected).ToList();
            }
        }

        private ComboboxItem GetAllNode(bool isSelected)
        {
            var allNode = new ComboboxItem
            {
                Idx = "-1",
                Name = "[Select All]",
                IsSelected = isSelected,
                IsEnabled = true
            };
            allNode.PropertyChanged += ItemOnPropertyChanged;
            return allNode;
        }

        private List<ComboboxItem> _items = new List<ComboboxItem>();
        public List<ComboboxItem> Items
        {
            get { return _items ?? (_items = new List<ComboboxItem>()); }
            set
            {
                _items = value;
                NotifyPropertyChanged(this, vm => vm.Items);
            }
        }

        public void SetItems(IEnumerable<ComboboxItem> items)
        {
            var enumeration = items.ToList();
            var tempList = new List<ComboboxItem>{GetAllNode(enumeration.All(v => v.IsSelected))};

            enumeration.Do(i => i.PropertyChanged += ItemOnPropertyChanged);
            tempList.AddRange(enumeration);

            tempList.FirstOrDefault(a => a.Idx == "-1").IsEnabled = tempList.All(a => a.IsEnabled);

            Items = new List<ComboboxItem>(tempList);
            
            SelectedItems = Items.Skip(1).Where(i => i.IsSelected).ToList();
        }

        public void Clear()
        {
            Items.Clear();
            SelectedItems = null;
        }

        private List<ComboboxItem> _selectedItems;
        public List<ComboboxItem> SelectedItems
        {
            get
            {
                if(_selectedItems == null) _selectedItems = new List<ComboboxItem>();
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                NotifyPropertyChanged(this, vm => vm.SelectedItems);
                NotifyPropertyChanged(this, vm => vm.SelectedItemsText);
            }
        }

        public List<string> SelectedItemIdxs
        {
            get { return SelectedItems.Select(i => i.Idx).ToList(); }
        }

        public string SelectedItemsText
        {
            get
            {
                if (Items == null || !SelectedItems.Any())
                    return "";

                var items = "";
                SelectedItems.Do(v => items += v.Name + ", ");

                // EW: Undid JS change below. Could not replicate the original issue and this fixed another issue. Spoke to JS.
                // JS: I've changed it because selected items were shown in two lines when a lot of selections were made. Setting the TextWrapping property to NoWrap had no effect.
                return items.Substring(0, items.Length - 2);
                //return items.Substring(0, items.Length > 113 ? 113 : items.Length - 2) + "...";
            }
        }

        private string _filterText;
        public string FilterText
        {
            get
            {
                if (_filterText == null)
                    _filterText = string.Empty;

                return _filterText;
            }
            set
            {
                _filterText = value;
                NotifyPropertyChanged(this, vm => vm.FilterText);
            }
        }

        private bool? _isEnabled;
        public bool? IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged(this, vm => vm.IsEnabled);
            }
        }

        public void DeSelect()
        {          
            SelectedItems = null;
            Items.Do(t => t.IsSelected = false);
        }

    }
}
