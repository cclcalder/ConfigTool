using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims.EventStatusV2
{
    public class EventStatusViewModel : ViewModelBase
    {
        
        #region properties

        public EventStatus Data { get; private set; }

        public bool IsSelected
        {
            get { return Data.IsSelected; }
            set
            {
                Data.IsSelected = value;
                if (Data.Id != SelectAllEventStatus.IDValue)
                {
                    OnSelectedEventStatusChanged(new EventArgs());
                }
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }

        #endregion

        #region ctors

        public EventStatusViewModel(EventStatus data)
		{
			Data = data;
        }

        #endregion

        #region events

        public event EventHandler<EventArgs> SelectedEventStatusChanged;

        #endregion

        #region protected methods

        protected virtual void OnSelectedEventStatusChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectedEventStatusChanged;
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
