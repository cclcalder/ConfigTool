using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Linq
{
	public static class LinqExtensions
	{
		private static Random RandomProvider;

		static LinqExtensions()
		{
			LinqExtensions.RandomProvider = new Random();
		}

		public static void AddFormat(this IList<string> list, string format, params object[] arguments)
		{
			list.Add(string.Format(format, arguments));
		}

		public static void AddFormattedLine(this IList<string> list, string format, params object[] arguments)
		{
			list.Add(string.Format(string.Concat(format, Environment.NewLine), arguments));
		}

		public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
		{
		    if (items == null)
		        items = new List<T>();

			foreach (T item in items)
			{
				list.Add(item);
			}
		}

		public static double? AverageOrDefault<T>(this IEnumerable<T> list, Func<T, int> selector)
		{
			double? nullable;
			if (!list.None<T>())
			{
				nullable = new double?(list.Average<T>(selector));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static double? AverageOrDefault<T>(this IEnumerable<T> list, Func<T, int?> selector)
		{
			double? nullable;
			if (!list.None<T>())
			{
				nullable = list.Average<T>(selector);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static double? AverageOrDefault<T>(this IEnumerable<T> list, Func<T, double> selector)
		{
			double? nullable;
			if (!list.None<T>())
			{
				nullable = new double?(list.Average<T>(selector));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static double? AverageOrDefault<T>(this IEnumerable<T> list, Func<T, double?> selector)
		{
			double? nullable;
			if (!list.None<T>())
			{
				nullable = list.Average<T>(selector);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static IEnumerable Cast(this IEnumerable list, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type type1 = typeof(string).Assembly.GetType("System.Collections.Generic.List`1");
			Type[] typeArray = new Type[] { type };
			IList lists = (IList)Activator.CreateInstance(type1.MakeGenericType(typeArray));
			foreach (object obj in list)
			{
				lists.Add(obj);
			}
			return lists;
		}

		public static IEnumerable<IEnumerable<T>> Chop<T>(this IEnumerable<T> list, int chopSize)
		{
			bool flag;
			flag = (chopSize == 0 ? false : list.Count<T>() != 0);
			if (flag)
			{
				yield return list.Take<T>(chopSize);
				if (list.Count<T>() > chopSize)
				{
					foreach (IEnumerable<T> ts in list.Skip<T>(chopSize).Chop<T>(chopSize))
					{
						yield return ts;
					}
				}
			}
			else
			{
				yield return list;
			}
		}

		public static List<T> Clone<T>(this List<T> list)
		{
			List<T> ts;
			if (list != null)
			{
				ts = new List<T>(list);
			}
			else
			{
				ts = null;
			}
			return ts;
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> list, T item)
		{
			return list.Concat<T>(new T[] { item });
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> list, params IEnumerable<T>[] otherLists)
		{
			IEnumerable<T> ts = list;
			IEnumerable<T>[] enumerableArrays = otherLists;
			for (int i = 0; i < (int)enumerableArrays.Length; i++)
			{
				ts = ts.Concat<T>(enumerableArrays[i]);
			}
			return ts;
		}

		public static bool Contains(this IEnumerable<string> list, string instance, bool caseSensitive)
		{
			bool flag;
			flag = ((caseSensitive ? false : !instance.IsEmpty()) ? list.Any<string>((string i) => (!i.HasValue() ? false : i.ToLower() == instance.ToLower())) : list.Contains<string>(instance));
			return flag;
		}

		public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> items)
		{
			bool flag = items.All<T>((T i) => list.Contains<T>(i));
			return flag;
		}

		public static bool ContainsAny<T>(this IEnumerable<T> list, params T[] items)
		{
			return list.Intersects<T>(items);
		}

		public static IEnumerable<T> Distinct<T, TResult>(this IEnumerable<T> list, Func<T, TResult> selector)
		{
			List<TResult> tResults = new List<TResult>();
			foreach (T t in list)
			{
				TResult tResult = selector(t);
				if (!tResults.Contains(tResult))
				{
					tResults.Add(tResult);
					yield return t;
				}
			}
		}

		public static void Do<T>(this IEnumerable<T> list, LinqExtensions.ItemHandler<T> action)
		{
			if (list != null)
			{
				foreach (T t in list)
				{
					action(t);
				}
			}
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> list, Func<T, bool> criteria)
		{
			IEnumerable<T> ts = 
				from i in list
				where !criteria(i)
				select i;
			return ts;
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> list, T item)
		{
			return list.Except<T>(new T[] { item });
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> list, params T[] items)
		{
			return list.Except<T>((IEnumerable<T>)items);
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> list, Type excludedType)
		{
			if (list == null)
			{
				throw new NullReferenceException("No collection is given for the extension method Except().");
			}
			if (excludedType == null)
			{
				throw new ArgumentNullException("excludedType");
			}
			if (!excludedType.IsClass)
			{
				if (!excludedType.IsInterface)
				{
					throw new NotSupportedException(string.Concat("Except(System.Type) method does not recognize ", excludedType));
				}
				foreach (T t in list)
				{
					if (t == null)
					{
						yield return t;
					}
					if (!t.GetType().Implements(excludedType))
					{
						yield return t;
					}
				}
			}
			else
			{
				foreach (T t1 in list)
				{
					if (t1 == null)
					{
						yield return t1;
					}
					Type type = t1.GetType();
					if (type != excludedType)
					{
						if (!type.IsSubclassOf(excludedType))
						{
							yield return t1;
						}
					}
				}
			}
		}

		public static IEnumerable<T> ExceptNull<T>(this IEnumerable<T> list)
		where T : class
		{
			IEnumerable<T> ts = 
				from i in list
				where i != null
				select i;
			return ts;
		}

		public static T GetElementAfter<T>(this IEnumerable<T> list, T item)
		where T : class
		{
			T t;
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			int num = list.IndexOf<T>(item);
			if (num == -1)
			{
				throw new ArgumentException("The specified item does not exist to this list.");
			}
			t = (num != list.Count<T>() - 1 ? list.ElementAt<T>(num + 1) : default(T));
			return t;
		}

		public static T GetElementBefore<T>(this IEnumerable<T> list, T item)
		where T : class
		{
			T t;
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			int num = list.IndexOf<T>(item);
			if (num == -1)
			{
				throw new ArgumentException("The specified item does not exist to this list.");
			}
			t = (num != 0 ? list.ElementAt<T>(num - 1) : default(T));
			return t;
		}

		public static IEnumerable<T> GetKeys<T, K>(this IDictionary<T, K> dictionary)
		{
			IEnumerable<T> ts = 
				from i in dictionary
				select i.Key;
			return ts;
		}

		public static K GetOrDefault<T, K>(this IDictionary<T, K> dictionary, T key)
		{
			K k1 = dictionary.Keys.FirstOrDefault<T>((T k) => k.Equals(key)).Get<T, K>((T k) => dictionary[k]);
			return k1;
		}

		public static int IndexOf<T>(this IEnumerable<T> list, T element)
		{
			int num;
			if (list == null)
			{
				throw new NullReferenceException("No collection is given for the extension method IndexOf().");
			}
			if (list.Contains<T>(element))
			{
				int num1 = 0;
				foreach (T t in list)
				{
					if (t == null)
					{
						if (element == null)
						{
							num = num1;
							return num;
						}
					}
					else if (!t.Equals(element))
					{
						num1++;
					}
					else
					{
						num = num1;
						return num;
					}
				}
				num = -1;
			}
			else
			{
				num = -1;
			}
			return num;
		}

		public static bool Intersects<T>(this IEnumerable<T> list, IEnumerable<T> otherList)
		{
			T t = default(T);
			bool flag;
			int? nullable = (list as ICollection).Get<ICollection>((ICollection l) => l.Count);
			int? nullable1 = (otherList as ICollection).Get<ICollection>((ICollection l) => l.Count);
			if ((!nullable.HasValue || !nullable1.HasValue ? false : nullable1.Value >= nullable.Value))
			{
				foreach (T t1 in list)
				{
					if (otherList.Contains<T>(t1))
					{
						flag = true;
						return flag;
					}
				}
			}
			else
			{
				foreach (T t1 in otherList)
				{
					if (list.Contains<T>(t1))
					{
						flag = true;
						return flag;
					}
				}
			}
			flag = false;
			return flag;
		}

		public static bool Intersects<T>(this IEnumerable<T> list, params T[] items)
		{
			return list.Intersects<T>((IEnumerable<T>)items);
		}

		public static bool IsEquivalentTo<T>(this IEnumerable<T> list, IEnumerable<T> other)
		{
			bool flag;
			if (list == null)
			{
				throw new NullReferenceException();
			}
			if (other == null)
			{
				flag = false;
			}
			else if (list.Count<T>() == other.Count<T>())
			{
				foreach (T t in list)
				{
					if (other.Contains<T>(t))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool IsSingle<T>(this IEnumerable<T> list)
		{
			return list.Count<T>() == 1;
		}

		public static bool IsSingle<T>(this IEnumerable<T> list, Func<T, bool> criteria)
		{
			return list.Count<T>(criteria) == 1;
		}

		public static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> target)
		{
			return target.ContainsAll<T>(source);
		}

		public static R MaxOrDefault<T, R>(this IEnumerable<T> list, Func<T, R> expression)
		{
			R r;
			if (list == null)
			{
				throw new NullReferenceException();
			}
			r = (!list.None<T>() ? list.Max<T, R>(expression) : default(R));
			return r;
		}

		public static R MinOrDefault<T, R>(this IEnumerable<T> list, Func<T, R> expression)
		{
			R r;
			if (list == null)
			{
				throw new NullReferenceException();
			}
			r = (!list.None<T>() ? list.Min<T, R>(expression) : default(R));
			return r;
		}

		public static bool None<T>(this IEnumerable<T> list, Func<T, bool> criteria)
		{
			return !list.Any<T>(criteria);
		}

		public static bool None<T>(this IEnumerable<T> list)
		{
			return !list.Any<T>();
		}

		public static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> list, string propertyName)
		{
			if (propertyName.IsEmpty())
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo property = typeof(TSource).GetProperty(propertyName);
			if (property == null)
			{
				object[] fullName = new object[] { typeof(TSource).FullName };
				throw new ArgumentException("{0} is not a readable property of {1} type.".FormatWith(propertyName, fullName));
			}
			IEnumerable<TSource> tSources = list.OrderBy<TSource, object>(new Func<TSource, object>(new PropertyComparer(property).ExtractValue<TSource, object>));
			return tSources;
		}

		public static IEnumerable OrderBy(this IEnumerable list, string propertyName)
		{
			object current;
			IEnumerable enumerable;
			if (propertyName.IsEmpty())
			{
				throw new ArgumentNullException("propertyName");
			}
			if (!propertyName.EndsWith(" DESC"))
			{
				Type type = null;
				IEnumerator enumerator = list.GetEnumerator();
				try
				{
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
						type = current.GetType();
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				if (type != null)
				{
					PropertyInfo property = type.GetProperty(propertyName);
					if (property == null)
					{
						throw new ArgumentException(string.Concat("Unusable property name specified:", propertyName));
					}
					PropertyComparer propertyComparer = new PropertyComparer(property);
					ArrayList arrayLists = new ArrayList();
					foreach (object current1 in list)
					{
						arrayLists.Add(current1);
					}
					arrayLists.Sort(propertyComparer);
					enumerable = arrayLists;
				}
				else
				{
					enumerable = list;
				}
			}
			else
			{
				enumerable = list.OrderByDescending(propertyName.TrimEnd(" DESC".Length));
			}
			return enumerable;
		}

		public static IEnumerable OrderByDescending(this IEnumerable list, string property)
		{
			if (string.IsNullOrEmpty(property))
			{
				throw new ArgumentNullException("property");
			}
			ArrayList arrayLists = new ArrayList();
			foreach (object obj in list.OrderBy(property))
			{
				arrayLists.Insert(0, obj);
			}
			return arrayLists;
		}

		public static IEnumerable<TSource> OrderByDescending<TSource>(this IEnumerable<TSource> list, string propertyName)
		{
			if (list == null)
			{
				throw new NullReferenceException();
			}
			if (propertyName.IsEmpty())
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo property = typeof(TSource).GetProperty(propertyName);
			if (property == null)
			{
				throw new ArgumentException(string.Concat(propertyName, " is not a readable property of ", typeof(TSource).FullName, "."));
			}
			IEnumerable<TSource> tSources = list.OrderByDescending<TSource, object>(new Func<TSource, object>(new PropertyComparer(property).ExtractValue<TSource, object>));
			return tSources;
		}

		public static T PickRandom<T>(this IEnumerable<T> list)
		{
			T t;
			if (!list.Any<T>())
			{
				t = default(T);
			}
			else
			{
				int num = LinqExtensions.RandomProvider.Next(list.Count<T>());
				if (num == list.Count<T>())
				{
					num--;
				}
				t = list.ElementAt<T>(num);
			}
			return t;
		}

		public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> list, int number)
		{
			if (number < 1)
			{
				throw new ArgumentException("number should be greater than 0.");
			}
			if (number >= list.Count<T>())
			{
				number = list.Count<T>();
			}
			int num = list.Count<T>() - 1;
			List<T> ts = new List<T>();
			while (ts.Count < number)
			{
				int num1 = LinqExtensions.RandomProvider.Next(num + 1);
				if (num1 > num)
				{
					num1 = num;
				}
				T t = list.ElementAt<T>(num1);
				if (!ts.Contains(t))
				{
					ts.Add(t);
				}
			}
			return ts;
		}

		public static IEnumerable<T> Randomize<T>(this IEnumerable<T> list)
		{
			IEnumerable<T> ts;
			if (!list.None<T>())
			{
				ts = list.PickRandom<T>(list.Count<T>());
			}
			else
			{
				ts = new T[0];
			}
			return ts;
		}

		public static void Remove<T>(this IList<T> list, IEnumerable<T> itemsToRemove)
		{
			if (itemsToRemove != null)
			{
				foreach (T t in itemsToRemove)
				{
					if (list.Contains(t))
					{
						list.Remove(t);
					}
				}
			}
		}

		public static IEnumerable<T> Take<T>(this IEnumerable<T> list, int lowerBound, int upperBound)
		{
			IEnumerable<T> ts;
			if (lowerBound < 0)
			{
				throw new ArgumentOutOfRangeException("lowerBound");
			}
			if (upperBound < 0)
			{
				throw new ArgumentOutOfRangeException("upperBound");
			}
			if (lowerBound > upperBound)
			{
				throw new ArgumentException("lower bound should be smaller than upperbound.", "upperBound");
			}
			if (lowerBound != 0)
			{
				List<T> ts1 = new List<T>();
				int num = -1;
				foreach (T t in list)
				{
					num++;
					if (num >= lowerBound)
					{
						if (num <= upperBound)
						{
							ts1.Add(t);
						}
						else
						{
							break;
						}
					}
				}
				ts = ts1;
			}
			else
			{
				ts = list.Take<T>(upperBound);
			}
			return ts;
		}

		public static string ToLinesString<T>(this IEnumerable<T> list)
		{
			return list.ToString<T>(Environment.NewLine);
		}

		public static IEnumerable<string> Trim(this IEnumerable<string> list)
		{
			IEnumerable<string> array = (
				from v in list
				where v.HasValue()
				select v.Trim() into v
				where v.HasValue()
				select v).ToArray<string>();
			return array;
		}

		public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> list, TKey key)
		{
			TValue item;
			if (list.ContainsKey(key))
			{
				try
				{
					item = list[key];
					return item;
				}
				catch
				{
				}
			}
			item = default(TValue);
			return item;
		}

		public static IEnumerable<T> Union<T>(this IEnumerable<T> list, params IEnumerable<T>[] otherLists)
		{
			IEnumerable<T> ts = list;
			IEnumerable<T>[] enumerableArrays = otherLists;
			for (int i = 0; i < (int)enumerableArrays.Length; i++)
			{
				ts = ts.Union<T>(enumerableArrays[i]);
			}
			return ts;
		}

		public static T WithMax<T, TKey>(this IEnumerable<T> list, Func<T, TKey> keySelector)
		{
			T t = (
				from i in list
				orderby keySelector(i) descending
				select i).FirstOrDefault<T>();
			return t;
		}

		public static T WithMin<T, TKey>(this IEnumerable<T> list, Func<T, TKey> keySelector)
		{
			T t = (
				from i in list
				orderby keySelector(i)
				select i).FirstOrDefault<T>();
			return t;
		}

		public delegate void ItemHandler<T>(T arg);
	}
}