using System;

namespace Model
{
    public class SelectAllConditionStatus : ConditionStatus
    {
        private readonly Action _onSelected;
        private readonly Action _onDeselected;
        private bool _isSelected;
        public const string IdText = "[ALL]";
        public const string NameText = "[Select all]";

        public SelectAllConditionStatus(Action onSelected, Action onDeselected)
        {
            _onSelected = onSelected;
            _onDeselected = onDeselected;
            Name = NameText;
            Id = IdText;
        }

        public bool DisableEvent { get; set; }

        public override bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (value)
                    _onSelected();
                else
                    _onDeselected();
            }
        }
    }
}