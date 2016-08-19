using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Entity
{
	public class SelectAllClaimMatchingStatus : ClaimMatchStatus
	{
        public static readonly string IDValue = "[ALL]";
        public static readonly string NameValue = "[Select all]";
		private readonly Action _onSelected;
		private readonly Action _onDeselected;
		private bool _isSelected;

        public override void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
        }

		public SelectAllClaimMatchingStatus(Action onSelected, Action onDeselected)
		{
			_onSelected = onSelected;
			_onDeselected = onDeselected;
            Name = NameValue;
            Id = IDValue;
		}

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
