using System.ComponentModel;

namespace Coder.UI.WPF
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    public class BindableItemList : ObservableCollection<BindableItem>
    {
        public BindableItemList()
        {
        }

        public BindableItemList(string displaySeparator)
        {
            DisplaySeparator = displaySeparator;
        }

        public string DisplaySeparator { get; set; }

        public override string ToString()
        {
            var outString = new StringBuilder();
            foreach (BindableItem s in Items.Where(s => s.IsSelected))
            {
                outString.Append(s.Title);
                outString.Append(Convert.ToChar(DisplaySeparator));
                outString.Append(" ");
            }

            return outString.ToString().TrimEnd(new[] {Convert.ToChar(DisplaySeparator), ' '});
        }
    }

    public class BindableItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private object _item;
        private string _title;

        public string Title
        {
            get { return _title; }

            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string _colour;
        public string Colour
        {
            get { return _colour; }

            set
            {
                _colour = value;
                RaisePropertyChanged("Colour");
            }
        }


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public object Item
        {
            get { return _item; }
            set
            {
                _item = value;
                RaisePropertyChanged("Item");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}