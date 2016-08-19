using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
	public class ClaimStatusViewModel : ViewModelBase
    {

        #region properties

        public ClaimStatus Data { get; private set; }

        public bool IsSelected
        {
            get { return Data.IsSelected; }
            set
            {
                Data.IsSelected = value;
                if (Data.Id != SelectAllClaimStatus.IDValue)
                {
                    OnSelectedClaimStatusChanged(new EventArgs());
                }
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }

        #endregion

        #region ctors

        public ClaimStatusViewModel(ClaimStatus data)
		{
			Data = data;
        }

        #endregion

        #region events

        public event EventHandler<EventArgs> SelectedClaimStatusChanged;

        #endregion

        #region protected methods

        protected virtual void OnSelectedClaimStatusChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectedClaimStatusChanged;
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
