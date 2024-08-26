using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace tfmStandalone.Views
{
	public class PathToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = value as string;
			if (text != null)
			{
				BitmapImage bitmapImage = new BitmapImage();
				using (FileStream fileStream = File.OpenRead(text))
				{
					bitmapImage.BeginInit();
					bitmapImage.StreamSource = fileStream;
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.EndInit();
				}
				return bitmapImage;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
