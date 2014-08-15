using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RainmeterStudio.Core.Utils
{
    /// <summary>
    /// Helper methods for key gestures
    /// </summary>
    public static class InputHelper
    {
        /// <summary>
        /// Converts a key gesture into its string representation
        /// </summary>
        /// <param name="gesture">Key gesture</param>
        /// <returns>The string representation</returns>
        public static string ConvertToString(this KeyGesture gesture)
        {
            // Safety check
            if (gesture == null)
                return null;

            // Build string
            string text = String.Empty;

            if ((gesture.Modifiers & ModifierKeys.Windows) != 0)
                text += "Win+";

            if ((gesture.Modifiers & ModifierKeys.Control) != 0)
                text += "Ctrl+";

            if ((gesture.Modifiers & ModifierKeys.Alt) != 0)
                text += "Alt+";

            if ((gesture.Modifiers & ModifierKeys.Shift) != 0)
                text += "Shift+";

            text += Enum.GetName(typeof(Key), gesture.Key);
            return text;
        }

        /// <summary>
        /// Obtains a key gesture from a string representation
        /// </summary>
        /// <param name="keyGesture">The key gesture string</param>
        /// <returns>A key gesture object</returns>
        public static KeyGesture GetKeyGesture(string keyGesture)
        {
            // Safety check
            if (keyGesture == null)
                return null;

            // Variables
            ModifierKeys mods = ModifierKeys.None;
            Key key = Key.None;

            // Parse each field
            foreach (var field in keyGesture.Split('+'))
            {
                // Trim surrounding white space
                string trimmed = field.Trim();

                // Parse
                if (trimmed.Equals("Win", StringComparison.InvariantCultureIgnoreCase))
                    mods |= ModifierKeys.Windows;
                if (trimmed.Equals("Ctrl", StringComparison.InvariantCultureIgnoreCase))
                    mods |= ModifierKeys.Control;
                if (trimmed.Equals("Alt", StringComparison.InvariantCultureIgnoreCase))
                    mods |= ModifierKeys.Alt;
                if (trimmed.Equals("Shift", StringComparison.InvariantCultureIgnoreCase))
                    mods |= ModifierKeys.Shift;
                else Enum.TryParse<Key>(field, out key);
            }

            return new KeyGesture(key, mods);
        }
    }
}
