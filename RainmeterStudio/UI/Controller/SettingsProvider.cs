using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RainmeterStudio.UI.Controller
{
    public static class SettingsProvider
    {
        /// <summary>
        /// Attempts to retrieve the setting of type T, where T is class
        /// </summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <param name="name">Name of setting</param>
        /// <returns>Retrieved setting, or null if not found</returns>
        public static T GetSetting<T> (string name) where T : class
        {
            var property = Properties.Settings.Default.Properties
                .OfType<SettingsProperty>()
                .FirstOrDefault(x => String.Equals(x.Name, name));
                
            return (property == null) ? null : (property.DefaultValue as T);
        }

        /// <summary>
        /// Attempts to retrieve the setting of type T
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="name">Name of setting</param>
        /// <param name="value">Output value</param>
        /// <returns>True if attempt was successful</returns>
        public static bool TryGetSetting<T>(string name, out T value)
        {
            var property = Properties.Settings.Default.Properties.OfType<SettingsProperty>().FirstOrDefault(x => x.Name.Equals(name));

            if (property != null)
            {
                value = (T)property.DefaultValue;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }
    }
}
