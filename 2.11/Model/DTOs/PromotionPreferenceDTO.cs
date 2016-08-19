using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class PromotionPreferenceDTO
    {        

        #region Statuses
        /// <summary>
        /// Gets or sets the Products of this PlanningPreferenceDTO.
        /// </summary>
        public IEnumerable<string> Statuses { get; set; }

        public IEnumerable<ScheduleStatuses> ScheduleStatuses { get; set; }

        #endregion

        #region Customers
        /// <summary>
        /// Gets or sets the Customers of this PlanningPreferenceDTO.
        /// </summary>
        public IEnumerable<string> Customers { get; set; }
        #endregion

        #region Products
        /// <summary>
        /// Gets or sets the Products of this PlanningPreferenceDTO.
        /// </summary>
        public IEnumerable<string> Products { get; set; }
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

        #region Chart_Idx
        /// <summary>
        /// Gets or sets the Chart Idx
        /// </summary>
        public string ChartIdx { get; set; }
        #endregion

        #region IsPowerEditor
        /// <summary>
        /// Sets if the promo view model is in power editor mode
        /// </summary>
        public string IsPowerEditorString { get; set; }

        #endregion

        public string ListingsGroupIdx { get; set; }

    }
}
