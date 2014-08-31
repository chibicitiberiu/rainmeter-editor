using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Core.Utils
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// Gets a tree of the folder structure
        /// </summary>
        /// <param name="folder">Folder</param>
        /// <returns>A tree</returns>
        public static Reference GetFolderTree(string folder)
        {
            // Build tree object
            Reference refTree = new Reference(Path.GetFileName(folder), folder);
            
            // Navigate folder structure
            if (Directory.Exists(folder))
            {
                foreach (var item in Directory.EnumerateDirectories(folder)
                    .Concat(Directory.EnumerateFiles(folder)))
                {
                    refTree.Add(GetFolderTree(item));
                }
            }

            // Return tree
            return refTree;
        }

        /// <summary>
        /// Returns true if two paths are equal
        /// </summary>
        /// <param name="path1">First path</param>
        /// <param name="path2">Second path</param>
        /// <returns>True if the paths are equal</returns>
        public static bool PathsEqual(string path1, string path2)
        {
            path1 = System.IO.Path.GetFullPath(path1);
            path2 = System.IO.Path.GetFullPath(path2);

            return String.Equals(path1, path2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
