using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace Model.Entity
{
    public class SalesOrgData
    {
        private const string IndexDbColumn = "SalesOrg_Idx";
        private const string NameDbColumn = "SalesOrg_Name";
        private const string SortIdxDbColumn = "SalesOrg_SortIdx";

        public SalesOrgData(XElement el)
        {
            ID = el.GetValue<string>(IndexDbColumn);
            Name = el.GetValue<string>(NameDbColumn);
            SortIndex = el.GetValue<int>(SortIdxDbColumn);
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public int SortIndex { get; set; }

        public ObservableCollection<XElement> el = new ObservableCollection<XElement>();
    }
}