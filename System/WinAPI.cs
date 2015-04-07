using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Simplane {
	internal static class WinAPI {
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("kernel32", SetLastError = true)]
		public static extern short GlobalAddAtom(string lpString);

		[DllImport("kernel32", SetLastError = true)]
		public static extern short GlobalDeleteAtom(short nAtom);

		public const int MOD_ALT = 1;
		public const int MOD_CONTROL = 2;
		public const int MOD_SHIFT = 4;
		public const int MOD_WIN = 8;

		public const uint VK_KEY_1 = 0x31;
		public const uint VK_KEY_2 = 0x32;
		public const uint VK_KEY_3 = 0x33;
		public const uint VK_KEY_4 = 0x34;

		public const int WM_HOTKEY = 0x312;
	}
}
