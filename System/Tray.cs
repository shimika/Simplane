using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Simplane {
	public partial class MainWindow : Window {
		public NotifyIcon tray = new NotifyIcon();
		ContextMenuStrip menu = new ContextMenuStrip();

		private void InitTray() {
			tray.Visible = true;

			tray.Icon = System.Drawing.Icon.FromHandle(Simplane.Properties.Resources.normal.Handle);
			tray.Text = "Simplane";

			ToolStripMenuItem ver = new ToolStripMenuItem(Version.NowVersion);
			ToolStripMenuItem recent = new ToolStripMenuItem("Recent");

			ToolStripMenuItem save = new ToolStripMenuItem("(Ctrl + Shift + 1) Save Area in Desktop", null, Function) { Tag = "save" };
			ToolStripMenuItem activate = new ToolStripMenuItem("(Ctrl + Shift + 2) Capture Activate Window", null, Function) { Tag = "activate" };
			ToolStripMenuItem desktop = new ToolStripMenuItem("(Ctrl + Shift + 3) Capture Desktop", null, Function) { Tag = "desktop" };
			ToolStripMenuItem area = new ToolStripMenuItem("(Ctrl + Shift + 4) Capture Area", null, Function) { Tag = "area" };

			ToolStripMenuItem open = new ToolStripMenuItem("Option");
			ToolStripMenuItem exit = new ToolStripMenuItem("Exit");

			ver.Enabled = false;
			recent.Enabled = false;
			open.Click += delegate(object sender, EventArgs e) { ShowOption(); };
			exit.Click += delegate(object sender, EventArgs e) { this.Close(); };

			menu.Items.Add(ver);
			menu.Items.Add(new ToolStripSeparator());
			menu.Items.Add(recent);
			menu.Items.Add(new ToolStripSeparator());
			menu.Items.Add(save);
			menu.Items.Add(activate);
			menu.Items.Add(desktop);
			menu.Items.Add(area);
			menu.Items.Add(new ToolStripSeparator());
			menu.Items.Add(open);
			menu.Items.Add(exit);

			tray.ContextMenuStrip = menu;
			tray.MouseDoubleClick += (o, e) => ShowOption();
			tray.BalloonTipClicked += (o, e) => BallonTouch(tag);
		}

		private void Function(object sender, EventArgs e) {
			string tag = (sender as ToolStripMenuItem).Tag.ToString();

			switch (tag) {
				case "activate":
					UploadFile(ScreenCapture.UploadActivate());
					break;
				case "desktop":
					UploadFile(ScreenCapture.UploadDesktop());
					break;
				case "area":
					capWindow.Reset(true);
					break;
				case "save":
					capWindow.Reset(false);
					break;
				default:
					break;
			}
		}

		private void ChangeTray(bool isUpload) {
			if (isUpload) {
				tray.Icon = System.Drawing.Icon.FromHandle(Simplane.Properties.Resources.upload.Handle);
			} else {
				if (Version.NowVersion != updater.NewVersion) {
					tray.Icon = System.Drawing.Icon.FromHandle(Simplane.Properties.Resources.update.Handle);
				}
				else {
					tray.Icon = System.Drawing.Icon.FromHandle(Simplane.Properties.Resources.normal.Handle);
				}
			}
		}

		private void ShowOption() {
			ApplySetting();
			this.Show();
		}

		private void HideOption() {
			this.Hide();
		}

		string tag { get; set; }
		private void ShowNotify(string message, string title, ToolTipIcon icon = ToolTipIcon.None, bool saveTag = false) {
			this.Dispatcher.BeginInvoke(new Action(() => {
				if (message == "") {
					//tray.ShowBalloonTip(10000, "Failed", "Error!!", ToolTipIcon.Error);
					notiWindow.Alert("Error!!", "Failed...", "Error", Brushes.Crimson, 10);
					tag = "";
				}
				else {
					//tray.ShowBalloonTip(10000, title, message, icon);

					if (saveTag) {
						tag = message;
						notiWindow.Alert(title, message, "Url", Brushes.CornflowerBlue, 10);
					}
					else {
						tag = "";
						notiWindow.Alert(title, message, "Noti", Brushes.CornflowerBlue, 10);
					}
				}
			}));
		}

		List<string> listRecent = new List<string>();
		private void AddLink(string url) {
			if (listRecent.Count == 5) {
				listRecent.RemoveAt(0);
			}
			listRecent.Add(url);

			List<int> listIndex = new List<int>();
			for (int i = menu.Items.Count - 1; i >= 0; i--) {
				if (menu.Items[i] is ToolStripMenuItem) {
					if ((menu.Items[i] as ToolStripMenuItem).Tag is KeyValuePair<string, string>) {
						listIndex.Add(i);
					}
				}
			}

			foreach (int i in listIndex) {
				menu.Items.RemoveAt(i);
			}

			for (int i = 0; i < listRecent.Count; i++) {
				try {
					string[] split = listRecent[i].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
					string name = split[split.Length - 1];
					ToolStripMenuItem item = new ToolStripMenuItem(name, null, FunctionRecent) {
						Tag = new KeyValuePair<string, string>("recent", listRecent[i]),
					};
					menu.Items.Insert(3, item);
				}
				catch { }
			}
		}

		private void FunctionRecent(object sender, EventArgs e) {
			string url = ((KeyValuePair<string, string>)(sender as ToolStripMenuItem).Tag).Value;
			BallonTouch(url);
		}

		private void BallonTouch(string url) {
			if (url != "") {
				try {
					Process pro = new Process() {
						StartInfo = new ProcessStartInfo() {
							FileName = url
						}
					};
					pro.Start();
				}
				catch { }
			}
		}
	}
}
