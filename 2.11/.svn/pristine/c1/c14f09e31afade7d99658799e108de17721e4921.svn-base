
using Exceedra.Common;
using System.Xml.Linq;

namespace Model.Entity.Generic
{
    public class CommentComboboxItem : ComboboxItem
    {
        public CommentComboboxItem() : base()
        {

        }

        public CommentComboboxItem(XElement xml) : base(xml)
        {
            CanDelete = xml.Element("CanDelete").MaybeValue() == "1";
        }

        public bool CanDelete { get; set; }
    }
}
