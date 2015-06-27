using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Simplane {
	public class ScreenCapture {
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

		[StructLayout(LayoutKind.Sequential)]
		public struct Rect {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		public static Bitmap CaptureWindow(IntPtr handle, Rect rect) {
			try {
				Rectangle bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
				Bitmap result = new Bitmap(bounds.Width, bounds.Height);

				using (var graphics = Graphics.FromImage(result)) {
					graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);
				}

				return result;
			} catch {
				return null;
			}
		}

		public static Bitmap CaptureWindow(IntPtr handle) {
			Rect rect = new Rect();
			GetWindowRect(handle, ref rect);

			return CaptureWindow(handle, rect);
		}

		public static Bitmap CaptureDesktop() {
			Bitmap bitmap = CaptureWindow(GetDesktopWindow(), new Rect() {
				Left = 0, Top = 0,
				Right = (int)SystemParameters.VirtualScreenWidth,
				Bottom = (int)SystemParameters.VirtualScreenHeight,
			});
			return bitmap;
		}

		public static Bitmap CaptureActiveWindow() {
			return CaptureWindow(GetForegroundWindow());
		}

		public static Bitmap CaptureArea(int left, int top, int right, int bottom) {
			Bitmap bitmap = CaptureWindow(GetDesktopWindow(), new Rect() {
				Left = left, Top = top,
				Right = right, Bottom = bottom,
			});

			return bitmap;
		}

		private static BitmapSource GetBitmapSource(Bitmap bitmap) {
			if (bitmap == null) {
				throw new ArgumentNullException("bitmap");
			}

			return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
				bitmap.GetHbitmap(),
				IntPtr.Zero,
				System.Windows.Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
		}

		private static int Count = 0;

		public static string GetFileName() {
			return String.Format("{0}_{1}",
				DateTime.Now.ToString("yyyy_MM_dd_HH.mm.ss"),
				Count++);
		}

		public static string SaveImage(Bitmap bitmap) {
			if (bitmap == null) { return ""; }

			try {
				if (!Directory.Exists(Setting.SavePath)) {
					Directory.CreateDirectory(Setting.SavePath);
				}

				string fileName = String.Format("{0}.{1}", GetFileName(), Setting.SaveMethod.ToLower());
				string filePath = String.Format("{0}{1}", Setting.SavePath, fileName);

				Log.WriteLog(fileName);
				Log.WriteLog(filePath);

				if (Setting.SaveMethod == "JPG") {
					JpegBitmapEncoder jpg = new JpegBitmapEncoder();
					jpg.QualityLevel = 90;
					jpg.Frames.Add(BitmapFrame.Create(GetBitmapSource(bitmap)));

					using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
						jpg.Save(fs);
					}
				}
				else {
					PngBitmapEncoder png = new PngBitmapEncoder();
					png.Frames.Add(BitmapFrame.Create(GetBitmapSource(bitmap)));

					using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
						png.Save(fs);
					}
				}

				return filePath;
			} catch { }
			return "";
		}

		public static string UploadActivate() {
			Bitmap img = CaptureActiveWindow();
			return SaveImage(img);
		}

		public static string UploadDesktop() {
			Bitmap img = CaptureDesktop();
			return SaveImage(img);
		}

		public static string SaveArea(int left, int top, int right, int bottom) {
			Bitmap img = CaptureArea(left, top, right, bottom);
			return SaveImage(img);
		}
	}
}
