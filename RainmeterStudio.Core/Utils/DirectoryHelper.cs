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
            Reference refTree = new Reference(Path.GetFileName(folder), folder, ReferenceTargetKind.File);
            
            // Navigate folder structure
            if (Directory.Exists(folder))
            {
                refTree.TargetKind = ReferenceTargetKind.Directory;

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

        /// <summary>
        /// Copies a directory from source to destination
        /// </summary>
        /// <param name="source">Directory to copy</param>
        /// <param name="destination">Destination directory</param>
        /// <param name="merge"></param>
        /// <remarks>If destination exists, the contents of 'source' will be copied to destination.
        /// Else, destination will be created, and the contents of source will be copied to destination.</remarks>
        public static void CopyDirectory(string source, string destination, bool merge = false)
        {
            if (source == destination)
                throw new IOException("You cannot copy a folder in the same folder.");

            if (Directory.Exists(destination) && !merge)
                throw new IOException("Destination folder already exists.");

            foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
            {
                string newFile = file.StartsWith(source) ? Path.Combine(destination, file.Substring(source.Length).Trim('\\')) : file;
                string newDirectory = Path.GetDirectoryName(newFile);

                if (!String.IsNullOrEmpty(newDirectory) && !Directory.Exists(newDirectory))
                    Directory.CreateDirectory(newDirectory);

                File.Copy(file, newFile);
            }
        }
    }
}
