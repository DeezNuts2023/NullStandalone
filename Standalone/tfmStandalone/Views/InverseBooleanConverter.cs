using System;
using System.Globalization;
using System.Windows.Data;

namespace tfmStandalone.Views
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return true;
			}
			if (value is bool)
			{
				return !(bool)value;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return true;
			}
			if (value is bool)
			{
				return !(bool)value;
			}
			return false;
		}
	}
}
