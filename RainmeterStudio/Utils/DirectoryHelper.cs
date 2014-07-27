using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RainmeterStudio.Model;

namespace RainmeterStudio.Utils
{
    public static class DirectoryHelper
    {
        public static Tree<Reference> GetFolderTree(string folder)
        {
            // Build tree object
            Reference reference = new Reference(Path.GetFileName(folder), folder);
            Tree<Reference> tree = new Tree<Reference>(reference);

            // Navigate folder structure
            if (Directory.Exists(folder))
            {
                foreach (var item in Directory.EnumerateDirectories(folder)
                    .Concat(Directory.EnumerateFiles(folder)))
                {
                    tree.Add(GetFolderTree(item));
                }
            }

            // Return tree
            return tree;
        }
    }
}
