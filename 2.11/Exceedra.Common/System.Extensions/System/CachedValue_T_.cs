namespace System
{
	public class CachedValue<T>
	{
		private T _Value;

		private Func<T> ValueBuilder;

		public T Value
		{
			get
			{
				if (this.ValueBuilder != null)
				{
					this._Value = this.ValueBuilder();
					this.ValueBuilder = null;
				}
				return this._Value;
			}
		}

		public CachedValue(T value)
		{
			this._Value = value;
		}

		public CachedValue(Func<T> valueBuilder)
		{
			this.ValueBuilder = valueBuilder;
		}

		public static implicit operator T(CachedValue<T> c)
		{
			return c.Value;
		}

		public static implicit operator CachedValue<T>(T v)
		{
			return new CachedValue<T>(v);
		}
	}
}