using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Simplane {
	/// <summary>
	/// CaptureWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class CaptureWindow : Window {
		Point pointDown = new Point(-1, -1), pointUp;
		public event EventHandler<AreaEventArgs> Response;

		bool viewmode = false;
		bool Upload = true;

		public CaptureWindow() {
			InitializeComponent();
		}

		public void Reset(bool isUpload) {
			if (viewmode) { return; }

			Upload = isUpload;
			viewmode = true;

			this.Left = this.Top = 0;
			this.Width = SystemParameters.VirtualScreenWidth;
			this.Height = SystemParameters.VirtualScreenHeight;

			pointDown = new Point(-1, -1);
			SetRectBrush(Upload ? Brushes.Black : Brushes.Crimson);

			this.Opacity = 1;
			this.Activate();
		}

		private void SetRectBrush(Brush brush) {
			border.BorderBrush = brush;
		}

		private void SetRectOpacity(double opacity) {
			border.Opacity = opacity;
		}

		private void SetRect(double left, double top, double right, double bottom) {
			Canvas.SetLeft(border, Math.Min(left, right));
			Canvas.SetTop(border, Math.Min(top, bottom));

			border.Width = Math.Abs(left - right);
			border.Height = Math.Abs(top - bottom);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			AltTab.HideAltTab(this);
			this.Activate();
		}

		private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			pointDown = e.GetPosition(this);

			SetRectOpacity(1);
			SetRect(0, 0, 0, 0);
			grid.Opacity = 1;
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.RightButton == MouseButtonState.Pressed && pointDown.X >= 0) {
				pointDown = new Point(-1, -1);
				HideComponent();
			}
		}

		private void Grid_MouseMove(object sender, MouseEventArgs e) {
			if (pointDown.X < 0) { return; }

			Refresh(e.GetPosition(this));
		}

		private void Grid_MouseUp(object sender, MouseButtonEventArgs e) {
			if (pointDown.X < 0) { return; }

			pointUp = e.GetPosition(this);

			HideComponent();

			DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100), };
			timer.Tick += timer_Tick;
			timer.Start();
		}

		private void HideComponent() {
			viewmode = false;
			SetRectOpacity(0);
			grid.Opacity = 0;
			this.Opacity = 0;

			WindowOrder.FocusZOrderWindow();
		}

		private void timer_Tick(object sender, EventArgs e) {
			(sender as DispatcherTimer).Stop();

			if (Response != null) {
				Response(this, new AreaEventArgs(pointDown.X, pointDown.Y, pointUp.X, pointUp.Y, Upload));
			}
		}

		private void Refresh(Point next) {
			double left = Math.Min(next.X, pointDown.X);
			double top = Math.Min(next.Y, pointDown.Y);
			double right = Math.Max(next.X, pointDown.X);
			double bottom = Math.Max(next.Y, pointDown.Y);

			double width = Math.Abs(next.X - pointDown.X);
			double height = Math.Abs(next.Y - pointDown.Y);

			SetRect(left, top, right, bottom);

			text.Text = String.Format("{0} x {1}", (int)width, (int)height);

			Canvas.SetLeft(grid, right - 140);
			Canvas.SetTop(grid, top - 50);
		}

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape) {
				HideComponent();
			}
		}

		private void Window_Deactivated(object sender, EventArgs e) {
			if (viewmode) {
				//this.Activate();
			}
		}
	}

	public class AreaEventArgs : EventArgs {
		public int Left { get; internal set; }
		public int Right { get; internal set; }
		public int Top { get; internal set; }
		public int Bottom { get; internal set; }

		public bool Upload { get; internal set; }

		public AreaEventArgs(double left, double top, double right, double bottom, bool upload) {
			this.Left = (int)(Math.Min(left, right) * Setting.SystemMatrix.M11);
			this.Top = (int)(Math.Min(top, bottom) * Setting.SystemMatrix.M22);

			this.Right = (int)(Math.Max(left, right) * Setting.SystemMatrix.M11);
			this.Bottom = (int)(Math.Max(top, bottom) * Setting.SystemMatrix.M22);

			this.Upload = upload;
		}
	}
}
