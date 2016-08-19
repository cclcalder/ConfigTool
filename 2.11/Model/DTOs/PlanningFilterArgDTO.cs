using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace Model
{
    public class PlanningFilterArgDTO
    {
        public IEnumerable<string> ProductIDs;
        public IEnumerable<string> CustomerIDs;
        public IEnumerable<string> MeasureIDs;
        public DateTime StartDate;
        public DateTime EndDate;
        public string IntervalIdx;
        public string SelectedTimeRangeIdx;
        public string SelectedScenarioIdx;
        public IEnumerable<Series> SelectedCharts;
    }

    public class Series
    {
        public string ProductID { get; set; }
        public string MeasureID { get; set; }

    }

}
