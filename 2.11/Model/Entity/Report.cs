using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Exceedra.Common.Utilities;

namespace Model
{
    public class Report : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the Name of this Report.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Url of this Report.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the IsDefault of this Report.
        /// </summary>
        public bool IsDefault { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                PropertyChanged.Raise(this, "IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
