using System.Xml.Linq;

namespace Model.Entity
{
    public class MasterCustomer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        private const string IdElement = "Idx";
        private const string NameElement = "Name";
        private const string IsSelectedElement = "IsSelected";

        public MasterCustomer(XElement node)
        {
            Id = node.GetValue<string>(IdElement);
            Name = node.GetValue<string>(NameElement);
            IsSelected = node.GetValue<string>(IsSelectedElement) == "1";
        }
    }
}
