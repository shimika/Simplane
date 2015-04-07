using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Simplane {
	public partial class MainWindow : Window {
		private void Window_Activated(object sender, EventArgs e) {
			grideffectShadow.BeginAnimation(
				DropShadowEffect.OpacityProperty,
				new DoubleAnimation(0.4, TimeSpan.FromMilliseconds(100)));
		}

		private void Window_Deactivated(object sender, EventArgs e) {
			grideffectShadow.BeginAnimation(
				DropShadowEffect.OpacityProperty,
				new DoubleAnimation(0.1, TimeSpan.FromMilliseconds(100)));
		}
	}
}
