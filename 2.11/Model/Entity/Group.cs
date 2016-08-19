using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class Group
    {
        #region Name
        /// <summary>
        /// Gets or sets the Name of this Group.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Reports
        /// <summary>
        /// Gets or sets the Reports of this Group.
        /// </summary>
        public List<Report> Reports { get; set; }
        #endregion
    }
}
