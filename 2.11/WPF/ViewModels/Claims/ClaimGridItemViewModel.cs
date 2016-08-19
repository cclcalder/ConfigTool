using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
    public class ClaimGridItemViewModel : ViewModelBase
    {
        #region properties

        public Claim Data { get; private set; }

        public bool IsSelected
        {
            get { return Data.IsSelected; }
            set
            {
                Data.IsSelected = value;
                OnSelectionChanged(new EventArgs());
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }

        #endregion

        #region ctors

        public ClaimGridItemViewModel(Claim data)
        {
            Data = data;
        }

        #endregion

        public void UpdateIsSelected(bool isSelected)
        {
            this.Data.IsSelected = isSelected;
            NotifyPropertyChanged(this, vm => vm.IsSelected);
        }

        #region protected methods

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region events

        public event EventHandler<EventArgs> SelectionChanged;

        #endregion
    }
}
