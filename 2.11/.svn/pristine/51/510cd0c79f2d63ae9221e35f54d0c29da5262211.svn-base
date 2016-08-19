using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
 

namespace System
{
	public static class SystemExtensions
	{
		public static void Add<T, K>(this IDictionary<Type, Type> typeDictionary)
		{
			typeDictionary.Add(typeof(T), typeof(K));
		}

		public static void AddFormattedLine(this StringBuilder r, string format, params object[] args)
		{
			try
			{
				r.AppendFormat(format, args);
				r.AppendLine();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string[] strArrays = new string[] { "Could not add formatted line of \"", format, "\" with the folliwing parameters: {", args.ToString<object>(", "), "}." };
				throw new FormatException(string.Concat(strArrays), exception);
			}
		}

		public static DateTime AddWorkingDays(this DateTime date, int days)
		{
			if (days < 1)
			{
				throw new ArgumentException("AddWorkingDays() requires a valid positive integer value.");
			}
			DateTime dateTime = date.NexWorkingDay();
			for (int i = 1; i < days; i++)
			{
				dateTime = dateTime.NexWorkingDay();
			}
			return dateTime;
		}

		public static void AppendIf(this StringBuilder r, string text, bool condition)
		{
			if (condition)
			{
				r.Append(text);
			}
		}

		public static void AppendLineIf(this StringBuilder r, string text, bool condition)
		{
			if (condition)
			{
				r.AppendLine(text);
			}
		}

		public static void AppendLineIf(this StringBuilder r, string text)
		{
			if (text.HasValue())
			{
				r.AppendLine(text);
			}
		}

		public static bool ContainsAll(this string text, string[] keywords, bool caseSensitive)
		{
			bool flag;
			if (!caseSensitive)
			{
				text = text.ToLower();
				for (int i = 0; i < (int)keywords.Length; i++)
				{
					keywords[i] = keywords[i].ToLower();
				}
			}
			string[] strArrays = keywords;
			int num = 0;
			while (true)
			{
				if (num >= (int)strArrays.Length)
				{
					flag = true;
					break;
				}
				else if (text.Contains(strArrays[num]))
				{
					num++;
				}
				else
				{
					flag = false;
					break;
				}
			}
			return flag;
		}
         
        public static string GetDisplayName(this Type input)
		{
			string name = input.Name;
			for (int i = name.Length - 1; i >= 0; i--)
			{
				if (name[i] == char.ToUpper(name[i]) && i > 0)
				{
					name = name.Insert(i, " ");
				}
			}
			return name;
		}

		public static IEnumerable<Type> GetParentTypes(this Type type)
		{
			List<Type> types = new List<Type>();
			for (Type i = type.BaseType; i != null; i = i.BaseType)
			{
				types.Add(i);
			}
			return types;
		}

		public static object GetValue(this PropertyInfo property, object @object)
		{
			return property.GetValue(@object, null);
		}

		public static bool Implements<T>(this Type type)
		{
			return type.Implements(typeof(T));
		}

		public static bool Implements(this Type type, Type interfaceType)
		{
			bool flag;
			if (interfaceType == null)
			{
				throw new ArgumentNullException("interfaceType");
			}
			if (!interfaceType.IsInterface)
			{
				throw new ArgumentException(string.Concat("The provided value for interfaceType, ", interfaceType.FullName, " is not an interface type."));
			}
			if (type != interfaceType)
			{
				Type @interface = type.GetInterface(interfaceType.FullName, false);
				flag = (@interface != null ? @interface.FullName == interfaceType.FullName : false);
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public static bool InhritsFrom(this Type type, Type baseType)
		{
			bool flag;
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			flag = (!baseType.IsInterface ? type.GetParentTypes().Contains<Type>(baseType) : baseType.Implements(baseType));
			return flag;
		}

		public static bool Is<T>(this string text)
		where T : struct
		{
			return text.TryParseAs<T>().HasValue;
		}

		public static bool IsBetween(this DateTime date, DateTime from, DateTime to)
		{
			bool flag;
			if (from > to)
			{
				throw new ArgumentException("\"From\" date should be smaller than or equal to \"To\" date.");
			}
			if (!(date < from))
			{
				flag = (!(date > to) ? true : false);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool IsWeekend(this DateTime value)
		{
			return (value.DayOfWeek == DayOfWeek.Sunday ? true : value.DayOfWeek == DayOfWeek.Saturday);
		}

		public static DateTime NexWorkingDay(this DateTime date)
		{
			DateTime dateTime = date.AddDays(1);
			while (dateTime.IsWeekend())
			{
				dateTime = dateTime.AddDays(1);
			}
			return dateTime;
		}

		public static bool References(this Assembly assembly, Assembly anotherAssembly)
		{
			if (assembly == null)
			{
				throw new NullReferenceException();
			}
			if (anotherAssembly == null)
			{
				throw new ArgumentNullException("anotherAssembly");
			}
			bool flag = assembly.GetReferencedAssemblies().Any<AssemblyName>((AssemblyName each) => each.FullName.Equals(anotherAssembly.FullName));
			return flag;
		}

		public static double Round(this double value, int digits)
		{
			return Math.Round(value, digits);
		}

		public static void SetValue(this PropertyInfo property, object @object, object value)
		{
			property.SetValue(@object, value, null);
		}

		public static ShortGuid Shorten(this Guid guid)
		{
			return new ShortGuid(guid);
		}

		public static IEnumerable<string> Split(this string text, int chunkSize)
		{
			if (text.HasValue())
			{
				if (text.Length <= chunkSize)
				{
					yield return text;
				}
				else
				{
					yield return text.Substring(0, chunkSize);
					foreach (string str in text.Substring(chunkSize).Split(chunkSize))
					{
						yield return str;
					}
				}
			}
		}

		public static string Substring(this string text, string from, string to, bool inclusive)
		{
			string empty;
			int length = text.IndexOf(from);
			int num = text.IndexOf(to);
			if (length == -1)
			{
				empty = string.Empty;
			}
			else if (num == -1)
			{
				empty = string.Empty;
			}
			else if (num >= length)
			{
				if (!inclusive)
				{
					length = length + from.Length;
				}
				else
				{
					num = num + to.Length;
				}
				empty = text.Substring(length, num - length);
			}
			else
			{
				empty = string.Empty;
			}
			return empty;
		}

		public static T To<T>(this string text)
		where T : struct
		{
			T t;
			if (typeof(T) != typeof(Color))
			{
				t = (typeof(T) != typeof(Guid) ? (T)Convert.ChangeType(text, typeof(T)) : (T)(object)(new Guid(text)));
			}
			else
			{
				if ((!text.StartsWith("#") ? true : text.Length != 7))
				{
					throw new Exception("Invalid color text. Expected format is #RRGGBB.");
				}
				t = (T)(object)Color.FromArgb(int.Parse(text.TrimStart("#").WithPrefix("FF"), NumberStyles.HexNumber));
			}
			return t;
		}

		public static Base32Integer ToBase32(this int value)
		{
			return new Base32Integer(value);
		}

		public static string ToFormatString<T>(this IEnumerable<T> list, string format, string seperator, string lastSeperator)
		{
			string str = (
				from i  in list
				select format.FormatWith(i, new object[0])).ToString<string>(seperator, lastSeperator);
			return str;
		}

		public static string ToFormatString<T>(this IEnumerable<T> list, string format, string seperator)
		{
			string str = (
				from i in list
				select format.FormatWith(i, new object[0])).ToString<string>(seperator);
			return str;
		}

		public static string ToFriendlyDateString(this DateTime Date)
		{
			string str = "";
			if (Date.Date == DateTime.Today)
			{
				str = "Today";
			}
			else if (!(Date.Date == DateTime.Today.AddDays(-1)))
			{
				str = (!(Date.Date > DateTime.Today.AddDays(-6)) ? Date.ToString("MMMM dd, yyyy") : Date.ToString("dddd").ToString());
			}
			else
			{
				str = "Yesterday";
			}
			str = string.Concat(str, " @ ", Date.ToString("t").ToLower());
			return str;
		}

		public static string ToFullMessage(this Exception ex)
		{
			return ex.ToFullMessage(null, false, false, false);
		}

		public static string ToFullMessage(this Exception error, string additionalMessage, bool includeStackTrace, bool includeSource, bool includeData)
		{
			if (error == null)
			{
				throw new NullReferenceException();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLineIf(additionalMessage, additionalMessage.HasValue());
			while (error != null)
			{
				stringBuilder.AppendLine(error.Message);
				if ((!includeSource ? false : error.Source.HasValue()))
				{
					stringBuilder.AppendLine("Source:");
					stringBuilder.AppendLine(error.Source);
				}
				if ((!includeStackTrace ? false : error.StackTrace.HasValue()))
				{
					stringBuilder.AppendLine("StackTrace:");
					stringBuilder.AppendLine(error.GetBaseException().StackTrace);
				}
				if ((!includeData ? false : error.Data != null))
				{
					foreach (object datum in error.Data)
					{
						object[] str = new object[] { datum, datum.ToString() };
						stringBuilder.AddFormattedLine("Data[{0}]: {1}", str);
					}
				}
				if (error is ReflectionTypeLoadException)
				{
					Exception[] loaderExceptions = (error as ReflectionTypeLoadException).LoaderExceptions;
					for (int i = 0; i < (int)loaderExceptions.Length; i++)
					{
						Exception exception = loaderExceptions[i];
						stringBuilder.AppendLine(string.Concat("Type load exception: ", exception.ToFullMessage()));
					}
				}
				error = error.InnerException;
				if (error != null)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("Base issue: ");
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToInformalMoneyString(this double value)
		{
			string shortMoneyString;
			string[] strArrays = new string[] { "k", "m", "bn" };
			int length = (int)strArrays.Length;
			while (true)
			{
				if (length > 0)
				{
					int num = 2 + 3 * (length - 1);
					double num1 = Math.Pow(10, (double)num);
					if ((value % num1 != 0 ? true : value / num1 <= 10))
					{
						length--;
					}
					else
					{
						value = value / (num1 * 10);
						if (value <= 1000)
						{
							char chr = value.ToShortMoneyString()[0];
							shortMoneyString = string.Concat(chr.ToString(), value, strArrays[length - 1]);
							break;
						}
						else
						{
							shortMoneyString = string.Concat(value.ToShortMoneyString(), strArrays[length - 1]);
							break;
						}
					}
				}
				else
				{
					shortMoneyString = value.ToShortMoneyString();
					break;
				}
			}
			return shortMoneyString;
		}

		public static string ToNaturalTime(this TimeSpan period)
		{
			return period.ToNaturalTime(true);
		}

		public static string ToNaturalTime(this TimeSpan period, bool longForm)
		{
			return period.ToNaturalTime(2, longForm);
		}

		public static string ToNaturalTime(this TimeSpan period, int precisionParts)
		{
			return period.ToNaturalTime(precisionParts, true);
		}

		public static string ToNaturalTime(this TimeSpan period, int precisionParts, bool longForm)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "year", "y" },
				{ "month", "M" },
				{ "week", "w" },
				{ "day", "d" },
				{ "hour", "h" },
				{ "minute", "m" },
				{ "second", "s" },
				{ " and ", " " },
				{ ", ", " " }
			};
			Dictionary<string, string> strs1 = strs;
			Func<string, string> func = (string k) => (longForm ? k : strs1[k]);
			Dictionary<string, double> strs2 = new Dictionary<string, double>();
			if (period.TotalDays >= 365)
			{
				int num = (int)Math.Floor(period.TotalDays / 365);
				strs2.Add(func("year"), (double)num);
				period = period - TimeSpan.FromDays((double)(365 * num));
			}
			if (period.TotalDays >= 30)
			{
				int num1 = (int)Math.Floor(period.TotalDays / 30);
				strs2.Add(func("month"), (double)num1);
				period = period - TimeSpan.FromDays((double)(30 * num1));
			}
			if (period.TotalDays >= 7)
			{
				int num2 = (int)Math.Floor(period.TotalDays / 7);
				strs2.Add(func("week"), (double)num2);
				period = period - TimeSpan.FromDays((double)(7 * num2));
			}
			if (period.TotalDays >= 1)
			{
				strs2.Add(func("day"), (double)period.Days);
				period = period - TimeSpan.FromDays((double)period.Days);
			}
			if ((period.TotalHours < 1 ? false : period.Hours > 0))
			{
				strs2.Add(func("hour"), (double)period.Hours);
				period = period.Subtract(TimeSpan.FromHours((double)period.Hours));
			}
			if ((period.TotalMinutes < 1 ? false : period.Minutes > 0))
			{
				strs2.Add(func("minute"), (double)period.Minutes);
				period = period.Subtract(TimeSpan.FromMinutes((double)period.Minutes));
			}
			if (!(period.TotalSeconds < 1 ? true : period.Seconds <= 0))
			{
				strs2.Add(func("second"), (double)period.Seconds);
				period = period.Subtract(TimeSpan.FromSeconds((double)period.Seconds));
			}
			else if (period.TotalSeconds > 0)
			{
				strs2.Add(func("second"), period.TotalSeconds.Round(3));
				period = TimeSpan.Zero;
			}
			IEnumerable<KeyValuePair<string, double>> keyValuePairs = strs2.Take<KeyValuePair<string, double>>(precisionParts);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, double> keyValuePair in keyValuePairs)
			{
				stringBuilder.Append(keyValuePair.Value);
				if (longForm)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(keyValuePair.Key);
				if ((keyValuePair.Value <= 1 ? false : longForm))
				{
					stringBuilder.Append("s");
				}
				if (keyValuePairs.IndexOf<KeyValuePair<string, double>>(keyValuePair) == keyValuePairs.Count<KeyValuePair<string, double>>() - 2)
				{
					stringBuilder.Append(func(" and "));
				}
				else if (keyValuePairs.IndexOf<KeyValuePair<string, double>>(keyValuePair) < keyValuePairs.Count<KeyValuePair<string, double>>() - 2)
				{
					stringBuilder.Append(func(", "));
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToShortMoneyString(this double value)
		{
			string str = "{0:c}".FormatWith(value, new object[0]).TrimEnd(".00");
			return str;
		}

		public static string ToShortMoneyString(this double? value)
		{
			string str;
			str = (!value.HasValue ? string.Empty : value.ToShortMoneyString());
			return str;
		}

		public static string ToString(this IEnumerable list, string seperator)
		{
			return list.Cast<object>().ToString<object>(seperator);
		}

		public static string ToString<T>(this IEnumerable<T> list, string seperator)
		{
			return list.ToString<T>(seperator, seperator);
		}

		public static string ToString<T>(this IEnumerable<T> list, string seperator, string lastSeperator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			object[] array = list.Cast<object>().ToArray<object>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				object obj = array[i];
				if (obj != null)
				{
					stringBuilder.Append(obj.ToString());
				}
				else
				{
					stringBuilder.Append("{NULL}");
				}
				if (i < (int)array.Length - 2)
				{
					stringBuilder.Append(seperator);
				}
				if (i == (int)array.Length - 2)
				{
					stringBuilder.Append(lastSeperator);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToString(this int? value, string format)
		{
			string str = string.Concat("{0:", format, "}").FormatWith(value, new object[0]);
			return str;
		}

		public static string ToString(this double? value, string format)
		{
			string str = string.Concat("{0:", format, "}").FormatWith(value, new object[0]);
			return str;
		}

		public static string ToString(this DateTime? value, string format)
		{
			string str = string.Concat("{0:", format, "}").FormatWith(value, new object[0]);
			return str;
		}

		public static string ToTimeDifferenceString(this DateTime date)
		{
			return date.ToTimeDifferenceString(true);
		}

		public static string ToTimeDifferenceString(this DateTime date, bool longForm)
		{
			return date.ToTimeDifferenceString(2, longForm);
		}

		public static string ToTimeDifferenceString(this DateTime date, int precisionParts)
		{
			return date.ToTimeDifferenceString(precisionParts, true);
		}

		public static string ToTimeDifferenceString(this DateTime date, int precisionParts, bool longForm)
		{
			string str;
			DateTime now = DateTime.Now;
			if (!(now == date))
			{
				str = (!(now > date) ? string.Concat(date.Subtract(now).ToNaturalTime(precisionParts, longForm), " later") : string.Concat(now.Subtract(date).ToNaturalTime(precisionParts, longForm), " ago"));
			}
			else
			{
				str = (!longForm ? "Now" : "Just now");
			}
			return str;
		}

		public static Nullable<T> TryParseAs<T>(this string text)
		where T : struct
		{
			Nullable<T> nullable;
			Nullable<T> nullable1;
			if (text.IsEmpty())
			{
				nullable1 = null;
				nullable = nullable1;
			}
			else if (typeof(T) == typeof(Guid))
			{
				if (!text.StartsWith("-"))
				{
					try
					{
						nullable = new Nullable<T>((T)(object)(new Guid(text)));
					}
					catch
					{
						nullable1 = null;
						nullable = nullable1;
					}
				}
				else
				{
					nullable = new Nullable<T>(default(T));
				}
			}
			else if (typeof(T) != typeof(ShortGuid))
			{
				try
				{
					nullable = new Nullable<T>((T)Convert.ChangeType(text, typeof(T)));
				}
				catch
				{
					nullable1 = null;
					nullable = nullable1;
				}
			}
			else
			{
				try
				{
					nullable = new Nullable<T>((T)(object)ShortGuid.Parse(text));
				}
				catch
				{
					nullable1 = null;
					nullable = nullable1;
				}
			}
			return nullable;
		}

		public static void WrapIn(this StringBuilder r, string left, string right)
		{
			r.Insert(0, left);
			r.Append(right);
		}

		public static void WrapInLines(this StringBuilder r, string left, string right)
		{
			r.Insert(0, string.Concat(left, Environment.NewLine));
			r.Append(string.Concat(Environment.NewLine, right));
		}
	}
}