﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Simplane {
	public class AltTab {
		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hwnd, int index);
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
		#region Window styles
		[Flags]
		public enum ExtendedWindowStyles {
			// ...
			WS_EX_TOOLWINDOW = 0x00000080,
			// ...
		}

		public enum GetWindowLongFields {
			// ...
			GWL_EXSTYLE = (-20),
			// ...
		}

		public static void HideAltTab(Window wnd) {
			WindowInteropHelper wndHelper = new WindowInteropHelper(wnd);
			int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

			exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
			SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
		}

		public static void ShowAltTab(Window wnd) {
			WindowInteropHelper wndHelper = new WindowInteropHelper(wnd);
			int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

			exStyle = exStyle & (~(int)ExtendedWindowStyles.WS_EX_TOOLWINDOW);
			SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
		}


		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
			int error = 0;
			IntPtr result = IntPtr.Zero;
			// Win32 SetWindowLong doesn't clear error on success
			SetLastError(0);

			if (IntPtr.Size == 4) {
				// use SetWindowLong
				Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
				error = Marshal.GetLastWin32Error();
				result = new IntPtr(tempResult);
			} else {
				// use SetWindowLongPtr
				result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
				error = Marshal.GetLastWin32Error();
			}

			if ((result == IntPtr.Zero) && (error != 0)) {
				throw new System.ComponentModel.Win32Exception(error);
			}

			return result;
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

		private static int IntPtrToInt32(IntPtr intPtr) { return unchecked((int)intPtr.ToInt64()); }

		[DllImport("kernel32.dll", EntryPoint = "SetLastError")]
		public static extern void SetLastError(int dwErrorCode);
		#endregion
	}
}
