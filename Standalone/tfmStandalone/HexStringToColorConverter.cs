using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace tfmStandalone
{
	public sealed class HexStringToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.IsNullOrWhiteSpace((value != null) ? value.ToString() : null))
			{
				return null;
			}
			return (Color?)ColorConverter.ConvertFromString(string.Format("#{0}", value));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
