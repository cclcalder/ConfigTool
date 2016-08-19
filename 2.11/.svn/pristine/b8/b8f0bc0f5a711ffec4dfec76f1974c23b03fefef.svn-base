using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
	public class ClaimMatchingStatusViewModel : ViewModelBase
    {
        #region properties

        public ClaimMatchStatus Data { get; private set; }

        public bool IsSelected
        {
            get { return Data.IsSelected; }
            set
            {
                Data.IsSelected = value;
                if (Data.Id != SelectAllClaimMatchingStatus.IDValue)
                {
                    OnSelectedClaimMatchingStatusChanged(new EventArgs());
                }
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }

        #endregion

        #region ctors

        public ClaimMatchingStatusViewModel(ClaimMatchStatus data)
        {
            Data = data;
        }

        #endregion

        #region events

        public event EventHandler<EventArgs> SelectedClaimMatchingStatusChanged;

        #endregion

        #region protected methods

        protected virtual void OnSelectedClaimMatchingStatusChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectedClaimMatchingStatusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public void SetSelected(bool isSelected)
        {
            Data.SetSelected(isSelected);
            NotifyPropertyChanged(this, vm => vm.IsSelected);
        }
    }
}
