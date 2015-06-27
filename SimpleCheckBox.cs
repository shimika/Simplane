using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Simplane {
	class SimpleCheckbBox : CheckBox {
		public string Method { get; set; }

		bool eventBlock = false;
		public bool RawValue {
			get { return Value; }
			set {
				eventBlock = true;
				Value = value;
				eventBlock = false;
			}
		}

		public bool Value {
			get { return (bool)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(bool), typeof(SimpleCheckbBox),
			new FrameworkPropertyMetadata(
				false,
				FrameworkPropertyMetadataOptions.AffectsRender,
				ValuePropertyChanged));

		private static void ValuePropertyChanged(DependencyObject property, DependencyPropertyChangedEventArgs e) {
			SimpleCheckbBox box = property as SimpleCheckbBox;
			box.IsChecked = (bool)e.NewValue;

			if (box.ValueChanged != null && !box.eventBlock) {
				box.ValueChanged(box, new CheckBoxEventArgs(box.Method, (bool)e.NewValue));
			}
		}

		public event EventHandler<CheckBoxEventArgs> ValueChanged;

		protected override void OnChecked(RoutedEventArgs e) {
			base.OnChecked(e);
			if (!eventBlock) {
				this.Value = true;
			}
		}

		protected override void OnUnchecked(RoutedEventArgs e) {
			base.OnUnchecked(e);
			if (!eventBlock) {
				this.Value = false;
			}
		}
	}

	public class CheckBoxEventArgs : EventArgs {
		public string Method { get; internal set; }
		public bool NewValue { get; internal set; }

		public CheckBoxEventArgs(string method, bool nValue) {
			this.Method = method;
			this.NewValue = nValue;
		}
	}
}
