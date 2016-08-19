using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
	public sealed class Base32Integer
	{
		private static string ValidChars;

		public int Value
		{
			get;
			private set;
		}

		static Base32Integer()
		{
			Base32Integer.ValidChars = "1AZ2WSX3EDC4RFV5TGB6YHN7UJM8K9LP";
		}

		public Base32Integer(int value)
		{
			this.Value = value;
		}

		public Base32Integer(string base32Integer)
		{
			this.Value = Base32Integer.FromBase32String(base32Integer);
		}

		public static int FromBase32String(string valueString)
		{
			int num = 0;
			for (int i = valueString.Length - 1; i >= 0; i--)
			{
				char chr = valueString[i];
				int num1 = Base32Integer.ValidChars.IndexOf(chr);
				num = num + (int)Math.Pow(32, (double)(valueString.Length - i - 1)) * num1;
			}
			return num;
		}

		public static implicit operator Int32(Base32Integer value)
		{
			return value.Value;
		}

		public static implicit operator Base32Integer(int value)
		{
			return new Base32Integer(value);
		}

		public static string ToBase32String(int value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			do
			{
				int num = value % 32;
				stringBuilder.Insert(0, Base32Integer.ValidChars[num]);
				value = (value - num) / 32;
			}
			while (value > 0);
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return Base32Integer.ToBase32String(this.Value);
		}
	}
}