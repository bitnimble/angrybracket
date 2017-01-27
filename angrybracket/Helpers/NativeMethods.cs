using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class NativeMethods
	{
		[DllImport("user32.dll", EntryPoint = "SendMessageW")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public static extern int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr CreateFile(string fileName, UInt32 desiredAccess, UInt32 shareMode,
			IntPtr securityAttributes, UInt32 creationDisposition, UInt32 flagsAndAttributes, IntPtr templateFile);

		[DllImport("kernel32.dll", EntryPoint = "CreateFileW")]
		public static extern IntPtr CreateFileW([MarshalAs(UnmanagedType.LPWStr)]string fileName,
			UInt32 desiredAccess, UInt32 shareMode, IntPtr securityAttributes, UInt32 creationDisposition, UInt32 flagsAndAttributes, IntPtr templateFile);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool GetVolumeInformationByHandleW(IntPtr hDisk, StringBuilder volumeNameBuffer, int volumeNameSize,
			ref uint volumeSerialNumber, ref uint maximumComponentLength, ref uint fileSystemFlags,
			StringBuilder fileSystemNameBuffer, int nFileSystemNameSize);
		
		public enum ThreadAccess : int
		{
			TERMINATE = (0x0001),
			SUSPEND_RESUME = (0x0002),
			GET_CONTEXT = (0x0008),
			SET_CONTEXT = (0x0010),
			SET_INFORMATION = (0x0020),
			QUERY_INFORMATION = (0x0040),
			SET_THREAD_TOKEN = (0x0080),
			IMPERSONATE = (0x0100),
			DIRECT_IMPERSONATION = (0x0200)
		}

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
		[DllImport("kernel32.dll")]
		public static extern uint SuspendThread(IntPtr hThread);
		[DllImport("kernel32.dll")]
		public static extern int ResumeThread(IntPtr hThread);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[Flags]
		public enum Win32FileAccess : uint
		{
			GenericRead = 0x80000000,
			GenericWrite = 0x40000000,
			GenericExecute = 0x20000000,
			GenericAll = 0x10000000
		}

		[Flags]
		public enum Win32FileShare : uint
		{
			FileShareNone = 0x0,
			FileShareDelete = 0x4,
			FileShareWrite = 0x2,
			FileShareRead = 0x1
		}

		public static Win32FileAccess GetWin32FileAccess(FileAccess access)
		{
			switch (access)
			{
				case FileAccess.Read:
					return Win32FileAccess.GenericRead;
				case FileAccess.ReadWrite:
					return Win32FileAccess.GenericRead | Win32FileAccess.GenericWrite;
				case FileAccess.Write:
					return Win32FileAccess.GenericWrite;
			}
			return Win32FileAccess.GenericAll;
		}
		public static Win32FileShare GetWin32FileShare(FileShare share)
		{
			switch (share)
			{
				case FileShare.Delete:
					return Win32FileShare.FileShareDelete;
				//case FileShare.Inheritable:
				//	break;
				case FileShare.Read:
					return Win32FileShare.FileShareRead;
				case FileShare.ReadWrite:
					return Win32FileShare.FileShareRead | Win32FileShare.FileShareWrite;
				case FileShare.Write:
					return Win32FileShare.FileShareWrite;
				case FileShare.None:
				default:
					return Win32FileShare.FileShareNone;
			}
		}
	}
}
