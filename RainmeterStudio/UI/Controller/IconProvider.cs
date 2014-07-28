using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RainmeterStudio.Model;

namespace RainmeterStudio.UI.Controller
{
    public static class IconProvider
    {
        private static Dictionary<string, ImageSource> _loadedImages = new Dictionary<string, ImageSource>();

        public static ImageSource GetIcon(string key)
        {
            if (!_loadedImages.ContainsKey(key))
            {
                // Try to get the icon file name
                string iconPath = Resources.Icons.ResourceManager.GetString(key);
                if (iconPath == null)
                    return null;

                // Load the image
                var uri = new Uri(iconPath, UriKind.RelativeOrAbsolute);
                _loadedImages.Add(key, new BitmapImage(uri));
            }
            return _loadedImages[key];
        }

        public static ImageSource GetProjectItemIcon(Reference item)
        {
            // Resource name
            string key = "ProjectItem";

            if (Directory.Exists(item.Path))
                key += "Directory";

            else if (File.Exists(item.Path))
                key += "_" + Path.GetExtension(item.Path).Substring(1);

            else key += "None";

            // Get icon
            var icon = GetIcon(key);
            if (icon == null)
                return GetIcon("ProjectItemUnknown");
            return icon;
        }
    }

    public class IconProviderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var reference = value as Reference;
            if (reference != null)
            {
                return IconProvider.GetProjectItemIcon(reference);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
