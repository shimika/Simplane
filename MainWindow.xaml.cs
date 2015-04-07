using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simplane {
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			App.mainWindow = this;

			this.Left = SystemParameters.PrimaryScreenWidth / 2 - 150;
			this.Top = SystemParameters.PrimaryScreenHeight / 2 - 150;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			if (Setting.Load()) { HideOption(); }
			ApplySetting();

			SetHotkeyEvent();
			ToggleHotKeyMode(true);

			InitTray();
			InitUpdateTimer();
		}

		private void ApplySetting() {
			textboxServer.Text = Setting.Server;
			textboxToken.Text = Setting.Token;

			buttonSave.ViewMode = ImageButton.Mode.Hidden;
		}

		private void Statusbar_MouseDown(object sender, MouseButtonEventArgs e) {
			DragMove();
		}

		private void ImageButton_Response(object sender, CustomButtonEventArgs e) {
			switch (e.Main) {
				case "close":
					HideOption();
					break;
				case "save":
					Setting.Server = textboxServer.Text;
					Setting.Token = textboxToken.Text;

					Setting.Save();
					buttonSave.ViewMode = ImageButton.Mode.Hidden;
					break;
				case "vercheck":
					CheckUpdate(true);
					break;
				case "update":
					UpdateDownload();
					break;
				default:
					MessageBox.Show(e.Main);
					break;
			}
		}

		private void Textbox_TextChanged(object sender, TextChangedEventArgs e) {
			buttonSave.ViewMode = ImageButton.Mode.Visible;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			try {
				window.Close();
				notiWindow.Close();
				tray.Dispose();
			} catch { }
		}

		public bool ProcessCommandLineArgs(IList<string> args) {
			tray.ShowBalloonTip(3000, "Alive", "I'm here~", System.Windows.Forms.ToolTipIcon.None);
			return true;
		}
	}
}
