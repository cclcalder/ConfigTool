using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Types which desn't belong to Domain Model, but used for transportation to the services
    /// </summary>
    public class PlanningPreferenceDTO
    {
        #region IntervalId
        /// <summary>
        /// Gets or sets the IntervalId of this PlanningPreferenceDTO.
        /// </summary>
        public string IntervalId { get; set; }
        #endregion

        #region Products
        /// <summary>
        /// Gets or sets the Products of this PlanningPreferenceDTO.
        /// </summary>
        public IEnumerable<string> Products { get; set; }
        #endregion

        #region Customers
        /// <summary>
        /// Gets or sets the Customers of this PlanningPreferenceDTO.
        /// </summary>
        public IEnumerable<string> Customers { get; set; }
        #endregion

        #region DateStart
        /// <summary>
        /// Gets or sets the DateStart of this PlanningPreferenceDTO.
        /// </summary>
        public string DateStart { get; set; }
        #endregion

        #region DateEnd
        /// <summary>
        /// Gets or sets the DateEnd of this PlanningPreferenceDTO.
        /// </summary>
        public string DateEnd { get; set; }
        #endregion

        public string ScenarioID { get; set; }

        public IEnumerable<string> Measures { get; set; }
        public string TimeRangeID { get; set; } 

        public List<string> SelectedCharts { get; set; }

        public string ListingsGroupIdx { get; set; }

        public string HierarchyIdx { get; set; }
    }
}
