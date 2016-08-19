using System;

namespace Model
{
    public class PlanningDataValueChangedEventArgs : EventArgs
    {
        private readonly PlanningDataValue _value;
        private readonly decimal _oldValue;
        private readonly decimal _newValue;

        public PlanningDataValueChangedEventArgs(PlanningDataValue value, decimal oldValue, decimal newValue)
        {
            _value = value;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public PlanningDataValue Value
        {
            get { return _value; }
        }

        public decimal NewValue
        {
            get { return _newValue; }
        }

        public decimal OldValue
        {
            get { return _oldValue; }
        }
    }
}