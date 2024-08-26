using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace tfmStandalone
{
	public sealed class ImageButton : Button
	{
		public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty ImageHoverProperty = DependencyProperty.Register("ImageHover", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty ImagePressedProperty = DependencyProperty.Register("ImagePressed", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton), new FrameworkPropertyMetadata(double.NaN));
		public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButton), new FrameworkPropertyMetadata(double.NaN));

        public ImageSource Image
        {
            get => (ImageSource)base.GetValue(ImageButton.ImageProperty);
            set => base.SetValue(ImageButton.ImageProperty, value);
        }

        public ImageSource ImageHover
        {
            get => (ImageSource)base.GetValue(ImageButton.ImageHoverProperty);
            set => base.SetValue(ImageButton.ImageHoverProperty, value);
        }

        public ImageSource ImagePressed
        {
            get => (ImageSource)base.GetValue(ImageButton.ImagePressedProperty);
            set => base.SetValue(ImageButton.ImagePressedProperty, value);
        }

        public double ImageWidth
        {
            get => (double)base.GetValue(ImageButton.ImageWidthProperty);
            set => base.SetValue(ImageButton.ImageWidthProperty, value);
        }

        public double ImageHeight
        {
            get => (double)base.GetValue(ImageButton.ImageHeightProperty);
            set => base.SetValue(ImageButton.ImageHeightProperty, value);
        }
    }
}
