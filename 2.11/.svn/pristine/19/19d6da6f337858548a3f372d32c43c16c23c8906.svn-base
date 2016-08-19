using System.IO;
using System.Runtime.CompilerServices;

namespace System
{
	public static class IOExtensionMethods
	{
		public static string GetPath(this AppDomain applicationDomain, params string[] relativePathSections)
		{
			string baseDirectory = applicationDomain.BaseDirectory;
			string[] strArrays = relativePathSections;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (str.HasValue())
				{
					baseDirectory = Path.Combine(baseDirectory, str);
				}
			}
			return baseDirectory;
		}
	}
}