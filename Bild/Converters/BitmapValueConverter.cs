using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Bild.Core.Files;
using System;
using System.Globalization;

namespace Bild.Converters
{
	public class BitmapValueConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is File input && System.IO.File.Exists(input.AbsolutePath))
			{
				return new Bitmap(input.AbsolutePath);
			}

			throw new NotSupportedException();
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
