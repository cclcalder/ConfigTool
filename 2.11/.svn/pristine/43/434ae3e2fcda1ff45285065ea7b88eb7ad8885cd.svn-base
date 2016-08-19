using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace Model
{
    public class SalesOrganisation
    {
        /// <summary>
        /// Creates a new User instance.
        /// </summary>
        public static readonly string SelectAllId = "[ALL]";

        public SalesOrganisation()
        { }
        public SalesOrganisation(string id, string name)
        {
            ID = id;
            DisplayName = name;
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this User.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region DisplayName
        /// <summary>
        /// Gets or sets the DisplayName of this User.
        /// </summary>
        public string DisplayName { get; set; }
        #endregion


        private static SalesOrganisation _CurrentSalesOrg;
        public static SalesOrganisation CurrentSalesOrg
        {
            get
            {

                return _CurrentSalesOrg;
            }
            set
            {
                _CurrentSalesOrg = value;
            }

        }
  
    }
}
