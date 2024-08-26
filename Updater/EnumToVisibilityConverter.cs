using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Updater
{
	public sealed class EnumToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null || !(value is Enum))
			{
				return Visibility.Collapsed;
			}
			return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
