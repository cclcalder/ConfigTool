namespace System
{
	public struct ShortGuid
	{
		public readonly static ShortGuid Empty;

		private System.Guid _guid;

		private string _value;

		public System.Guid Guid
		{
			get
			{
				return this._guid;
			}
			set
			{
				if (value != this._guid)
				{
					this._guid = value;
					this._value = ShortGuid.Encode(value);
				}
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					this._guid = ShortGuid.Decode(value);
				}
			}
		}

		static ShortGuid()
		{
			ShortGuid.Empty = new ShortGuid(System.Guid.Empty);
		}

		public ShortGuid(string value)
		{
			this._value = value;
			this._guid = ShortGuid.Decode(value);
		}

		public ShortGuid(System.Guid guid)
		{
			this._value = ShortGuid.Encode(guid);
			this._guid = guid;
		}

		public static System.Guid Decode(string value)
		{
			value = value.Replace("_", "/").Replace("-", "+");
			byte[] numArray = Convert.FromBase64String(string.Concat(value, "=="));
			return new System.Guid(numArray);
		}

		public static string Encode(string value)
		{
			return ShortGuid.Encode(new System.Guid(value));
		}

		public static string Encode(System.Guid guid)
		{
			string base64String = Convert.ToBase64String(guid.ToByteArray());
			base64String = base64String.Replace("/", "_").Replace("+", "-");
			return base64String.Substring(0, 22);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is ShortGuid)
			{
				flag = this._guid.Equals(((ShortGuid)obj)._guid);
			}
			else if (!(obj is System.Guid))
			{
				flag = (!(obj is string) ? false : this._guid.Equals(((ShortGuid)obj)._guid));
			}
			else
			{
				flag = this._guid.Equals((System.Guid)obj);
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this._guid.GetHashCode();
		}

		public static ShortGuid NewGuid()
		{
			return new ShortGuid(System.Guid.NewGuid());
		}

		public static bool operator ==(ShortGuid x, ShortGuid y)
		{
			bool flag;
			flag = ((object)x != null ? x._guid == y._guid : (object)y == null);
			return flag;
		}

		public static implicit operator String(ShortGuid shortGuid)
		{
			return shortGuid._value;
		}

		public static implicit operator Guid(ShortGuid shortGuid)
		{
			return shortGuid._guid;
		}

		public static implicit operator ShortGuid(string shortGuid)
		{
			return new ShortGuid(shortGuid);
		}

		public static implicit operator ShortGuid(System.Guid guid)
		{
			return new ShortGuid(guid);
		}

		public static bool operator !=(ShortGuid x, ShortGuid y)
		{
			return !(x == y);
		}

		public static ShortGuid Parse(string text)
		{
			ShortGuid empty;
			if (!text.IsEmpty())
			{
				empty = (text.Length < 30 ? new ShortGuid(text) : new ShortGuid(new System.Guid(text)));
			}
			else
			{
				empty = ShortGuid.Empty;
			}
			return empty;
		}

		public override string ToString()
		{
			return this._value;
		}
	}
}