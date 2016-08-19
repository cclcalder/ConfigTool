using System;
using Model.Entity.ROBs;

namespace WPF.ViewModels.Events
{
    public class SelectAllStatus : Status
    {
        public const string IDText = "[ALL]";
        public const string NameText = "[Select all]";

        public SelectAllStatus()
        {
            Name = NameText;
            ID = IDText;
            IsActive = true;
            IsEnabled = true;
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                if (base.IsSelected == value) return;
                base.IsSelected = value;
                OnIsSelectedChanged();
            }
        }

        public bool DisableEvent { get; set; }

        public event EventHandler IsSelectedChanged;

        protected virtual void OnIsSelectedChanged()
        {
            if (DisableEvent) return;
            var handler = IsSelectedChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
