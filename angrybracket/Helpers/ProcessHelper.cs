using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class ProcessHelper
	{

		public static void Suspend(this Process process)
		{
			for (int i = 0; i < process.Threads.Count; i++)
			{
				ProcessThread pThread = process.Threads[i];
				var openedThread = NativeMethods.OpenThread(NativeMethods.ThreadAccess.SUSPEND_RESUME, false, (uint)pThread.Id);
				if (openedThread == IntPtr.Zero) continue;
				NativeMethods.SuspendThread(openedThread);
			}
		}
		public static void Resume(this Process process)
		{
			for (int i = 0; i < process.Threads.Count; i++)
			{
				ProcessThread pThread = process.Threads[i];
				var openedThread = NativeMethods.OpenThread(NativeMethods.ThreadAccess.SUSPEND_RESUME, false, (uint)pThread.Id);
				if (openedThread == IntPtr.Zero) continue;
				NativeMethods.ResumeThread(openedThread);
			}
		}
		public static Process GetForegroundProcess()
		{
			return GetProcessByHandle(NativeMethods.GetForegroundWindow());
		}

		public static Process GetProcessByHandle(IntPtr handle)
		{
			uint pid;
			NativeMethods.GetWindowThreadProcessId(handle, out pid);
			return Process.GetProcessById((int)pid);
		}
	}
}
