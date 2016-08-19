using System.Xml.Linq;

namespace Model.Entity
{
    public class ScenarioStatus
    {
        private string _id;
        private string _name;
        private bool _isEditable;

        public ScenarioStatus(XElement el)
        {
            ID = el.GetValue<string>("Scen_Status_Idx");
            Name = el.GetValue<string>("Scen_Status_Name");
            IsEditable = el.GetValue<string>("IsEditable") == "1";
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

        public bool IsEditable 
        { 
            get { return _isEditable; } 
            set { _isEditable = value; } 
        }
    }
}