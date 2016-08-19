using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    public class Role
    {
        /// <summary>
        /// Creates a new Role instance.
        /// </summary>
        public Role()
        { }

        public Role(XElement el)
        {
            ID = el.GetValue<string>("ID");
            Name = el.GetValue<string>("Name");
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this Role.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this Role.
        /// </summary>
        public string Name { get; set; }
        #endregion

    }
}
