using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
	public class ClaimValueRangeViewModel : ViewModelBase
    {
        #region properties

        public ClaimValueRange Data { get; private set; }

        public bool IsSelected
        {
            get { return Data.IsSelected; }
            set
            {
                Data.IsSelected = value;
                if (Data.Id != SelectAllClaimValueRange.IDValue)
                {
                    OnSelectedClaimValueRangeChanged(new EventArgs());
                }
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }

        #endregion

        #region ctors

        public ClaimValueRangeViewModel(ClaimValueRange data)
		{
			Data = data;
		}

        #endregion

        #region events

        public event EventHandler<EventArgs> SelectedClaimValueRangeChanged;

        #endregion

        #region protected methods

        protected virtual void OnSelectedClaimValueRangeChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectedClaimValueRangeChanged;
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
