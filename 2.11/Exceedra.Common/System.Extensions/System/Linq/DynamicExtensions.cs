using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Linq
{
	public static class DynamicExtensions
	{
		public static IEnumerable<K> Select<T, K>(this IEnumerable<T> list, string query)
		{
			return (new DynamicExpressionsCompiler<T>(list)).Select<K>(query);
		}

		public static IEnumerable<object> Select<T>(this IEnumerable<T> list, string query)
		{
			return list.Select<T, object>(query);
		}

		public static IEnumerable<T> Where<T>(this IEnumerable<T> list, string criteria)
		{
			return (new DynamicExpressionsCompiler<T>(list)).Where(criteria);
		}

		public static IEnumerable Where(this IEnumerable list, string criteria, Type type)
		{
			IEnumerable enumerable = (new DynamicExpressionsCompiler<object>(list.OfType<object>(), type)).Where(criteria);
			return enumerable;
		}
	}
}