using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;

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
