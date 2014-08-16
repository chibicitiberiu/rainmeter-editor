using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Core.Utils
{
    public static class PathHelper
    {
        /// <summary>
        /// Validates a path
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>True if the path is valid</returns>
        public static bool IsPathValid(string path)
        {
            // Check for invalid characters
            if (Path.GetInvalidPathChars().Intersect(path).Any())
                return false;

            return true;
        }

        /// <summary>
        /// Validates a file name
        /// </summary>
        /// <param name="name">Name of file</param>
        /// <returns></returns>
        public static bool IsFileNameValid(string name)
        {
            // No name is not a valid name
            if (String.IsNullOrEmpty(name))
                return false;

            // Check for invalid characters
            if (Path.GetInvalidFileNameChars().Intersect(name).Any())
                return false;

            return true;
        }
    }
}
