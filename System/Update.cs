using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Simplane {
	public partial class MainWindow : Window {
		string Project = "Simplane";

		DispatcherTimer TimerUpdate;
		string NewVersion = "", NewUrl = "";

		private void InitUpdateTimer() {
			textVersion.Text = Setting.Version;
			NewVersion = Setting.Version;

			TimerUpdate = new DispatcherTimer();
			TimerUpdate.Interval = TimeSpan.FromHours(1);
			TimerUpdate.Tick += TimerUpdate_Tick;
			TimerUpdate.Start();

			CheckUpdate();
		}

		private void TimerUpdate_Tick(object sender, EventArgs e) {
			CheckUpdate();
		}

		bool IsChecking = false;
		private void CheckUpdate(bool isForce = false) {
			if (IsChecking || NewVersion != Setting.Version) { return; }
			IsChecking = true;

			textVersion.Opacity = 0;
			buttonUpdateCheck.StartAnimateImage(-1);

			WebClient web = new WebClient();
			web.DownloadStringCompleted += web_DownloadStringCompleted;
			web.DownloadStringAsync(new Uri(String.Format("https://dl.dropboxusercontent.com/u/95054900/Shimika/{0}.txt", Project)), isForce);
		}

		private void web_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e) {
			buttonUpdateCheck.StopAnimateImage();
			textVersion.Opacity = 1;

			IsChecking = false;
			if (e.Error != null) {
				NewVersion = NewVersion = Setting.Version;
				NewUrl = "";
				return;
			}

			bool isForce = (bool)e.UserState;

			try {
				string[] res = e.Result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				NewVersion = res[0];
				NewUrl = res[1];

				textVersion.Text = string.Format("{0} (Newest {1})", Setting.Version, NewVersion);

				if (NewVersion != Setting.Version) {
					buttonUpdateCheck.ViewMode = ImageButton.Mode.Hidden;
					buttonUpdate.ViewMode = ImageButton.Mode.Visible;
					ChangeTray(false);
				}
			}
			catch {
			}
		}

		public static string UpdateFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Shimika\";

		BackgroundWorker BwUpdate;
		bool isUpdating = false;
		private void UpdateDownload() {
			if (isUpdating) { return; }
			if (NewUrl == "") { return; }
			isUpdating = true;

			buttonUpdate.ViewMode = ImageButton.Mode.Disable;
			if (!Directory.Exists(UpdateFolder)) {
				Directory.CreateDirectory(UpdateFolder);
			}

			StartUpdateIndicator();

			BwUpdate = new BackgroundWorker() {
				WorkerSupportsCancellation = true,
			};
			BwUpdate.DoWork += BwUpdate_DoWork;
			BwUpdate.RunWorkerCompleted += BwUpdate_RunWorkerCompleted;

			List<string> list = new List<string>();
			// Path, project, 

			list.Add(string.Format("{0}updater.exe", UpdateFolder));
			list.Add(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			list.Add(Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName));
			list.Add(NewUrl);

			BwUpdate.RunWorkerAsync(list);
		}

		private void BwUpdate_DoWork(object sender, DoWorkEventArgs e) {
			List<string> list = (List<string>)e.Argument;

			string updater = list[0];
			string path = list[1];
			string project = list[2];
			string url = list[3];

			WebClient web = new WebClient();
			web.DownloadFile("https://dl.dropboxusercontent.com/u/95054900/Shimika/Updater.exe", updater);
			web.DownloadFile(url, string.Format("{0}\\{1}_update.exe", path, project));

			e.Result = list;
		}

		private void BwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			buttonUpdate.ViewMode = ImageButton.Mode.Visible;

			if (e.Cancelled) {
				StopUpdateIndicator();
				isUpdating = false;
				return;
			}

			List<string> list = (List<string>)e.Result;
			string updater = list[0];
			string path = list[1];
			string project = list[2];
			int id = Process.GetCurrentProcess().Id;

			try {
				if (!File.Exists(updater)) {
					throw new Exception("Can't get updater");
				}

				if (!File.Exists(string.Format("{0}\\{1}_update.exe", path, project))) {
					throw new Exception("Update error");
				}

				Process pro = new Process();
				pro.StartInfo = new ProcessStartInfo();
				pro.StartInfo.FileName = updater;
				pro.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\" {2}", project, path, id);
				pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				pro.Start();

				this.Close();
			}
			catch (Exception ex) {
			}

			StopUpdateIndicator();
			isUpdating = false;
		}

		DispatcherTimer timerUpdateIndicator;
		int turnUpdate;

		// Torrent

		private void StartUpdateIndicator() {
			if (timerUpdateIndicator == null) {
				timerUpdateIndicator = new DispatcherTimer();
				timerUpdateIndicator.Interval = TimeSpan.FromMilliseconds(500);
				timerUpdateIndicator.Tick += timerUpdateIndicator_Tick;
			}
			turnUpdate = 0;
			timerUpdateIndicator.Start();
		}

		private void StopUpdateIndicator() {
			if (timerUpdateIndicator != null) {
				timerUpdateIndicator.Stop();
			}
			buttonUpdate.Opacity = 1;
		}

		private void timerUpdateIndicator_Tick(object sender, EventArgs e) {
			turnUpdate = (turnUpdate + 1) % 2;
			buttonUpdate.Opacity = turnUpdate;
		}
	}
}
