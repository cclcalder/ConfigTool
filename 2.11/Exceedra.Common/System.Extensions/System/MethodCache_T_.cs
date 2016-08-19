using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
	public class MethodCache<T> : MethodCache
	{
		private T @value;

		private bool valueCalculated;

		private Func<T> Body
		{
			get;
			set;
		}

		public MethodCache(object instance, MethodBase method, Func<T> body)
		{
			this.Body = body;
		}

		public T Run()
		{
			if (!this.valueCalculated)
			{
				this.@value = this.Body();
				this.valueCalculated = true;
			}
			return this.@value;
		}
	}
}