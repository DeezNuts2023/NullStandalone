using System;
using System.Windows;
using System.Windows.Controls;

namespace tfmStandalone
{
	public class HeaderScrollViewer : ScrollViewer
	{
        public static DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(HeaderScrollViewer), new FrameworkPropertyMetadata(null));

        public object Header
        {
            get => base.GetValue(HeaderScrollViewer.HeaderProperty);
            set => base.SetValue(HeaderScrollViewer.HeaderProperty, value);
        }
    }
}
