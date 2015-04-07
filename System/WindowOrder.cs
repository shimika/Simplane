using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Simplane {
	class WindowOrder {
		[DllImport("user32")]
		static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		public static void FocusZOrderWindow() {
			IntPtr nextPtr = GetDesktopWindow();
			int zOrder = int.MaxValue;
			foreach (Process p in Process.GetProcesses(".")) {
				try {
					if (p.MainWindowTitle.Length > 0) {
						if (p.MainWindowHandle == Process.GetCurrentProcess().MainWindowHandle) { continue; }

						int z = GetZOrder(p.MainWindowHandle);
						if (zOrder > z) {
							zOrder = z;
							nextPtr = p.MainWindowHandle;
						}
					}
				} catch { }
			}

			SetForegroundWindow(nextPtr);
		}

		private static int GetZOrder(IntPtr handle) {
			int z = 0;
			do {
				z++;
				handle = GetWindow(handle, 3);
			} while (handle != IntPtr.Zero);

			return z;
		}
	}
}
