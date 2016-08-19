using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using Exceedra.Common.Utilities;

namespace Model
{
    using System.Globalization;

    public class PlanningDataValue : DependencyObject, INotifyPropertyChanged
    {
        private static readonly Regex OnlyDigits = new Regex(@"[^-\d\.,\(\)]", RegexOptions.Compiled);

        internal PlanningDataValue() { }

        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the Column of this PlanningDataValue.
        /// </summary>
        public string ColumnId { get; set; }

        private decimal _originalData;
        public decimal OriginalData
        {
            get
            {
                return _originalData;
            }
            set
            {
                _originalData = value;
            }
        }

        public decimal Round(decimal value)
        {
            // Multiply by 100 for currency values...
            value = value * 100;
            // Ignore 2 decimal places because we multiplied by 100
            var places = Math.Min(_originalData.GetPlaces(), DataInternal.GetPlaces()) - 2;
            if (places < 0) places = 0;
            // Round the value and re-divide by 100
            return decimal.Round(value, places, MidpointRounding.AwayFromZero) / 100;
        }

        /// <summary>
        /// Internal Data property for initial value setting.
        /// </summary>
        /// <remarks>
        /// This property is non-side-effecting, changing its value will not cause recalculations etc.
        /// For a side-effecting setter use Data instead.
        /// </remarks>
        internal decimal DataInternal
        {
            get { return (decimal)GetValue(DataProperty); }
            set
            {
                SetValue(DataProperty, value);
                PropertyChanged.Raise(this, "DataString");
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(decimal), typeof(PlanningDataValue), new UIPropertyMetadata(0m));

        /// <summary>
        /// Publicly exposed Data property for UI binding etc
        /// </summary>
        /// <remarks>
        /// Causes side-effects such as recalculations and parents locking their child products.
        /// For a non-side-effecting change use DataInternal instead.
        /// </remarks>
        public decimal Data
        {
            get { return DataInternal; }
            set
            {
                //if (value == -1) // System sees -1 as 0 - TFS:410
                //    value = 0;

                if (DataInternal != value)
                {

                    var oldValue = DataInternal;
                    DataInternal = value;

                    if (ParentPlanningData != null)
                    {
                        ParentPlanningData.OnDataValuesChanged(new PlanningDataValueChangedEventArgs(this, oldValue, Data));
                    }
                }
            }
        }

        /// <summary>
        /// Value of Data formatted according to the the format string defined in 
        /// Format of the parent PlanningData object.
        /// </summary>
        public string DataString
        {
            get
            {
                return FormattedData(Data, ParentPlanningData);
            }
            set
            {
                Data = ParseData(value);
            }
        }

        /// <summary>
        /// Gets or sets the Column of this PlanningDataValue.
        /// </summary>
        public string ColumnName { get; set; }

        private bool _isEditable;

        /// <summary>
        /// Gets or sets the IsEditable of this PlanningDataValue.
        /// </summary>
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                PropertyChanged.Raise(this, "IsEditable");
                PropertyChanged.Raise(this, "IsReadOnly");
            }
        }

        public bool IsReadOnly { get { return !IsEditable; } }

        private string _color = "#FFFFFFFF";
        /// <summary>
        /// Gets or sets the Color of this PlanningDataValue.
        /// </summary>
        public string Color { get { return _color; } set { _color = value; } }

        /// <summary>
        /// Gets or sets the IsBold of this PlanningDataValue.
        /// </summary>
        public bool IsBold { get; set; }

        /// <summary>
        /// Gets or sets the ParentPlanningData of this PlanningDataValue.
        /// </summary>
        public PlanningData ParentPlanningData { get; set; }

        internal string OriginalComment { get; set; }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;

                    if (ParentPlanningData != null)
                    {
                        var val = Data;
                        ParentPlanningData.OnDataValuesChanged(new PlanningDataValueChangedEventArgs(this, val, val));
                    }

                    PropertyChanged.Raise(this, "Comment");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal static string FormattedData(decimal data, PlanningData planningData)
        {
            if (planningData == null || string.IsNullOrWhiteSpace(planningData.Format))
            {
                if (Math.Truncate(data) == data)
                    return data.ToString("N0");
                return data.ToString(CultureInfo.CurrentUICulture);
            }
            return data.ToString(planningData.Format);
        }

        private static decimal ParseData(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            if (value.Trim() == "-") return 0;
            return decimal.Parse(OnlyDigits.Replace(value, string.Empty));
        }

        public bool IsDataChanged
        {
            get
            {
                if (Round(OriginalData) != Round(DataInternal))
                {
                    return true;
                }
#if(DEBUG)
                var current = FormattedData(DataInternal, ParentPlanningData);
                var original = FormattedData(OriginalData, ParentPlanningData);
                if (original.Equals(current, StringComparison.OrdinalIgnoreCase))
                    return false;
                return true;
#endif
                return !FormattedData(DataInternal, ParentPlanningData)
                    .Equals(FormattedData(OriginalData, ParentPlanningData), StringComparison.OrdinalIgnoreCase);
            }
        }

        public void Reset()
        {
            _originalData = Data;
        }
    }

    internal static class DecimalEx
    {
        public static int GetPlaces(this decimal value)
        {
            return (decimal.GetBits(value)[3] & 16711680) >> 16;
        }
    }
}
