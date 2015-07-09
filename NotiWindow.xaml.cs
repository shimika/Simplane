using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Simplane {
	/// <summary>
	/// NotiWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class NotiWindow : Window {
		public NotiWindow(MainWindow mainWindow) {
			InitializeComponent();
			mWindow = mainWindow;

			RearrangeWindow();
			DispatcherTimer dtm = new DispatcherTimer() {
				Interval = TimeSpan.FromMilliseconds(3000),
				IsEnabled = true,
			};
			dtm.Tick += (o, e) => RearrangeWindow();

			this.Loaded += NotiWindow_Loaded;
		}

		MainWindow mWindow = null;
		public bool IsClose = false;

		private void NotiWindow_Loaded(object sender, RoutedEventArgs e) {
			AltTab.HideAltTab(this);
		}

		private void RearrangeWindow() {
			this.Top = SystemParameters.WorkArea.Top;
			this.Left = SystemParameters.WorkArea.Width - 350;
			this.Height = SystemParameters.WorkArea.Height;
		}

		public void Alert(string title, string script, string type, Brush brush, int Timeout = 70) {
			this.Topmost = false; this.Topmost = true;

			NotiItem item = new NotiItem(title, script, type, brush);
			item.Response += item_Response;
			stackNotiPanel.Children.Add(item);

			DispatcherTimer dtm = new DispatcherTimer() {
				Interval = TimeSpan.FromSeconds(Timeout),
				IsEnabled = true, Tag = item,
			};
			dtm.Tick += (o, e) => {
				(o as DispatcherTimer).Stop();
				DeleteAlert((o as DispatcherTimer).Tag as NotiItem);
				// Timeout
			};
		}

		private void item_Response(object sender, CustomButtonEventArgs e) {
			if (e.ActionType == "Action") {
				switch (e.Main) {
					case "Url":
						BallonTouch(e.Detail);
						break;
					case "Local":
						string path =  Path.Combine(
							Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
							e.Detail);

						if (File.Exists(path)) {
							string argument = String.Format("/select, \"{0}\"", path);
							Task.Factory.StartNew(() => Process.Start("explorer.exe", argument));
						}

						break;
				}
			}

			DeleteAlert(sender as NotiItem);
			//MessageBox.Show(String.Format("{0} {1} {2}", e.ActionType, e.Main, e.Detail));
		}

		public int UnreadCount = 0;
		private void DeleteAlert(NotiItem grid) {
			try {
				Storyboard sb = new Storyboard();
				DoubleAnimation da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250)) {
					EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 5 }
				};
				Storyboard.SetTarget(da, grid);
				Storyboard.SetTargetProperty(da, new PropertyPath(Grid.OpacityProperty));

				sb.Children.Add(da);
				sb.Completed += (o, e) => {
					try {
						stackNotiPanel.Children.Remove(grid);
					}
					catch { }
				};
				sb.Begin(this);
			}
			catch { }
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
