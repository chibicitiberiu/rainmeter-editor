using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RainmeterStudio.Core.Utils
{
    public static class BitmapHelper
    {
        public static ImageSource GetImageSource(this System.Drawing.Bitmap image)
        {
            BitmapSource destination;
            IntPtr bitmapHandle = image.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            destination = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmapHandle, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            destination.Freeze();
            return destination;
        }
    }

    public class BitmapToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var bitmap = value as System.Drawing.Bitmap;
            if (bitmap != null)
            {
                return bitmap.GetImageSource();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
