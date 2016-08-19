using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Exceedra.SearchableMultiSelect
{
    public class ComobBoxSourceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        private const string IdElement = "Idx";
        private const string NameElement = "Name";
        private const string IsSelectedElement = "IsSelected";

        public ComobBoxSourceModel(XElement node)
        {
            Id = node.GetValue<string>(IdElement);
            Name = node.GetValue<string>(NameElement);
            IsSelected = node.GetValue<string>(IsSelectedElement) == "1";
        }

        public ComobBoxSourceModel(bool isSelected = false)
        {
            IsSelected = isSelected;
        }
    }
}
