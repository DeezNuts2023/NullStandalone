using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace tfmStandalone
{
	public class PinnableWindow : Window
	{
        public static readonly DependencyProperty CanPinProperty = DependencyProperty.Register("CanPin", typeof(bool), typeof(PinnableWindow), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty CanMinimizeProperty = DependencyProperty.Register("CanMinimize", typeof(bool), typeof(PinnableWindow), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty CanMaximizeProperty = DependencyProperty.Register("CanMaximize", typeof(bool), typeof(PinnableWindow), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(PinnableWindow), new FrameworkPropertyMetadata(true));

        public bool CanPin
        {
            get => (bool)base.GetValue(PinnableWindow.CanPinProperty);
            set => base.SetValue(PinnableWindow.CanPinProperty, value);
        }

        public bool CanMinimize
        {
            get => (bool)base.GetValue(PinnableWindow.CanMinimizeProperty);
            set => base.SetValue(PinnableWindow.CanMinimizeProperty, value);
        }

        public bool CanMaximize
        {
            get => (bool)base.GetValue(PinnableWindow.CanMaximizeProperty);
            set => base.SetValue(PinnableWindow.CanMaximizeProperty, value);
        }

        public bool CanClose
        {
            get => (bool)base.GetValue(PinnableWindow.CanCloseProperty);
            set => base.SetValue(PinnableWindow.CanCloseProperty, value);
        }


        public override void OnApplyTemplate()
		{
			Label label = base.GetTemplateChild("TitleLabel") as Label;
			if (label != null)
			{
				label.MouseLeftButtonDown += this.HandleTitleLabelMouseLeftButtonDown;
				label.MouseDoubleClick += this.HandleTitleLabelMouseDoubleClick;
			}
			Border border = base.GetTemplateChild("PinButton") as Border;
			if (border != null)
			{
				border.MouseUp += delegate(object sender, MouseButtonEventArgs args)
				{
					base.Topmost = true;
				};
			}
			Border border2 = base.GetTemplateChild("UnpinButton") as Border;
			if (border2 != null)
			{
				border2.MouseUp += delegate(object sender, MouseButtonEventArgs args)
				{
					base.Topmost = false;
				};
			}
			Border border3 = base.GetTemplateChild("MinimizeButton") as Border;
			if (border3 != null)
			{
				border3.MouseUp += delegate(object sender, MouseButtonEventArgs args)
				{
					base.WindowState = WindowState.Minimized;
				};
			}
			Border border4 = base.GetTemplateChild("MaximizeButton") as Border;
			if (border4 != null)
			{
				border4.MouseUp += delegate(object sender, MouseButtonEventArgs args)
				{
					base.WindowState = WindowState.Maximized;
				};
			}
			Border border5 = base.GetTemplateChild("RestoreButton") as Border;
			if (border5 != null)
			{
				border5.MouseUp += delegate(object sender, MouseButtonEventArgs args)
				{
					base.WindowState = WindowState.Normal;
				};
			}
			Border border6 = base.GetTemplateChild("CloseButton") as Border;
			if (border6 != null)
			{
				border6.MouseUp += delegate(object sender, MouseButtonEventArgs args)
				{
					base.Close();
				};
			}
			base.OnApplyTemplate();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!this.CanClose)
			{
				e.Cancel = true;
			}
			base.OnClosing(e);
		}

		private void HandleTitleLabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			base.DragMove();
		}

		private void HandleTitleLabelMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (base.WindowState == WindowState.Maximized)
			{
				base.WindowState = WindowState.Normal;
				return;
			}
			if (base.WindowState == WindowState.Normal)
			{
				base.WindowState = WindowState.Maximized;
			}
		}
	}
}
