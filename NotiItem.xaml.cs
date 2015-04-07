using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simplane {
	/// <summary>
	/// NotiItem.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class NotiItem : UserControl {

		public event EventHandler<CustomButtonEventArgs> Response;
		public string NotifyType { get; set; }
		public string NotifyScript { get; set; }

		public NotiItem(string title, string script, string type, Brush brush) {
			InitializeComponent();

			gridMain.Background = brush;
			textTitle.Foreground = brush;

			NotifyType = type;
			NotifyScript = script;

			textTitle.Text = title;
			textScript.Text = script;

			this.BeginAnimation(UserControl.OpacityProperty,
				new DoubleAnimation(1, TimeSpan.FromMilliseconds(250)));
		}

		private void CloseEvent(object sender, CustomButtonEventArgs e) {
			if (Response != null) {
				Response(this, new CustomButtonEventArgs("Close", NotifyType, NotifyScript));
			}
		}

		private void MainArea_MouseDown(object sender, MouseButtonEventArgs e) {
			if (Response != null) {
				Response(this, new CustomButtonEventArgs("Action", NotifyType, NotifyScript));
			}
		}
	}
}
