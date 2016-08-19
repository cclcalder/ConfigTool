using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Extensions.System.IO
{
	public class PathBuilder
	{
		private List<string> Parts;

		public PathBuilder(params string[] sections)
		{
			this.Parts = new List<string>(sections ?? new string[0]);
		}

		public void Add(string part)
		{
			this.Parts.Add(part);
		}

		public void Insert(int index, string part)
		{
			this.Parts.Insert(index, part);
		}

		public static implicit operator String(PathBuilder b)
		{
			return b.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string str in 
				from p in this.Parts
				where p.HasValue()
				select p.Trim().Replace("/", "\\"))
			{
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(str);
				}
				else if (!str.StartsWith("\\"))
				{
					stringBuilder.Append(str);
				}
				else
				{
					char[] chrArray = new char[] { '\\' };
					stringBuilder.Append(str.TrimStart(chrArray));
				}
				if (!str.EndsWith("\\"))
				{
					stringBuilder.Append("\\");
				}
			}
			return stringBuilder.ToString();
		}
	}
}