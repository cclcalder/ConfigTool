using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.ViewModels;
using Model;
using Model.Entity.Generic;
using System.Windows.Input;
using Exceedra.Test;

namespace Exceedra.SingleSelectCombo.ViewModel
{
    public class SingleSelectViewModel : Base
    {
        public SingleSelectViewModel()
        {

        }

        //Overload required for constructing the VM via the getData generic methods.
        //(T)Activator.CreateInstance(typeof(T), res) can't handle the bool forceInitialSelection
        public SingleSelectViewModel(XElement xml)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Items = xml.Elements().Select(x => new ComboboxItem(x)).ToList();
            });
        }

        /// <param name="useDispatcher">Using dispatcher in MVC causes the web app to crash so the dispatcher should only be used in WPF</param>
        public SingleSelectViewModel(XElement xml, bool useDispatcher = true)
        {
            if (useDispatcher)
            {
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    Init(xml);
                });
            }
            else
            {
                Init(xml);
            }
        }

        private void Init(XElement xml)
        {
            Items = xml.Elements().Select(x => new ComboboxItem(x)).ToList();
        }

        private List<ComboboxItem> _items;
        public List<ComboboxItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                if (_items != null && _items.Any())
                {
                    var selectedItem = _items.FirstOrDefault(item => item.IsSelected);
                    if (selectedItem != null) SelectedItem = selectedItem;
                    else SelectedItem = Items.First();
                }

                NotifyPropertyChanged(this, vm => vm.Items);
            }
        }

        private ComboboxItem _selectedItem;
        public ComboboxItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {

                if (value == null) return;

                if (value != null && SelectedItem != null && (value.Idx == SelectedItem.Idx) || value == SelectedItem) return;

                if ((value != null && !value.IsEnabled) && SelectedItem != null) return;

                _selectedItem = value;
                NotifyPropertyChanged(this, vm => vm.SelectedItem);
            }
        }

        private bool? _isEditable;
        public bool? IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
                if (Items != null && Items.Any())
                    Items.Do(i => i.IsEnabled = (IsEditable == true || i.IsSelected));
            }
        }

        public void SetItems(IEnumerable<ComboboxItem> items)
        {
            var temp = items.ToList();

            //if(IsEditable != null)
            //    temp.Do(i => i.IsEnabled = (IsEditable == true || i.IsSelected));

            Items = new List<ComboboxItem>(temp);
        }

        public void SetItems(XElement itemsXml)
        {
            var temp = itemsXml.Elements().Select(x => new ComboboxItem(x)).ToList();

            if (IsEditable != null)
                temp.Do(i => i.IsEnabled = (IsEditable == true || i.IsSelected));

            Items = new List<ComboboxItem>(temp);
        }

        public void SetSelection(string idx)
        {
            SelectedItem = Items.FirstOrDefault(i => i.Idx == idx);
        }

        public void Clear()
        {
            Items = new List<ComboboxItem>();
            SelectedItem = null;
        }

        private ICommand _leftCommand;
        public ICommand LeftCommand
        {
            get
            {
                return _leftCommand ?? (_leftCommand = new CommandHandler(CanMoveLeft, MoveLeft));
            }
        }

        public void MoveLeft(object o)
        {
            var selectedPos = Items.IndexOf(SelectedItem);
            Items[selectedPos - 1].IsSelected = true;
            SelectedItem = Items[selectedPos - 1];
        }

        public bool CanMoveLeft(object o)
        {
            return Items != null && Items.Any() && Items.IndexOf(SelectedItem) > 0;
        }

        private ICommand _rightCommand;
        public ICommand RightCommand
        {
            get
            {
                return _rightCommand ?? (_rightCommand = new CommandHandler(CanMoveRight, MoveRight));
            }
        }

        public void MoveRight(object o)
        {
            var selectedPos = Items.IndexOf(SelectedItem);
            Items[selectedPos + 1].IsSelected = true;
            SelectedItem = Items[selectedPos + 1];
        }

        public bool CanMoveRight(object o)
        {
            return Items != null && Items.Any() && Items.IndexOf(SelectedItem) < Items.Count() - 1;
        }
    }
}
