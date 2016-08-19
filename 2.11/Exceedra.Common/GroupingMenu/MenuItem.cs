using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Exceedra.Common.GroupingMenu
{
    public class MenuItem : IMenuItem
    {
        private IList<IMenuItem> _children;
        private bool _isSelected;

        public string Id { get; set; }
        public string Header { get; set; }

        public string Url { get; set; }

        public bool HasValidUrl { get; set; }

        public string ParentId { get; set; }
        public bool HasParent
        {
            get { return !string.IsNullOrEmpty(ParentId); } 
        }

        public IList<IMenuItem> Children
        {
            get { return _children ?? (_children = new List<IMenuItem>()); }
            set { _children = value; }
        }
        public bool HasChildren
        {
            get { return Children != null && Children.Any(); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public int SortOrder { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}