using System.Runtime.CompilerServices;

namespace System
{
	public static class ObjectMemberAccessExtensions
	{
		public static K Get<T, K>(this T item, Func<T, K> selector)
		{
			K k;
			k = (!object.ReferenceEquals(item, null) ? selector(item) : default(K));
			return k;
		}

		public static Nullable<K> Get<T, K>(this T item, Func<T, Nullable<K>> selector)
		where K : struct
		{
			Nullable<K> nullable;
			if (item != null)
			{
				nullable = selector(item);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static Guid? Get<T>(this T item, Func<T, Guid> selector)
		where T : class
		{
			Guid? nullable;
			if (item != null)
			{
				nullable = new Guid?(selector(item));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static int? Get<T>(this T item, Func<T, int> selector)
		where T : class
		{
			int? nullable;
			if (item != null)
			{
				nullable = new int?(selector(item));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static double? Get<T>(this T item, Func<T, double> selector)
		where T : class
		{
			double? nullable;
			if (item != null)
			{
				nullable = new double?(selector(item));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static decimal? Get<T>(this T item, Func<T, decimal> selector)
		where T : class
		{
			decimal? nullable;
			if (item != null)
			{
				nullable = new decimal?(selector(item));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static bool? Get<T>(this T item, Func<T, bool> selector)
		where T : class
		{
			bool? nullable;
			if (item != null)
			{
				nullable = new bool?(selector(item));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static string Get(this DateTime? item, Func<DateTime?, string> selector)
		{
			string str;
			if (item.HasValue)
			{
				str = selector(item);
			}
			else
			{
				str = null;
			}
			return str;
		}

		public static byte? Get<T>(this T item, Func<T, byte> selector)
		where T : class
		{
			byte? nullable;
			if (item != null)
			{
				nullable = new byte?(selector(item));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static DateTime? Get<T>(this T item, Func<T, DateTime> selector)
		where T : class
		{
			DateTime? nullable;
			if (item != null)
			{
				nullable = new DateTime?(selector(item));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static DateTime? Get<T>(this T item, Func<T, DateTime?> selector)
		where T : class
		{
			DateTime? nullable;
			if (item != null)
			{
				nullable = selector(item);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static T Get<T>(this DateTime? item, Func<DateTime?, T> selector)
		where T : struct
		{
			return (item.HasValue ? selector(item) : default(T));
		}

		public static void Perform<T>(this T item, Action<T> action)
		where T : class
		{
			if (item != null)
			{
				action(item);
			}
		}
	}
}