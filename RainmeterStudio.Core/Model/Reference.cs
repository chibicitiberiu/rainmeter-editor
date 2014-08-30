using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RainmeterStudio.Core.Model
{
    /// <summary>
    /// Reference to a file or folder
    /// </summary>
    [DebuggerDisplay("ProjectPath = {ProjectPath}, StoragePath = {StoragePath}")]
    public struct Reference
    {
        private string[] _projectPath;
        private string _storagePath;

        /// <summary>
        /// Gets the name of the reference
        /// </summary>
        public string Name
        {
            get
            {
                // Try to get the last item from the project path
                if (_projectPath != null && _projectPath.Length > 0)
                    return _projectPath[_projectPath.Length - 1];

                // None found, return null
                return null;
            }
        }

        /// <summary>
        /// Gets the path to the file on the disk. If reference is in a project, the path should be relative.
        /// </summary>
        public string StoragePath
        {
            get
            {
                return _storagePath;
            }
        }

        /// <summary>
        /// Gets the qualified path
        /// </summary>
        public string ProjectPath
        {
            get
            {
                if (_projectPath != null)
                {
                    return _projectPath.Aggregate(String.Empty, (a, b) => a + "/" + b);
                }

                return null;
            }
        }

        /// <summary>
        /// Initializes the reference
        /// </summary>
        /// <param name="name">Name of reference</param>
        /// <param name="path">Path to item referenced</param>
        public Reference(string filePath, string projectPath = null)
        {
            _storagePath = filePath;

            if (projectPath != null)
            {
                _projectPath = projectPath.Split('/').Skip(1).ToArray();
            }
            else
            {
                _projectPath = null;
            }
        }

        /// <summary>
        /// Checks if the reference points to a project item
        /// </summary>
        public bool IsInProject()
        {
            return (_projectPath != null);
        }

        /// <summary>
        /// Checks if the reference has a file on disk
        /// </summary>
        /// <returns></returns>
        public bool IsOnStorage()
        {
            return (_storagePath != null);
        }

        /// <summary>
        /// Compares a reference to another objects
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>True if objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Reference)
            {
                Reference other = (Reference)obj;

                // 2 references are equal if they point to the same project item
                if (_projectPath != null && other._projectPath != null)
                    return _projectPath.SequenceEqual(other._projectPath);

                // If there is no project item, compare storage paths
                if (_projectPath == null && other._projectPath == null)
                    return String.Equals(_storagePath, other._storagePath);
            }

            return false;
        }

        /// <summary>
        /// Obtains the hash code of this reference
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            int hash = (_projectPath == null) ? 0 : _projectPath.GetHashCode();

            if (_projectPath != null)
            {
                foreach (var item in _projectPath)
                    hash = hash * 7 + item.GetHashCode();
            }
            else
            {
                hash = hash * 2113 + ((_storagePath == null) ? 0 : _storagePath.GetHashCode());
            }

            return hash;
        }

        /// <summary>
        /// Gets the string representation of this reference
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ProjectPath;
        }
    }
}
