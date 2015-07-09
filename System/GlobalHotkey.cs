using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Simplane {
	public partial class MainWindow : Window {
		// Global hotkey

		HwndSource HWndSource;
		private int Showing, Desktop, Area, Save;

		private void SetHotkeyEvent() {
			WindowInteropHelper wih = new WindowInteropHelper(this);
			HWndSource = HwndSource.FromHwnd(wih.Handle);
			HWndSource.AddHook(MainWindowProc);

			Showing = WinAPI.GlobalAddAtom("Showing");
			Desktop = WinAPI.GlobalAddAtom("Desktop");
			Area = WinAPI.GlobalAddAtom("Area");
			Save = WinAPI.GlobalAddAtom("Save");
		}

		private void ToggleHotKeyMode(bool isON) {
			WindowInteropHelper wih = new WindowInteropHelper(this);

			if (isON) {
				WinAPI.RegisterHotKey(wih.Handle, Showing, 6, WinAPI.VK_KEY_2);
				WinAPI.RegisterHotKey(wih.Handle, Desktop, 6, WinAPI.VK_KEY_3);
				WinAPI.RegisterHotKey(wih.Handle, Area, 6, WinAPI.VK_KEY_4);
				WinAPI.RegisterHotKey(wih.Handle, Save, 6, WinAPI.VK_KEY_1);
			}
			else {
				WinAPI.UnregisterHotKey(wih.Handle, Showing);
				WinAPI.UnregisterHotKey(wih.Handle, Desktop);
				WinAPI.UnregisterHotKey(wih.Handle, Area);
				WinAPI.UnregisterHotKey(wih.Handle, Save);
			}
		}

		int count = 0;
		private IntPtr MainWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if (msg == WinAPI.WM_HOTKEY) {
				if (wParam.ToString() == Showing.ToString()) {
					UploadFile(ScreenCapture.UploadActivate());
				}
				else if (wParam.ToString() == Desktop.ToString()) {
					UploadFile(ScreenCapture.UploadDesktop());
				}
				else if (wParam.ToString() == Area.ToString()) {
					capWindow.Reset(true);
				}
				else if (wParam.ToString() == Save.ToString()) {
					capWindow.Reset(false);
				}
			}

			return IntPtr.Zero;
		}
	}
}
