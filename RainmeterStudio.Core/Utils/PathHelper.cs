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
    }
}
