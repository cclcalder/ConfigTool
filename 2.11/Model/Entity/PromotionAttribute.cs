using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Annotations;

    [Serializable()]
    public class PromotionAttribute : INotifyPropertyChanged
    {
        public PromotionAttribute(XElement el)
        {
            ID = el.GetValue<string>("ID");
            Name = el.GetValue<string>("Name");
            IsMultiSelect = el.Element("IsMultiSelect").MaybeValue() == "1";
            Options = el.Element("Options").MaybeElements("Option").Select(op => new PromotionAttributeOption(op)).ToList();

            SelectedOptions = new ObservableCollection<PromotionAttributeOption>(Options.Where(o => o.IsSelected));
        }

        /// <summary>
        /// Gets or sets the Id of this PromotionAttribute.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the Name of this PromotionAttribute.
        /// </summary>
        public string Name { get; set; }

        public bool IsMultiSelect { get; private set; }
        public bool IsSingleSelect { get { return !IsMultiSelect; } }

        private List<PromotionAttributeOption> _options;
        public List<PromotionAttributeOption> Options
        {
            get { return _options; }
            set { _options = value.ToList(); }
        }
        
        private ObservableCollection<PromotionAttributeOption> _selectedOptions;
        public ObservableCollection<PromotionAttributeOption> SelectedOptions
        {
            get { return _selectedOptions; }
            set
            {
                _selectedOptions = value;
                OnPropertyChanged("SelectedOptions");
            }
        }

        public PromotionAttributeOption SelectedOption {
            get { return SelectedOptions.FirstOrDefault(); }
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
