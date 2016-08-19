using Model.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    public class Measure
    {
        public Measure()
        {
            
        }
        /// <summary>
        /// Creates a new Product instance.
        /// </summary>
        public Measure(XElement c)
        {
            ID = c.GetValue<string>("ID");
            DisplayName = c.GetValue<string>("DisplayName");
            IsSelected = c.GetValue<int>("IsSelected") == 1 ? true : false;
            EditableFromDate = c.GetValue<DateTime>("EditableFromDate");
            EditableToDate = c.GetValue<DateTime>("EditableToDate");
            SortOrder = c.GetValue<int>("SortOrder");
            ParentID = c.GetValue<string>("ParentId");            
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this Measure.
        /// </summary>
        public string ID { get; set; }
        #endregion

        public int SortOrder { get; set; }

        #region DisplayName
        /// <summary>
        /// Gets or sets the DisplayName of this Measure.
        /// </summary>
        public string DisplayName { get; set; }
        #endregion

        #region IsSelected
        /// <summary>
        /// Gets or sets the IsSelected of this Measure.
        /// </summary>
        public bool IsSelected { get; set; }
        #endregion

        #region EditableFromDate
        /// <summary>
        /// Gets or sets the EditableFromDate of this Measure.
        /// </summary>
        public DateTime EditableFromDate { get; set; }
        #endregion

        #region EditableToDate
        /// <summary>
        /// Gets or sets the EditableToDate of this Measure.
        /// </summary>
        public DateTime EditableToDate { get; set; }
        #endregion      

        public string ParentID { get; set; }

        private List<Measure> _Children;
        public IEnumerable<Measure> Children
        {
            get
            {
                if (_Children == null)
                {

                    //_Children = PlanningAccess.GetPlanningMeasures().Where(p => p.ParentID == ID).ToList();
 
                }
                return _Children;
            }
            internal set { _Children = value.ToList(); }

        }
    }
}
