namespace Model.Entity.Funds
{
    using System;

    public class Option : IEquatable<Option>
    {
        public bool Equals(Option other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ID, other.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Option) obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public static bool operator ==(Option left, Option right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Option left, Option right)
        {
            return !Equals(left, right);
        }

        public string ControlID { get; set; }
        public string ID { get; set; }
        public decimal Value { get; set; }
    }
}