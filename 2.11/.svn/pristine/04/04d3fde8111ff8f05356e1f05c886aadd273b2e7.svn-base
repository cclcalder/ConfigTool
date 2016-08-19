using Exceedra.Common;

namespace Model
{
    using System;
    using System.Xml.Linq;

    public class Scenario : IEquatable<Scenario>
    {
        private readonly string _id;
        private readonly string _name;
        private readonly bool _isSelected;

        public Scenario(string id, string name, bool isSelected = false)
        {
            if (id == null) throw new ArgumentNullException("id");
            _id = id;
            _name = name;
            _isSelected = isSelected;
        }

        /// <summary>
        ///     Gets or sets the ID of this PlanningScenario.
        /// </summary>
        public string ID
        {
            get { return _id; }
        }

        /// <summary>
        ///     Gets or sets the Name of this PlanningScenario.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
        }

        public bool Equals(Scenario other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_id, other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Scenario)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(Scenario left, Scenario right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Scenario left, Scenario right)
        {
            return !Equals(left, right);
        }

        public static Scenario FromXml(XElement xml)
        {
            return new Scenario(xml.Element("ID").MaybeValue(), xml.Element("Name").MaybeValue(), xml.Element("IsSelected").MaybeValue() == "1");
        }
    }
}