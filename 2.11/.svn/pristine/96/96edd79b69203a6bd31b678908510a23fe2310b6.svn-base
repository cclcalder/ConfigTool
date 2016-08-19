using System.Collections.Generic;
using System.Linq;

namespace Model
{
    static class AggregateEx
    {
        public static decimal Aggregate(this IEnumerable<decimal> source, AggregateType aggregateType)
        {
            switch (aggregateType)
            {
                case AggregateType.Avg:
                    var onlyNonZeroValues = source.Where(v => v != 0m).ToList();
                    return onlyNonZeroValues.Count == 0 ? 0m : onlyNonZeroValues.Average();
                case AggregateType.Min:
                    return source.Min();
                case AggregateType.Max:
                    return source.Max();
                default:
                    return source.Sum();
            }
        }

        /// <summary>
        /// Planning: 
        /// Has checks for override values of -1 and 
        /// removes them from the aggregation. 
        /// </summary>
        public static decimal Aggregate(this IEnumerable<PlanningDataValue> source)
        {
            var planningDataValues = source as IList<PlanningDataValue> ?? source.ToList();

            if (!planningDataValues.Any()) return 0;

            var parentPlanningData = planningDataValues.First().ParentPlanningData;
            var aggregateType = parentPlanningData != null ? parentPlanningData.AggrType : AggregateType.Sum;

            if (planningDataValues.Select(v => v)
                .All(
                    pdv =>
                        pdv.ParentPlanningData != null &&
                        (pdv.ParentPlanningData.Measure.ToLower().Contains("override") && pdv.Data == -1)))
                return -1;

            return planningDataValues.Select(v => v)
                .Where(pdv => pdv.ParentPlanningData != null && !(pdv.ParentPlanningData.Measure.ToLower().Contains("override") && pdv.Data == -1))
                .Select(val => val.Data)
                .Aggregate(aggregateType);
        }
    }

    public enum AggregateType
    {
        Sum,
        Avg,
        Min,
        Max
    }
}