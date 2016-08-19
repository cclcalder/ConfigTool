using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
    public class ClaimEntryEventViewModel : ViewModelBase
    {
        #region private fields

        private bool _isSelected;

        #endregion

        #region properties

        public EventBase Data { get; private set; }

        public string Name
        {
            get
            {
                return this.Data.Name;
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }

        #endregion

        #region ctors

        public ClaimEntryEventViewModel(EventBase data)
        {
            Data = data;
        }

        #endregion

    }
}
