using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
	public class EventTypeViewModel: ViewModelBase
    {
        #region properties

        public EventType Data { get; private set; }

        public bool IsSelected
        {
            get { return Data.IsSelected; }
            set
            {
                Data.IsSelected = value;
                if (Data.Id != SelectAllEventTypes.IDValue)
                {
                    OnSelectedEventTypeChanged(new EventArgs());
                }
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }

        #endregion

        #region ctors

        public EventTypeViewModel(EventType data)
		{
			Data = data;
        }

        #endregion

        #region events

        public event EventHandler<EventArgs> SelectedEventTypeChanged;

        #endregion

        #region protected methods

        protected virtual void OnSelectedEventTypeChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectedEventTypeChanged;
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
