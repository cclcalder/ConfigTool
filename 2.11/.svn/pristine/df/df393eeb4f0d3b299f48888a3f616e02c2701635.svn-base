using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Exceedra.Common.Utilities;
using Model.Entity;

namespace Model
{
    public class PlanningData : INotifyPropertyChanged
    {
        internal PlanningData() {}
        
        /// <summary>
        /// Gets or sets the ProductId of this PlanningData.
        /// </summary>
        public string ProductId { get; set; }
        
        /// <summary>
        /// Gets or sets the MeasureId of this PlanningData.
        /// </summary>        
        public string MeasureId { get; set; }
        
        /// <summary>
        /// Gets or sets the Measure of this PlanningData.
        /// </summary>
        private string _Measure;
        public string Measure
        {
            get
            {
                if (_Measure == null)
                    _Measure = PlanningMeasures.GetMeasureName(MeasureId);

                return _Measure;
            }
            set { _Measure = value; }
        }
        
        /// <summary>
        /// Gets or sets the Values of this PlanningData.
        /// </summary>       
        public List<PlanningDataValue> Values { get; set; }
        
        /// <summary>
        /// Gets or sets the Format of this PlanningData.
        /// </summary>
        public string Format { get; set; }
        
        /// <summary>
        /// Gets or sets the IsSelectedInChart of this PlanningData.
        /// </summary>
        public bool IsSelectedInChart { get; internal set; }
        
        /// <summary>
        /// Gets or sets the AggrType of this PlanningData.
        /// </summary>
        public AggregateType AggrType { get; set; }
        
        /// <summary>
        /// Gets or sets the Total of this PlanningData.
        /// </summary>
        public string Aggr
        {
            get
            {
                decimal tot = Values.Aggregate();
                return PlanningDataValue.FormattedData(tot, this);
            }
        }
        
        /// <summary>
        /// Fired on change of the Data property of any of this objects child PlanningDataValue objects.
        /// </summary>
        public event EventHandler<PlanningDataValueChangedEventArgs> DataChanged;

        /// <summary>
        /// Product to which this data "row" belongs.
        /// </summary>
        public PlanningDataItem ContainingPlanningDataProduct { get; set; }
        
        /// <summary>
        /// Gets the TotalValue of this PlanningData.
        /// </summary>
        public string TotalValue
        {
            //get { return String.Format("{0} ({1})", Aggr, AggrType.ToString("G").ToLower()); }
            get { return String.Format("{0}", Aggr); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        

        /// <summary>
        /// Gets or sets the DataValuesChanged of this PlanningData.
        /// </summary>
        /// <param name="planningDataValueChangedEventArgs"></param>
        public void OnDataValuesChanged(PlanningDataValueChangedEventArgs planningDataValueChangedEventArgs)
        {
            if (planningDataValueChangedEventArgs == null)
            {
                PropertyChanged.Raise(this, "TotalValue");
                return;
            }

            if (ContainingPlanningDataProduct != null)
            {
                ContainingPlanningDataProduct.OnDataValuesChanged(planningDataValueChangedEventArgs);
            }

            if (DataChanged != null)
            {
                DataChanged(this, planningDataValueChangedEventArgs);
            }

            PropertyChanged.Raise(this, "TotalValue");
        }

        public bool HasChanges()
        {
            return Values.Any(v => v.IsDataChanged || v.Comment != v.OriginalComment)
                   || (ContainingPlanningDataProduct != null && ContainingPlanningDataProduct.ChildItems.Any(p => p.Measures.Any(m => m.HasChanges())));
        }

        /* When saving we only need to send the lowest level where the changes were made
         * e.g. I change planning level, which visually aggregates up the tree, but I only need to send the value I typed, and not all the parents that changed.
         */
        public bool LowestLevelChanges()
        {
            if(ContainingPlanningDataProduct != null && ContainingPlanningDataProduct.ChildItems.Any(p => p.Measures.Any(m => m.LowestLevelChanges())))
            {
                return false;
            }

            return Values.Any(v => v.IsDataChanged || v.Comment != v.OriginalComment);
        }
    }
}
