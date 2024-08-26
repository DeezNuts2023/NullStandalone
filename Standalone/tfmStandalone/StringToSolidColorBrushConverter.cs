using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace tfmStandalone
{
	public sealed class StringToSolidColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.IsNullOrWhiteSpace((value != null) ? value.ToString() : null))
			{
				return null;
			}
			try
			{
				return (SolidColorBrush)new BrushConverter().ConvertFrom(string.Format("{0}", value));
			}
			catch
			{
			}
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
