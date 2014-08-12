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
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("path")]
        public string Path { get; set; }

        public Reference()
        {
        }

        public Reference(string name, string path = null)
        {
            Name = name;
            Path = path;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Reference;
            
            // Types are different, so not equal
            if (other == null)
                return false;

            // Compare using string equals
            return String.Equals(Name, other.Name) && String.Equals(Path, other.Path);
        }

        public override int GetHashCode()
        {
            int hash = (Name == null) ? 0 : Name.GetHashCode();
            hash = hash * 7 + ((Path == null) ? 0 : Path.GetHashCode());

            return hash;
        }
    }
}
