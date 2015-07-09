using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Simplane {
	public partial class MainWindow : Window {
		CaptureWindow capWindow;
		NotiWindow notiWindow;

		private void InitCaptureManager() {
			capWindow = new CaptureWindow();
			notiWindow = new NotiWindow(this);

			capWindow.Response += CaptureWindow_Response;
			capWindow.Show();
			notiWindow.Show();
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
