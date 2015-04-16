using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simplane {
	public partial class MainWindow : Window {
		private void UploadFile(string path) {
			if (!File.Exists(path)) {
				Log.WriteLog("File not found");
				Log.WriteLog(path);

				ShowNotify("File not found", "Error", System.Windows.Forms.ToolTipIcon.Error);
				return;
			}

			ChangeTray(true);

			WebClient client = new System.Net.WebClient() { Proxy = null };
			client.Headers.Add("Content-Type", "binary/octet-stream");
			client.Headers.Add("Accept-Token", Setting.Token);
			client.UploadFileCompleted += client_UploadFileCompleted;
			client.UploadFileAsync(new UriBuilder(String.Format("http://{0}/upload.php", Setting.Server)).Uri,
				"POST", path);
		}

		private void client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e) {
			ChangeTray(false);
			//this.Activate();

			if (e.Error != null) {
				ShowNotify(e.Error.Message, "Error", System.Windows.Forms.ToolTipIcon.Error);
			}
			else {
				string s = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
				string url = String.Format("http://{0}/{1}", Setting.Server, s);

				ShowNotify(url, "Success", System.Windows.Forms.ToolTipIcon.None, true);

				AddLink(url);

				for (int i = 0; i < 50; i++) {
					try {
						Clipboard.SetDataObject(url);
						return;
					}
					catch { }
				}
			}
		}
	}
}
