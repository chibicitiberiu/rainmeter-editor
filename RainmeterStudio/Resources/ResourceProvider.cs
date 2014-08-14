using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RainmeterStudio.Resources
{
    /// <summary>
    /// Manages and provides resources
    /// </summary>
    public static class ResourceProvider
    {
        /// <summary>
        /// Holds information about a resource manager
        /// </summary>
        private struct ResourceManagerInfo
        {
            public ResourceManager Manager;
            public Assembly Assembly;
        }

        private static List<ResourceManagerInfo> _resourceManagers = new List<ResourceManagerInfo>();
        private static Dictionary<string, string> _cacheStrings = new Dictionary<string, string>();
        private static Dictionary<string, ImageSource> _cacheImages = new Dictionary<string, ImageSource>();

        /// <summary>
        /// Registers a resource manager
        /// </summary>
        /// <param name="manager">The resource manager</param>
        /// <param name="ownerAssembly">The assembly which will contain the non-string resources</param>
        public static void RegisterManager(ResourceManager manager, Assembly ownerAssembly)
        {
            _resourceManagers.Add(new ResourceManagerInfo()
            {
                Manager = manager,
                Assembly = ownerAssembly
            });
        }

        /// <summary>
        /// Gets a string from the resource managers
        /// </summary>
        /// <param name="key">Identifier of the resource</param>
        /// <param name="bypassCache">By default, strings are cached. Setting this to true will bypass the cache.</param>
        /// <param name="keepInCache">If this parameter is set to true, the obtained string will be cached.</param>
        /// <returns>The string, or null if not found</returns>
        public static string GetString(string key, bool bypassCache = false, bool keepInCache = true)
        {
            string value = null;

            // Look up in cache
            if (bypassCache || !_cacheStrings.TryGetValue(key, out value))
            {
                // Not found, query resource managers
                foreach (var info in _resourceManagers)
                {
                    // Try to get resource
                    var str = info.Manager.GetString(key);

                    // Found?
                    if (str != null)
                    {
                        value = str;

                        if (keepInCache)
                            _cacheStrings[key] = str;

                        break;
                    }
                }
            }

            // Resource not found
            return value;
        }

        /// <summary>
        /// Gets an image from the resource manager.
        /// </summary>
        /// <param name="key">Identifier of the resource</param>
        /// <param name="bypassCache">By default, images are cached. Setting this to true will bypass the cache.</param>
        /// <param name="keepInCache">If this parameter is set to true, the obtained image will be cached.</param>
        /// <returns>The image source, or null if not found</returns>
        public static ImageSource GetImage(string key, bool bypassCache = false, bool keepInCache = true)
        {
            ImageSource image = null;

            // Look up in cache
            if (bypassCache || !_cacheImages.TryGetValue(key, out image))
            {
                // Not found, query resource managers
                foreach (var info in _resourceManagers)
                {
                    // Try to get resource
                    var path = info.Manager.GetString(key);

                    // Found
                    if (path != null)
                    {
                        Uri fullPath = new Uri("/" + info.Assembly.GetName().Name + ";component" + path, UriKind.Relative);
                        image = new BitmapImage(fullPath);

                        if (keepInCache)
                            _cacheImages[key] = image;

                        break;
                    }
                }
            }
            
            // Resource not found
            return image;
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        public static void ClearCache()
        {
            _cacheImages.Clear();
            _cacheStrings.Clear();
        }
    }
}
