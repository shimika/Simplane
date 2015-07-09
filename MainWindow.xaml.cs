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
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			checkJPG.ValueChanged += ImageSave_SettingChanged;
			checkPNG.ValueChanged += ImageSave_SettingChanged;

			if (Setting.Load()) { HideOption(); }
			ApplySetting();

			SetHotkeyEvent();
			InitCaptureManager();
			ToggleHotKeyMode(true);

			InitTray();
			InitUpdater();
		}

		private void ApplySetting() {
			textboxServer.Text = Setting.Server;
			textboxToken.Text = Setting.Token;

			if (Setting.SaveMethod == "JPG") {
				checkJPG.IsChecked = true;
			}
			else {
				checkPNG.IsChecked = true;
			}

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

					if (checkJPG.IsChecked == true) {
						Setting.SaveMethod = "JPG";
					}
					else {
						Setting.SaveMethod = "PNG";
					}

					Setting.Save();
					buttonSave.ViewMode = ImageButton.Mode.Hidden;
					break;
				case "vercheck":
					UpdateCheck();
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

		private void ImageSave_SettingChanged(object sender, CheckBoxEventArgs e) {
			if (e.NewValue == false) { return; }

			bool jpgCheck = e.Method == "JPG";

			checkJPG.IsChecked = jpgCheck;
			checkJPG.IsHitTestVisible = !jpgCheck;
			checkPNG.IsChecked = !jpgCheck;
			checkPNG.IsHitTestVisible = jpgCheck;

			buttonSave.ViewMode = ImageButton.Mode.Visible;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			try {
				Setting.Save();

				capWindow.Close();
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
