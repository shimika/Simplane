using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simplane {
	public partial class MainWindow : Window {
		private void Window_DragEnter(object sender, DragEventArgs e) {
			if (!(e.Data is DataObject) || !(e.Data as DataObject).ContainsFileDropList()) { return; }

			gridUpload.Opacity = 1;
			string file = GetFirstFile(e.Data as DataObject);

			if (file != "") {
				textUpload.Text = String.Format("Upload: {0}", Path.GetFileName(file));
			}
			else {
				textUpload.Text = "Image file not found";
			}
		}
		private void Window_DragLeave(object sender, DragEventArgs e) {
			gridUpload.Opacity = 0;
		}

		private void Window_Drop(object sender, DragEventArgs e) {
			gridUpload.Opacity = 0;

			string file = GetFirstFile(e.Data as DataObject);
			if (file != "") {
				UploadFile(file);
			}
		}

		string[] extCollect = new string[] { "jpg", "jpeg", "png", "gif", "bmp", "zip", "mp3", "rar" };
		private string GetFirstFile(DataObject data) {
			foreach (string filePath in data.GetFileDropList()) {
				if (File.Exists(filePath)) {
					string ext = Path.GetExtension(filePath).Replace(".", "");
					if (extCollect.Contains(ext)) {
						return filePath;
					}
				}
			}

			return "";
		}
	}
}
