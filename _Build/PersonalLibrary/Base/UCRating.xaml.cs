using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Base {
	public partial class UCRating : UserControl {
		public static readonly DependencyProperty RatingValueProperty =
			DependencyProperty.Register("RatingValue", typeof(int), typeof(UCRating),
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					new PropertyChangedCallback(RatingValueChanged)));


		private int _maxValue = 5;
		public event Action<UCRating> onClick;

		public int RatingValue {
			get { return (int)GetValue(RatingValueProperty); }
			set {
				if (value < 0)
					SetValue(RatingValueProperty, 0);
				else if (value > _maxValue)
					SetValue(RatingValueProperty, _maxValue);
				else
					SetValue(RatingValueProperty, value);
			}
		}

		public UCRating() {
			InitializeComponent();
		}

		private static void RatingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			var parent = sender as UCRating;
			var ratingValue = (int)e.NewValue;
			var children = ((Grid)(parent.Content)).Children;
			ToggleButton button = null;

			for (int i = 0; i < ratingValue; i++) {
				button = children[i] as ToggleButton;
				if (button != null)
					button.IsChecked = true;
			}

			for (int i = ratingValue; i < children.Count; i++) {
				button = children[i] as ToggleButton;
				if (button != null)
					button.IsChecked = false;
			}
		}

		private void RatingButtonClickEventHandler(object sender, RoutedEventArgs e) {
			var button = sender as ToggleButton;
			var newRating = int.Parse(button.Tag.ToString());

			if ((bool)button.IsChecked || newRating < RatingValue)
				RatingValue = newRating;
			else
				RatingValue = newRating - 1;

			e.Handled = true;
			onClick?.Invoke(this);
		}
	}
}