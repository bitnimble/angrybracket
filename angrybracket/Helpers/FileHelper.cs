using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class FileHelper
	{
		public static void MakeParentDirectories(string path)
		{
			var directory = Path.GetDirectoryName(path);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
		}

		/// <summary>
		/// Retrieves the depth of the specified file or directory. If the input is a directory, it must contain
		/// a trailing slash.
		/// </summary>
		/// <param name="path">Absolute path to the specified file or directory.</param>
		/// <returns></returns>
		public static int GetPathDepth(string path)
		{
			return path.Count(Path.DirectorySeparatorChar);
		}

		public static bool AreEqual(string file1, string file2)
		{
			FileInfo info1 = new FileInfo(file1), info2 = new FileInfo(file2);

			//Check both files exist...
			if (!info1.Exists && !info2.Exists)
				return true;
			else if (!info1.Exists || !info2.Exists)
				return false;

			//... and are the same size...
			if (info1.Length != info2.Length)
				return false;

			//... and have the same contents.
			FileStream fs1 = null, fs2 = null;
			try
			{
				fs1 = info1.OpenRead();
				fs2 = info2.OpenRead();

				//Read in chunks of 4KB.
				byte[] buff1 = new byte[4096], buff2 = new byte[4096];
				long position = 0, length = fs1.Length;
				while (position < length)
				{
					int readLen = (int)Math.Max(length - position, 4096);

					fs1.Read(buff1, 0, readLen);
					fs2.Read(buff2, 0, readLen);

					for (int i = 0; i < 4096; i++)
						if (buff1[i] != buff2[i])
							return false;

					position += readLen;
				}
			}
			finally
			{
				fs1.Close();
				fs2.Close();
			}
			return true;
		}

		/// <summary>
		/// Compares the textual contents of 2 files, returning true if they are equal.
		/// Ignores BOMs &amp; different new line conventions.
		/// Reads the files line by line, beware memory issues on infinite lines.
		/// </summary>
		public static bool AreEqualText(string file1, string file2)
		{
			int dummy;
			return AreEqualText(file1, file2, out dummy);
		}

		/// <summary>
		/// Compares the textual contents of 2 files, returning true if they are equal.
		/// Ignores BOMs &amp; different new line conventions.
		/// Reads the files line by line, beware memory issues on infinite lines.
		/// </summary>
		public static bool AreEqualText(string file1, string file2, out int lineDiff)
		{
			FileInfo info1 = new FileInfo(file1), info2 = new FileInfo(file2);
			lineDiff = 0;

			//Check both files exist...
			if (!info1.Exists && !info2.Exists)
				return true;
			else if (!info1.Exists || !info2.Exists)
				return false;

			StreamReader sr1 = null, sr2 = null;

			try
			{
				sr1 = new StreamReader(file1);
				sr2 = new StreamReader(file2);
				while (true)
				{
					lineDiff++;
					if (sr1.EndOfStream || sr2.EndOfStream)
						return (sr1.EndOfStream && sr2.EndOfStream);

					string l1 = sr1.ReadLine(), l2 = sr2.ReadLine();
					if (l1 != l2)
						return false;
				}
			}
			finally
			{
				if (sr1 != null)
					sr1.Close();
				if (sr2 != null)
					sr2.Close();
			}
		}

		/// <summary>
		/// Allows the filename to have a colon in it, to access an ADS
		/// </summary>
		public static Stream OpenFile(string filename, FileMode mode, FileAccess access, FileShare share)
		{
			var winAccess = (uint)NativeMethods.GetWin32FileAccess(access);
			var winShare = (uint)NativeMethods.GetWin32FileShare(share);
			IntPtr handle = NativeMethods.CreateFileW(filename, winAccess, winShare, IntPtr.Zero, (uint)mode, 0, IntPtr.Zero);

			return new FileStream(new Microsoft.Win32.SafeHandles.SafeFileHandle(handle, true), access);
		}

		public static async Task<string> ReadAllTextAsync(string path)
		{
			var sr = new StreamReader(path);
			var contents = await sr.ReadToEndAsync();
			sr.Close();
			return contents;
		}
	}
}
