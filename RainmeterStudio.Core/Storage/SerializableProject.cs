using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Utils;

namespace RainmeterStudio.Core.Storage
{
    /// <summary>
    /// Helper class to serialize or deserialize project objects
    /// </summary>
    public class SerializableProject
    {
        private Project _project;
        private SerializableReference _autoLoadFile;
        private List<SerializableReference> _variableFiles;
        private SerializableTree<SerializableReference> _root;

        /// <summary>
        /// Gets or sets the project
        /// </summary>
        [XmlIgnore]
        public Project Project
        {
            get
            {
                _project.AutoLoadFile = _autoLoadFile.Reference;
                _project.VariableFiles = _variableFiles.Select(x => x.Reference).ToList();
                _project.Root = _root.AsTree().TransformData(x => x.Reference);
                return _project;
            }
            set
            {
                _project = value;
                UpdateSelf();
            }
        }

        /// <summary>
        /// Gets or sets the project name
        /// </summary>
        [XmlElement("name")]
        public string Name
        {
            get
            {
                return Project.Name;
            }
            set
            {
                Project.Name = value;
                UpdateSelf();
            }
        }

        /// <summary>
        /// Gets or sets the project path
        /// </summary>
        [XmlIgnore]
        public string Path
        {
            get
            {
                return Project.Path;
            }
            set
            {
                Project.Path = value;
                UpdateSelf();
            }
        }

        /// <summary>
        /// Gets or sets the author of the project
        /// </summary>
        [XmlElement("author")]
        public string Author
        {
            get
            {
                return Project.Author;
            }
            set
            {
                Project.Author = value;
                UpdateSelf();
            }
        }

        /// <summary>
        /// Gets or sets the project version
        /// </summary>
        [XmlElement("version")]
        public string Version
        {
            get
            {
                return Project.Version.ToString();
            }
            set
            {
                Project.Version = new Version(value);
                UpdateSelf();
            }
        }

        /// <summary>
        /// Gets or sets the reference to the file to automatically load at package installation
        /// </summary>
        [XmlElement("autoLoadFile")]
        public SerializableReference AutoLoadFile
        {
            get
            {
                return _autoLoadFile;
            }
            set
            {
                _autoLoadFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of variable files
        /// </summary>
        [XmlArray("variableFiles")]
        public List<SerializableReference> VariableFiles
        {
            get
            {
                return _variableFiles;
            }
            set
            {
                _variableFiles = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum rainmeter version
        /// </summary>
        public string MinimumRainmeter
        {
            get
            {
                return Project.MinimumRainmeter.ToString();
            }
            set
            {
                Project.MinimumRainmeter = new Version(value);
                UpdateSelf();
            }
        }

        /// <summary>
        /// Gets or sets the minimum Windows version
        /// </summary>
        public string MinimumWindows
        {
            get
            {
                return Project.MinimumWindows.ToString();
            }
            set
            {
                Project.MinimumWindows = new Version(value);
                UpdateSelf();
            }
        }

        /// <summary>
        /// Gets or sets the root node
        /// </summary>
        public SerializableTree<SerializableReference> Root
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
            }
        }

        /// <summary>
        /// Initializes the serializable project
        /// </summary>
        public SerializableProject()
        {
            Project = new Project();
        }

        /// <summary>
        /// Initializes the serializable project
        /// </summary>
        /// <param name="project">Base project</param>
        public SerializableProject(Project project)
        {
            Project = project;
        }

        private void UpdateSelf()
        {
            _autoLoadFile = new SerializableReference(_project.AutoLoadFile);
            _variableFiles = _project.VariableFiles.Select(x => new SerializableReference(x)).ToList();
            _root = _project.Root.TransformData(x => new SerializableReference(x)).AsSerializableTree();
        }
    }
}
