using System.Collections.Generic;
using System.Reflection;

namespace System
{
	public class MethodCache
	{
		private static Dictionary<MethodCache.MethodKey, MethodCache> cache;

		static MethodCache()
		{
			MethodCache.cache = new Dictionary<MethodCache.MethodKey, MethodCache>();
		}

		public MethodCache()
		{
		}

		public static MethodCache<T> Create<T>(object instance, MethodBase method, Func<T> body)
		{
			MethodCache<T> item;
			MethodCache.MethodKey methodKey = new MethodCache.MethodKey(instance, method);
			if (!MethodCache.cache.ContainsKey(methodKey))
			{
				MethodCache<T> methodCache = new MethodCache<T>(instance, method, body);
				MethodCache.cache.Add(methodKey, methodCache);
				item = methodCache;
			}
			else
			{
				item = MethodCache.cache[methodKey] as MethodCache<T>;
			}
			return item;
		}

		private class MethodKey : IEquatable<MethodCache.MethodKey>
		{
			private object instance;

			private MethodBase method;

			public MethodKey(object instance, MethodBase method)
			{
				this.instance = instance;
				this.method = method;
			}

			public bool Equals(MethodCache.MethodKey other)
			{
				bool flag;
				if (other == null)
				{
					flag = false;
				}
				else if (this.instance == other.instance)
				{
					flag = (other.method == this.method ? true : false);
				}
				else
				{
					flag = false;
				}
				return flag;
			}

			public override int GetHashCode()
			{
				return this.instance.GetHashCode();
			}
		}
	}
}