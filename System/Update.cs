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
using ShimiKore;

namespace Simplane {
	public partial class MainWindow : Window {

		Updater updater = null;

		DispatcherTimer timerUpdateIndicator;
		int turnUpdate;

		private void InitUpdater() {
			updater = new Updater("Simplane", Version.NowVersion);
			updater.UpdateAvailable += UpdateAvailable;
			updater.UpdateComplete += UpdateComplete;

			updater.UpdateCheck();
		}

		private void UpdateAvailable(object sender, UpdateArgs e) {
			textVersion.Text = string.Format("{0} (Newest {1})", Version.NowVersion, e.NewVersion);

			buttonUpdateCheck.StopAnimateImage();
			textVersion.Opacity = 1;

			if (e.IsOld) {
				ChangeTray(false);

				buttonUpdateCheck.ViewMode = ImageButton.Mode.Hidden;
				buttonUpdate.ViewMode = ImageButton.Mode.Visible;
			}
		}

		private void UpdateComplete(object sender, UpdateCompleteArgs e) {
			if (e.Complete) {
				this.Close();
			}
			else {
				buttonUpdate.ViewMode = ImageButton.Mode.Visible;
				StopUpdateIndicator();
			}
		}

		private void UpdateCheck() {
			textVersion.Opacity = 0;
			buttonUpdateCheck.StartAnimateImage(-1);

			updater.UpdateCheck();
		}

		private void UpdateDownload() {
			buttonUpdate.ViewMode = ImageButton.Mode.Disable;
			StartUpdateIndicator();

			updater.UpdateApplication();
		}
	}
}
