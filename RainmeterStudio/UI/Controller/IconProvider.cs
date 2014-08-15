using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.UI.Controller
{
    /// <summary>
    /// Provides icons
    /// </summary>
    public static class IconProvider
    {
        /// <summary>
        /// Gets an icon by resource name
        /// </summary>
        /// <param name="key">Resource name</param>
        /// <returns>The icon</returns>
        public static ImageSource GetIcon(string key)
        {
            return ResourceProvider.GetImage(key);
        }

        /// <summary>
        /// Gets an icon by reference
        /// </summary>
        /// <param name="item">The reference</param>
        /// <returns>The icon</returns>
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

    /// <summary>
    /// Icon provider converter, for use in xaml
    /// </summary>
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
