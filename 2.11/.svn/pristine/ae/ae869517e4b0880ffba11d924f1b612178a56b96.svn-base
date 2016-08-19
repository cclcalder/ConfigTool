using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace Model.Entity.PowerPromotion
{
    public class PowerPromotion
    {
        public string Idx { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SelectedCustomer { get; set; }
        public string DatePeriodIdx { get; set; }
        public string SelectedStatus { get; set; }
        public List<string> SelectedProducts { get; set; }
        public List<string> SelectedSubCustomers { get; set; }
        public List<string> SelectedScenarios { get; set; } 
        //Cannot access PromoDataVM or RowVM in DataAccess so for now we send in the XML.
        public XElement DatesXml { get; set; }
        public XElement AttributesXml { get; set; }
        public XElement PAndLXml { get; set; }
        public XElement ProductMeasuresXml { get; set; }

    }
}