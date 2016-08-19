using Exceedra.Common;

namespace Model.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Linq;

    public abstract class PhasingProfile
    {
        public sealed class EqualityComparer : IEqualityComparer<PhasingProfile>
        {
            public bool Equals(PhasingProfile x, PhasingProfile y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x._id, y._id);
            }

            public int GetHashCode(PhasingProfile obj)
            {
                return (obj._id != null ? obj._id.GetHashCode() : 0);
            }
        }

        private readonly string _id;
        private readonly Func<string, IEnumerable<decimal>> _getDetail;
        private List<decimal> _values; 

        protected PhasingProfile(string id, string name, Func<string, IEnumerable<decimal>> getDetail)
        {
            _id = id;
            Name = name;
            _getDetail = getDetail ?? (i => Enumerable.Empty<decimal>());
        }

        public string Name { get; set; }

        public string ID
        {
            get { return _id; }
        }

        public List<decimal> Values
        {
            get { return _values ?? (_values = _getDetail(ID).ToList()); }
            set { _values = value; }
        }

        public decimal Total
        {
            get { return _values.Sum(); }
        }

        public virtual int Size
        {
            get { return Values.Count; }
            set
            {
                var newValues = new decimal[value];
                Values.CopyTo(0, newValues, 0, Math.Min(value, Values.Count));
                Values = newValues.ToList();
            }
        }

        public abstract string Type { get; }

        protected abstract int MinValues { get; }
        protected abstract int MaxValues { get; }
    }

    public sealed class DayPhasingProfile : PhasingProfile
    {
        public DayPhasingProfile(string id, string name, Func<string, IEnumerable<decimal>> getDetail) : base(id, name, getDetail)
        {
        }

        public override int Size
        {
            get { return 7; }
            set { /* DO NOTHING, IN BLATANT VIOLATION OF LISKOV. */ }
        }

        protected override int MinValues
        {
            get { return 7; }
        }

        protected override int MaxValues
        {
            get { return 7; }
        }

        public override string Type
        {
            get { return "Daily"; }
        }

        public static DayPhasingProfile FromXml(XElement element, Func<string, IEnumerable<decimal>> getDetail)
        {
            return new DayPhasingProfile(element.Element("ID").MaybeValue(), element.Element("Name").MaybeValue(), getDetail);
        }
    }
    
    public sealed class WeekPhasingProfile : PhasingProfile
    {
        public WeekPhasingProfile(string id, string name, Func<string, IEnumerable<decimal>> getDetail)
            : base(id, name, getDetail)
        {
        }

        protected override int MinValues
        {
            get { return 1; }
        }

        protected override int MaxValues
        {
            get { return 52; }
        }

        public override string Type
        {
            get { return "Weekly"; }
        }

        public static WeekPhasingProfile FromXml(XElement element, Func<string, IEnumerable<decimal>> getDetail)
        {
            return new WeekPhasingProfile(element.Element("ID").MaybeValue(), element.Element("Name").MaybeValue(), getDetail);
        }
    }
}