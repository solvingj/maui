using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Maui.Controls
{
	[Xaml.TypeConversion(typeof(ImageSource))]
	public sealed class ImageSourceConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			=> sourceType == typeof(string);

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			=> destinationType == typeof(string);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var strValue = value?.ToString();
			if (strValue != null)
				return Uri.TryCreate(strValue, UriKind.Absolute, out Uri uri) && uri.Scheme != "file" ? ImageSource.FromUri(uri) : ImageSource.FromFile(strValue);

			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", strValue, typeof(ImageSource)));
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is FileImageSource fis)
				return fis.File;
			if (value is UriImageSource uis)
				return uis.Uri.ToString();
			throw new NotSupportedException();
		}
	}
}