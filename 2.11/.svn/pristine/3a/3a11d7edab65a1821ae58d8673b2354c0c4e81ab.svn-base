using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity.Funds
{
    public class CommentType
    {
        private string _id;
        private string _name;
        private bool _canDelete;
        private bool _isSelected;

        public CommentType(XElement el)
        {
            ID = el.GetValue<string>("ID");
            Name = el.GetValue<string>("Name");
            CanDelete = el.Element("CanDelete") != null && el.Element("CanDelete").MaybeValue() == "1";
            IsSelected = el.Element("IsSelected") != null && el.Element("IsSelected").MaybeValue() == "1";
        }

        public CommentType(string id, string name, bool canDelete)
        {
            _id = id;
            _name = name;
            _canDelete = canDelete;
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

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public bool CanDelete
        {
            get { return _canDelete; }
            set { _canDelete = value; }
        }

        public static CommentType FromXml(XElement xml)
        {
            if (xml == null) return null;

            return new CommentType(xml);
        }
    }
}
