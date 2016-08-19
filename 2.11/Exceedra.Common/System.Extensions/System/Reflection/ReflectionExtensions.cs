using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Reflection
{
	public static class ReflectionExtensions
	{
		private static Dictionary<Assembly, Dictionary<Type, IEnumerable<Type>>> SubTypesCache;

		static ReflectionExtensions()
		{
			ReflectionExtensions.SubTypesCache = new Dictionary<Assembly, Dictionary<Type, IEnumerable<Type>>>();
		}

		public static T Cache<T>(this MethodBase method, object[] arguments, ReflectionExtensions.Method<T> methodBody)
		where T : class
		{
			return method.Cache<T>(null, arguments, methodBody);
		}

		public static T Cache<T>(this MethodBase method, object instance, object[] arguments, ReflectionExtensions.Method<T> methodBody)
		where T : class
		{
			T data;
			string str = string.Concat(method.DeclaringType.GUID, ":", method.Name);
			if (instance != null)
			{
				str = string.Concat(str, instance.GetHashCode(), ":");
			}
			if (arguments != null)
			{
				arguments.Do<object>((object arg) => str = string.Concat(str, arg.GetHashCode(), ":"));
			}
			if (CallContext.GetData(str) != null)
			{
				data = (T)(CallContext.GetData(str) as T);
			}
			else
			{
				T t = methodBody();
				CallContext.SetData(str, t);
				data = t;
			}
			return data;
		}

		public static IEnumerable<Type> GetSubTypes(this Assembly assembly, Type baseType)
		{
			Type[] array;
			if (!ReflectionExtensions.SubTypesCache.ContainsKey(assembly))
			{
				lock (ReflectionExtensions.SubTypesCache)
				{
					if (!ReflectionExtensions.SubTypesCache.ContainsKey(assembly))
					{
						ReflectionExtensions.SubTypesCache.Add(assembly, new Dictionary<Type, IEnumerable<Type>>());
					}
				}
			}
			Dictionary<Type, IEnumerable<Type>> item = ReflectionExtensions.SubTypesCache[assembly];
			if (!item.ContainsKey(baseType))
			{
				lock (assembly)
				{
					if (!item.ContainsKey(baseType))
					{
						try
						{
							array = (
								from t in (IEnumerable<Type>)assembly.GetTypes()
								where t.BaseType == baseType
								select t).ToArray<Type>();
						}
						catch (ReflectionTypeLoadException reflectionTypeLoadException1)
						{
							ReflectionTypeLoadException reflectionTypeLoadException = reflectionTypeLoadException1;
							string fullName = assembly.FullName;
							object[] str = new object[] { (
								from e in (IEnumerable<Exception>)reflectionTypeLoadException.LoaderExceptions
								select e.Message).Distinct<string>().ToString<string>(" | ") };
							throw new Exception("Could not load the types of the assembly '{0}'. Type-load exceptions: {1}".FormatWith(fullName, str));
						}
						item.Add(baseType, array);
					}
				}
			}
			return item[baseType];
		}

        public static void InvokeMethodOnParentPage(DependencyObject currentPage, string methodName, object[] methodParameters)
        {
            var parentPage = GetParentPage(currentPage);

            var method = parentPage.GetType().GetMethod(methodName);
            if (method == null)
            {
                parentPage = GetParentPage(parentPage);
                method = parentPage.GetType().GetMethod(methodName);
            }

            method.Invoke(parentPage, methodParameters);
        }

        public static DependencyObject GetParentPage(DependencyObject currentPage)
        {
            var parentPage = VisualTreeHelper.GetParent(currentPage);
            while (!(parentPage is Page))
                parentPage = VisualTreeHelper.GetParent(parentPage);

            return parentPage;
        }

        public delegate T Method<T>();
	}
}