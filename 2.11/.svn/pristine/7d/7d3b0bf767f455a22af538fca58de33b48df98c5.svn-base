using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
	public static class DataTableExtentions
	{
		public static Dictionary<T, DataRow> CastAsDictionary<T>(this DataTable data, object propertyMappings)
		where T : new()
		{
			if (propertyMappings != null)
			{
				PropertyInfo[] properties = propertyMappings.GetType().GetProperties();
				for (int i = 0; i < (int)properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					if (propertyInfo.PropertyType != typeof(string))
					{
						if (propertyInfo.PropertyType != typeof(Func<string, object>))
						{
							throw new ArgumentException("Unrecognized value for the property {0} of the specified propertyMappings".FormatWith(propertyInfo.PropertyType, new object[0]));
						}
						if (!propertyInfo.Name.StartsWith("set_"))
						{
							throw new ArgumentException("Property convertors must start with 'set_{property name}'");
						}
					}
				}
			}
			Dictionary<string, string> strs = DataTableExtentions.FindPropertyMappings(typeof(T), data.Columns, propertyMappings);
			Dictionary<string, Func<string, object>> strs1 = (propertyMappings == null ? new Dictionary<string, Func<string, object>>() : (
				from p in propertyMappings.GetType().GetProperties()
				where p.PropertyType == typeof(Func<string, object>)
				select p).ToDictionary<PropertyInfo, string, Func<string, object>>((PropertyInfo p) => p.Name.Substring(4), (PropertyInfo p) => (Func<string, object>)p.GetValue(propertyMappings)));
			Dictionary<T, DataRow> ts = new Dictionary<T, DataRow>();
			foreach (DataRow row in data.Rows)
			{
				ts.Add(DataTableExtentions.ParseObject<T>(row, strs, strs1), row);
			}
			return ts;
		}

		public static IEnumerable<T> CastTo<T>(this DataTable dataTable)
		where T : new()
		{
			return dataTable.CastTo<T>(null);
		}

		public static IEnumerable<T> CastTo<T>(this DataTable dataTable, object propertyMappings)
		where T : new()
		{
			IEnumerable<T> list = (
				from i in dataTable.CastAsDictionary<T>(propertyMappings)
				select i.Key).ToList<T>();
			return list;
		}

		private static Dictionary<string, string> FindPropertyMappings(Type targetType, DataColumnCollection columns, object declaredMappings)
		{
			PropertyInfo[] properties;
			int i;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			if (declaredMappings != null)
			{
				properties = declaredMappings.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
				for (i = 0; i < (int)properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					if (propertyInfo.CanWrite)
					{
						if (targetType.GetProperty(propertyInfo.Name) == null)
						{
							throw new Exception(string.Concat(targetType.FullName, " does not have a property named ", propertyInfo.Name));
						}
						string value = (string)propertyInfo.GetValue(declaredMappings);
						strs.Add(propertyInfo.Name, value);
					}
				}
			}
			string[] array = (
				from DataColumn c in columns
				select c.ColumnName).ToArray<string>();
			properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			for (i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo1 = properties[i];
				if (propertyInfo1.CanWrite)
				{
					if (!strs.ContainsKey(propertyInfo1.Name))
					{
						if (!array.Contains<string>(propertyInfo1.Name))
						{
							string str = array.FirstOrDefault<string>((string c) => c.ToLower() == propertyInfo1.Name.ToLower());
							if (str.HasValue())
							{
								strs.Add(propertyInfo1.Name, str);
							}
						}
						else
						{
							strs.Add(propertyInfo1.Name, propertyInfo1.Name);
						}
					}
				}
			}
			return strs;
		}

		private static T ParseObject<T>(DataRow dataContainer, Dictionary<string, string> propertyMappings, Dictionary<string, Func<string, object>> convertors)
		{
			object obj;
			T t = Activator.CreateInstance<T>();
			foreach (KeyValuePair<string, string> propertyMapping in propertyMappings)
			{
				PropertyInfo property = t.GetType().GetProperty(propertyMapping.Key);
				string str = dataContainer[propertyMapping.Value].Get<object, string>((object v) => v.ToString());
				try
				{
					obj = (!convertors.ContainsKey(propertyMapping.Key) ? Convert.ChangeType(str, property.PropertyType) : convertors[propertyMapping.Key](str));
					property.SetValue(t, obj);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					string key = propertyMapping.Key;
					object[] objArray = new object[] { str };
					throw new Exception("Could not set the value of the property '{0}' from the value of '{1}'.".FormatWith(key, objArray), exception);
				}
			}
			return t;
		}
	}
}