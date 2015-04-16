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
		private int Activate, Desktop, Area, Save;
		CaptureWindow window = new CaptureWindow();

		private void SetHotkeyEvent() {
			WindowInteropHelper wih = new WindowInteropHelper(this);
			HWndSource = HwndSource.FromHwnd(wih.Handle);
			HWndSource.AddHook(MainWindowProc);

			Activate = WinAPI.GlobalAddAtom("Activate");
			Desktop = WinAPI.GlobalAddAtom("Desktop");
			Area = WinAPI.GlobalAddAtom("Area");
			Save = WinAPI.GlobalAddAtom("Save");

			window.Response += CaptureWindow_Response;
			window.Show();
		}

		private void ToggleHotKeyMode(bool isON) {
			WindowInteropHelper wih = new WindowInteropHelper(this);

			if (isON) {
				WinAPI.RegisterHotKey(wih.Handle, Activate, 6, WinAPI.VK_KEY_2);
				WinAPI.RegisterHotKey(wih.Handle, Desktop, 6, WinAPI.VK_KEY_3);
				WinAPI.RegisterHotKey(wih.Handle, Area, 6, WinAPI.VK_KEY_4);
				WinAPI.RegisterHotKey(wih.Handle, Save, 6, WinAPI.VK_KEY_1);
			}
			else {
				WinAPI.UnregisterHotKey(wih.Handle, Activate);
				WinAPI.UnregisterHotKey(wih.Handle, Desktop);
				WinAPI.UnregisterHotKey(wih.Handle, Area);
				WinAPI.UnregisterHotKey(wih.Handle, Save);
			}
		}

		int count = 0;
		private IntPtr MainWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if (msg == WinAPI.WM_HOTKEY) {
				if (wParam.ToString() == Activate.ToString()) {
					UploadFile(ScreenCapture.UploadActivate());
				}
				else if (wParam.ToString() == Desktop.ToString()) {
					UploadFile(ScreenCapture.UploadDesktop());
				}
				else if (wParam.ToString() == Area.ToString()) {
					window.Reset(true);
				}
				else if (wParam.ToString() == Save.ToString()) {
					window.Reset(false);
				}
			}

			return IntPtr.Zero;
		}

		private void CaptureWindow_Response(object sender, AreaEventArgs e) {
			if (e.Left < 0 || e.Top < 0) { return; }

			if (e.Upload) {
				string file = ScreenCapture.SaveArea(e.Left, e.Top, e.Right, e.Bottom);
				UploadFile(file);
			}
			else {
				string path = ScreenCapture.SaveArea(e.Left, e.Top, e.Right, e.Bottom);
				string file = Path.GetFileName(path);

				if (File.Exists(path)) {
					string to = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), file);
					File.Copy(path, to);

					notiWindow.Alert("Success", file, "Local", Brushes.MediumSeaGreen, 10);
				}
				else {
					ShowNotify("Save Failed", "Oops");
				}
			}
		}
	}
}
