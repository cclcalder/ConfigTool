using System;
using System.Collections.Generic;
using System.Linq;
using Model.DataAccess;
using System.Collections.ObjectModel;
using Exceedra.Common;

namespace Model
{
    public class PlanningDataItem
    {
        public PlanningDataItem()
        {
            ChildItems = new ObservableCollection<PlanningDataItem>();
            _valuesForWeek = new IndexedProperty<string, IEnumerable<PlanningDataValue>>(s => Measures.SelectMany(m => m.Values).Where(v => v.ColumnId == s));
        }

        public List<PlanningData> Measures { get; internal set; }

        private readonly IndexedProperty<string, IEnumerable<PlanningDataValue>> _valuesForWeek;

        public ILookup<string, IEnumerable<PlanningDataValue>> ValuesForWeek
        {
            get { return _valuesForWeek; }
        }

        public ObservableCollection<PlanningDataItem> ChildItems { get; set; }

        public string Idx { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set { _displayName = value; }
        }

        public delegate void DataValuesChangedEventHandler(object sender, PlanningDataValueChangedEventArgs e);
        public event DataValuesChangedEventHandler Changed;

        public PlanningDataItem Parent { get; set; }
        
        internal void OnDataValuesChanged(PlanningDataValueChangedEventArgs e)
        {
            if (NonSideEffectingScope.IsActive) return;
            
            if (Parent != null)
            {
                Parent.Recalculate(e.Value.ParentPlanningData.MeasureId, e.Value.ColumnId);
                Parent.OnDataValuesChanged(e);
            }

            //When my value changes, lock my children and parents
            if (e.Value.ParentPlanningData.ContainingPlanningDataProduct == this)
            {
                if (ChildItems.Any())
                {
                    LockChildren(e.Value.ParentPlanningData.MeasureId, e.Value.ColumnId);
                }

                if (Parent != null)
                {
                    LockParents(e.Value.ParentPlanningData.MeasureId, e.Value.ColumnId);
                }
            }

            if (Changed != null)
                Changed(this, e);
        }

        private void LockParents(string measureId, string columnId)
        {
            Parent.ValuesForWeek[columnId]
                .Where(v => v.ParentPlanningData.MeasureId == measureId)
                .Do(v => v.IsEditable = false);
        }

        private void LockChildren(string measureId, string columnId)
        {
            this
                .Flatten(p => p.ChildItems, false)
                .SelectMany(p => p.ValuesForWeek[columnId])
                .Where(v => v.ParentPlanningData.MeasureId == measureId)
                .Do(v => v.IsEditable = false);   
        }
        
        private void Recalculate(string measureId, string columnId)
        {
            var planningData = ValueForWeekAndMeasure(columnId, measureId);
            planningData.DataInternal = ChildItems
                .Select(p => p.ValueForWeekAndMeasure(columnId, measureId))
                .Aggregate();
            /* This will update the totalValue column */
            Measures.First(m => m.MeasureId == measureId).OnDataValuesChanged(null);
        }

        private void RecalculateInternal(string measureId, string columnId)
        {
            var planningData = ValueForWeekAndMeasure(columnId, measureId);
            planningData.DataInternal = planningData.OriginalData = ChildItems
                .Select(p => p.ValueForWeekAndMeasure(columnId, measureId))
                .Aggregate();
        }

        internal void RecalculateAll()
        {
            if (!ChildItems.Any()) return;
            foreach (var measure in Measures)
            {
                foreach (var planningDataValue in measure.Values)
                {
                    RecalculateInternal(measure.MeasureId, planningDataValue.ColumnId);
                }
            }
        }

        private PlanningDataValue ValueForWeekAndMeasure(string weekId, string measureId)
        {
            return ValuesForWeek[weekId].Single(m => m.ParentPlanningData.MeasureId == measureId);
        }
        
        
    }
}
