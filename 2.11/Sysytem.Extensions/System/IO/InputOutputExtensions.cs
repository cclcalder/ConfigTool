using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace System.IO
{
	public static class InputOutputExtensions
	{
		private const int MAXIMUM_ATTEMPTS = 3;

		private const int ATTEMPT_PAUSE = 50;

		public static void CopyTo(this DirectoryInfo source, string destination)
		{
			int i;
			if (!Directory.Exists(destination))
			{
				Directory.CreateDirectory(destination);
			}
			FileInfo[] files = source.GetFiles();
			for (i = 0; i < (int)files.Length; i++)
			{
				FileInfo fileInfo = files[i];
				fileInfo.CopyTo(Path.Combine(destination, fileInfo.Name));
			}
			DirectoryInfo[] directories = source.GetDirectories();
			for (i = 0; i < (int)directories.Length; i++)
			{
				DirectoryInfo directoryInfo = directories[i];
				directoryInfo.CopyTo(Path.Combine(destination, directoryInfo.Name));
			}
		}

		public static void CopyTo(this FileInfo file, FileInfo destinationPath)
		{
			destinationPath.WriteAllBytes(file.ReadAllBytes());
		}

		public static void Delete(this DirectoryInfo directory, bool recursive, bool harshly)
		{
			if (directory == null)
			{
				throw new ArgumentNullException("directory");
			}
			if (directory.Exists)
			{
				if ((!harshly ? false : !recursive))
				{
					throw new ArgumentException("For deleting a folder harshly, the recursive option should also be specified.");
				}
				if (harshly)
				{
					try
					{
						directory.Delete(true);
					}
					catch
					{
						InputOutputExtensions.HarshDelete(directory);
					}
				}
				else
				{
					directory.Delete(recursive);
				}
			}
		}

		public static void Delete(this FileInfo file, bool harshly)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (file.Exists)
			{
				if (harshly)
				{
					FileInfo fileInfo = file;
					InputOutputExtensions.TryHard(file, new Action(fileInfo.Delete), "The system cannot delete the file, even after several attempts. Path: {0}");
				}
				else
				{
					file.Delete();
				}
			}
		}

		public static IEnumerable<DirectoryInfo> GetDirectories(this DirectoryInfo parent, bool recursive)
		{
			IEnumerable<DirectoryInfo> directories;
			if (recursive)
			{
				List<DirectoryInfo> list = parent.GetDirectories().ToList<DirectoryInfo>();
				DirectoryInfo[] directoryInfoArray = parent.GetDirectories();
				for (int i = 0; i < (int)directoryInfoArray.Length; i++)
				{
					list.AddRange(directoryInfoArray[i].GetDirectories(true));
				}
				directories = list;
			}
			else
			{
				directories = parent.GetDirectories();
			}
			return directories;
		}

		public static string[] GetFiles(this DirectoryInfo folder, bool includeSubDirectories)
		{
			List<string> strs = new List<string>(
				from each in (IEnumerable<FileInfo>)folder.GetFiles()
				select each.FullName);
			if (includeSubDirectories)
			{
				DirectoryInfo[] directories = folder.GetDirectories();
				for (int i = 0; i < (int)directories.Length; i++)
				{
					strs.AddRange(directories[i].GetFiles(true));
				}
			}
			return strs.ToArray();
		}

		public static DirectoryInfo GetSubDirectory(this DirectoryInfo parent, string subdirectoryName)
		{
			DirectoryInfo directoryInfo = parent.GetDirectories().SingleOrDefault<DirectoryInfo>((DirectoryInfo d) => d.Name.ToString() == subdirectoryName);
			return directoryInfo;
		}

		private static void HarshDelete(DirectoryInfo directory)
		{
			LinqExtensions.ItemHandler<FileInfo> itemHandler = null;
			LinqExtensions.ItemHandler<DirectoryInfo> itemHandler1 = null;
			if (directory.Exists)
			{
				InputOutputExtensions.TryHard(directory, () => {
					FileInfo[] files = directory.GetFiles();
					if (itemHandler == null)
					{
						itemHandler = (FileInfo f) => f.Delete(true);
					}
					((IEnumerable<FileInfo>)files).Do<FileInfo>(itemHandler);
					DirectoryInfo[] directories = directory.GetDirectories();
					if (itemHandler1 == null)
					{
						itemHandler1 = (DirectoryInfo d) => InputOutputExtensions.HarshDelete(d);
					}
					((IEnumerable<DirectoryInfo>)directories).Do<DirectoryInfo>(itemHandler1);
					directory.Delete();
				}, "The system cannot delete the directory, even after several attempts. Directory: {0}");
			}
		}

		public static byte[] ReadAllBytes(this FileInfo file)
		{
			byte[] numArray = InputOutputExtensions.TryHard<byte[]>(file, () => File.ReadAllBytes(file.FullName), "The system cannot read the file: {0}");
			return numArray;
		}

		public static byte[] ReadAllBytes(this Stream stream)
		{
			byte[] numArray = new byte[(int) checked((IntPtr)stream.Length)];
			stream.Position = (long)0;
			stream.Read(numArray, 0, (int)numArray.Length);
			return numArray;
		}

		public static string ReadAllText(this FileInfo file)
		{
			string str = InputOutputExtensions.TryHard<string>(file, () => File.ReadAllText(file.FullName), "The system cannot read the file: {0}");
			return str;
		}

		private static T TryHard<T>(FileSystemInfo fileOrFolder, Func<T> action, string error)
		{
			T t = default(T);
			InputOutputExtensions.TryHard(fileOrFolder, () => t = action(), error);
			return t;
		}

		private static void TryHard(FileSystemInfo fileOrFolder, Action action, string error)
		{
			int num = 0;
			Exception exception = null;
		Label1:
			while (num <= 3)
			{
				try
				{
					action();
				}
				catch (Exception exception1)
				{
					exception = exception1;
					try
					{
						fileOrFolder.Attributes = FileAttributes.Normal;
					}
					catch
					{
					}
					num++;
					Thread.Sleep(50);
					goto Label1;
				}
				return;
			}
			throw new IOException(error.FormatWith(fileOrFolder.FullName, new object[0]), exception);
		}

		public static void WriteAllBytes(this FileInfo file, byte[] content)
		{
			if (!file.Directory.Exists)
			{
				file.Directory.Create();
			}
			InputOutputExtensions.TryHard(file, () => File.WriteAllBytes(file.FullName, content), "The system cannot write the specified content on the file: {0}");
		}

		public static void WriteAllText(this FileInfo file, string content)
		{
			file.WriteAllText(content, Encoding.GetEncoding(1252));
		}

		public static void WriteAllText(this FileInfo file, string content, Encoding encoding)
		{
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (!file.Directory.Exists)
			{
				file.Directory.Create();
			}
			file.WriteAllBytes(encoding.GetBytes(content));
		}

		public static bool WriteWhenDifferent(this FileInfo file, string newContent)
		{
			bool flag;
			if (file.Exists)
			{
				if (newContent == file.ReadAllText())
				{
					flag = false;
					return flag;
				}
			}
			file.WriteAllText(newContent);
			flag = true;
			return flag;
		}
	}
}