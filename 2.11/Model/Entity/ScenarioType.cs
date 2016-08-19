using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ScenarioType
    {
        private string _id;
        private string _name;

        public ScenarioType(XElement el)
        {
            ID = el.GetValue<string>("Scen_Type_Idx");
            Name = el.GetValue<string>("Scen_Type_Name");
        }

        public ScenarioType(string id,string name)
        {
            _id = id;
            _name = name;
        }

        /// <summary>
        ///     Gets or sets the ID of this PlanningScenario.
        /// </summary>
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        ///     Gets or sets the Name of this PlanningScenario.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
