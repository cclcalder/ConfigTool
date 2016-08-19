using System.Xml.Linq;

namespace Model.Entity
{
    public class ConditionScenario
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public bool IsWorkFlowDriven { get; set; }

        public static ConditionScenario FromXml(XElement node)
        {
            return new ConditionScenario
            {
                Id = node.GetValue<string>("ScenarioId"),
                Name = node.GetValue<string>("ScenarioName"),
                IsSelected = node.GetValue<int>("IsSelected") == 1,
                IsWorkFlowDriven = node.GetValue<int>("IsWorkflowDriven") == 1,
            };
        }
    }
}