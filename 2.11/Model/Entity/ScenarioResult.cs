using System.Xml.Linq;

namespace Model.Entity
{
    public class ScenarioResult
    {
        private string _id;
        private string _outcome;

        public ScenarioResult()
        {
            
        }
        public ScenarioResult(XElement el)
        {
            ID = el.GetValue<string>("Scen_Idx");
            Outcome = el.GetValue<string>("Outcome");
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
        public string Outcome
        {
            get { return _outcome; }
            set { _outcome = value; }
        }
    }
}