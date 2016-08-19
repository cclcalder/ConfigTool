using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Xml.Linq
{
	public static class CommonExtensions
	{
		public static XElement GetElement(this XContainer parent, string path)
		{
			return parent.GetNode(path) as XElement;
		}

		public static XObject GetNode(this XContainer parent, string path)
		{
			XObject xObject;
			if (path.IsEmpty())
			{
				throw new ArgumentNullException("path");
			}
			XContainer xContainer = parent;
			string[] strArrays = path.Split(new char[] { '/' });
			int num = 0;
			while (true)
			{
				if (num < (int)strArrays.Length)
				{
					string str = strArrays[num];
					if (!str.StartsWith("@"))
					{
						XElement xElement = xContainer.Element(str);
						if (xElement == null)
						{
							xContainer = xContainer.Elements().FirstOrDefault<XElement>((XElement e) => (e.Name == null ? false : e.Name.LocalName == str));
							if (xContainer == null)
							{
								xObject = null;
								break;
							}
						}
						else
						{
							xContainer = xElement;
						}
						num++;
					}
					else
					{
						XElement xElement1 = xContainer as XElement;
						if (xElement1 != null)
						{
							string str1 = str.Substring(1);
							XAttribute xAttribute = xElement1.Attribute(str1);
							if (xAttribute == null)
							{
								xObject = xElement1.Attributes().FirstOrDefault<XAttribute>((XAttribute a) => (a.Name == null ? false : a.Name.LocalName == str1));
								break;
							}
							else
							{
								xObject = xAttribute;
								break;
							}
						}
						else
						{
							xObject = null;
							break;
						}
					}
				}
				else
				{
					xObject = xContainer;
					break;
				}
			}
			return xObject;
		}

		public static T GetValue<T>(this XContainer parent, string path)
		{
			T t;
			string value = null;
			XObject node = parent.GetNode(path);
			if (node is XElement)
			{
				value = (node as XElement).Value;
			}
			else if (node is XAttribute)
			{
				value = (node as XAttribute).Value;
			}
			else if (node != null)
			{
				object[] objArray = new object[] { "The provided path (", path, ") points to an invalid Xml node (", node.GetType(), ")." };
				throw new Exception(string.Concat(objArray));
			}
			if (!(value == null ? false : !(value == string.Empty)))
			{
				t = default(T);
			}
			else if (typeof(T) != typeof(string))
			{
				try
				{
					t = (T)Convert.ChangeType(value, typeof(T));
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					string[] fullName = new string[] { "Could not convert the value of \"", value, "\" to type ", typeof(T).FullName, "." };
					throw new Exception(string.Concat(fullName), exception);
				}
			}
			else
			{
                t = (T)Convert.ChangeType(value, typeof(T));
			}
			return t;
		}
	}
}