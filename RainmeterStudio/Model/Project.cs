using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RainmeterStudio.Model
{
    public class Project
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("author")]
        public string Author { get; set; }

        [XmlIgnore]
        public Version Version { get; set; }

        [XmlElement("version")]
        public string VersionString
        {
            get 
            {
                return Version.ToString();
            }
            set
            {
                Version = new Version(value);
            }
        }

        [XmlElement("autoLoadFile")]
        public Reference AutoLoadFile { get; set; }

        [XmlArray("variableFiles")]
        public List<Reference> VariableFiles { get; set; }

        [XmlIgnore]
        public Version MinimumRainmeter { get; set; }

        [XmlElement("minimumRainmeter")]
        public string MinimumRainmeterString
        {
            get
            {
                return MinimumRainmeter.ToString();
            }
            set
            {
                MinimumRainmeter = new Version(value);
            }
        }

        [XmlIgnore]
        public Version MinimumWindows { get; set; }

        [XmlElement("minimumWindows")]
        public string MinimumWindowsString
        {
            get
            {
                return MinimumWindows.ToString();
            }
            set
            {
                MinimumWindows = new Version(value);
            }
        }

        [XmlElement("root")]
        public Tree<Reference> Root { get; set; }

        public Project()
        {
            Root = new Tree<Reference>();
            VariableFiles = new List<Reference>();
            Version = new Version();
            MinimumRainmeter = new Version("3.1");
            MinimumWindows = new Version("5.1");
        }

        public override bool Equals(object obj)
        {
            Project other = obj as Project;
            
            if (other == null)
                return false;
            
            bool res = String.Equals(Author, other.Author);
            res &= Reference.Equals(AutoLoadFile, other.AutoLoadFile);
            res &= Version.Equals(MinimumRainmeter, other.MinimumRainmeter);
            res &= Version.Equals(MinimumWindows, other.MinimumWindows);
            res &= String.Equals(Name, other.Name);
            res &= Tree<Reference>.Equals(Root, other.Root);
            res &= Version.Equals(Version, other.Version);

            return res;
        }

        public override int GetHashCode()
        {
            int hash = (Author == null) ? 0 : Author.GetHashCode();
            hash = hash * 7 + ((AutoLoadFile == null) ? 0 : AutoLoadFile.GetHashCode());
            hash = hash * 7 + ((MinimumRainmeter == null) ? 0 : MinimumRainmeter.GetHashCode());
            hash = hash * 7 + ((MinimumWindows == null) ? 0 : MinimumWindows.GetHashCode());
            hash = hash * 7 + ((Name == null) ? 0 : Name.GetHashCode());
            hash = hash * 7 + ((Root == null) ? 0 : Root.GetHashCode());
            hash = hash * 7 + ((Version == null) ? 0 : Version.GetHashCode());

            return hash;
        }
    }
}
