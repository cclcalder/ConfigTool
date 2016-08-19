using System;
using System.Collections;
using System.Reflection;

namespace System.Linq
{
	internal class PropertyComparer : IComparer
	{
		private PropertyInfo property;

		public PropertyComparer(PropertyInfo p)
		{
			this.property = p;
		}

		public int Compare(object x, object y)
		{
			int num = Comparer.Default.Compare(this.property.GetValue(x, null), this.property.GetValue(y, null));
			return num;
		}

		public K ExtractValue<T, K>(T item)
		{
			return (K)this.property.GetValue(item, null);
		}
	}
}