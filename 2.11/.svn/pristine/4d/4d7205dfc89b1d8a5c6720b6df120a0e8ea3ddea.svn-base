using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Extensions.System.IO
{
	public class SevenZipper
	{
		private const string EXE_FILE = "C:\\Program Files\\7-Zip\\7z.exe";

		public SevenZipper()
		{
		}

		public static Process Zip(string zipFileName, params string[] foldersToCompress)
		{
			return SevenZipper.Zip(zipFileName, null, foldersToCompress);
		}

		public static Process Zip(string zipFileName, int? splitSize, params string[] foldersToCompress)
		{
			if (zipFileName.IsEmpty())
			{
				throw new ArgumentNullException("zipFileName");
			}
			if ((foldersToCompress == null ? true : (int)foldersToCompress.Length == 0))
			{
				throw new ArgumentNullException("foldersToCompress");
			}
			if ((!splitSize.HasValue ? false : splitSize.Value < 1))
			{
				throw new ArgumentException("The minimum possible split size for 7 Zipper is 1KB.");
			}
			if (!File.Exists("C:\\Program Files\\7-Zip\\7z.exe"))
			{
				throw new Exception("7 Zip is not installed. Could not find the file at C:\\Program Files\\7-Zip\\7z.exe");
			}
			ProcessStartInfo processStartInfo = new ProcessStartInfo("C:\\Program Files\\7-Zip\\7z.exe", "a -r ");
			if (splitSize.HasValue)
			{
				ProcessStartInfo processStartInfo1 = processStartInfo;
				object arguments = processStartInfo1.Arguments;
				object[] objArray = new object[] { arguments, " -v\"", splitSize, "k\" " };
				processStartInfo1.Arguments = string.Concat(objArray);
			}
			ProcessStartInfo processStartInfo2 = processStartInfo;
			processStartInfo2.Arguments = string.Concat(processStartInfo2.Arguments, "\"", zipFileName, "\" ");
			ProcessStartInfo processStartInfo3 = processStartInfo;
			processStartInfo3.Arguments = string.Concat(processStartInfo3.Arguments, (
				from pt in foldersToCompress
				select string.Concat("\"", pt, "\"")).ToString<string>(" "));
			return Process.Start(processStartInfo);
		}
	}
}