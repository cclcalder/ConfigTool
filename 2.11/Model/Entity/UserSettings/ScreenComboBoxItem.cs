using System.Xml.Linq;
using Exceedra.Common;
using Model.Entity.Generic;

namespace Model.Entity.UserSettings
{
    public class ScreenComboBoxItem : ComboboxItem
    {
        public string IsRobScreen { get; set; }
        public double SortOrder { get; set; }

        public ScreenComboBoxItem(XElement xml)
            : base(xml)
        {
            IsRobScreen = xml.Element("IsRobScreen").MaybeValue();
            double o = 0;
            double.TryParse(xml.Element("SortOrder").MaybeValue(), out o);
            SortOrder = o;
        }
    }
}