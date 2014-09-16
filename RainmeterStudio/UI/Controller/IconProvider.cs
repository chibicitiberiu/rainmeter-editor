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

            // Is a file?
            if (item.TargetKind == ReferenceTargetKind.File || item.TargetKind == ReferenceTargetKind.Project)
            {
                var extension = Path.GetExtension(item.StoragePath);

                if (String.IsNullOrEmpty(extension))
                    key += "Unknown";

                else
                    key += "_" + extension.Substring(1);
            }

            // Not a file, try to figure out if a directory
            else if (item.TargetKind == ReferenceTargetKind.Directory)
            {
                key += "Directory";
            }

            // None
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
            if (value is Reference)
            {
                return IconProvider.GetProjectItemIcon((Reference)value);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
