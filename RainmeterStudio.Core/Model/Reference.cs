using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RainmeterStudio.Core.Model
{
    /// <summary>
    /// Reference to a file or folder
    /// </summary>
    public class Reference
    {
        /// <summary>
        /// Gets the name of the reference
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the path of the reference
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Initializes the reference
        /// </summary>
        /// <param name="name">Name of reference</param>
        /// <param name="path">Path to item referenced</param>
        public Reference(string name, string path = null)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        /// Compares a reference to another objects
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>True if objects are equal</returns>
        public override bool Equals(object obj)
        {
            var other = obj as Reference;
            
            // Types are different, so not equal
            if (other == null)
                return false;

            // Compare using string equals
            return String.Equals(Name, other.Name) && String.Equals(Path, other.Path);
        }

        /// <summary>
        /// Obtains the hash code of this reference
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            int hash = (Name == null) ? 0 : Name.GetHashCode();
            hash = hash * 7 + ((Path == null) ? 0 : Path.GetHashCode());

            return hash;
        }
    }
}
