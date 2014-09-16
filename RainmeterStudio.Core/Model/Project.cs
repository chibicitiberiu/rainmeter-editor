using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core.Storage;
using RainmeterStudio.Core.Utils;
using Version = RainmeterStudio.Core.Utils.Version;

namespace RainmeterStudio.Core.Model
{
    /// <summary>
    /// Defines a Rainmeter Studio project
    /// </summary>
    public class Project : INotifyPropertyChanged
    {
        private string _author;
        private Reference _autoLoadFile;
        private Version _version, _minimumWindows, _minimumRainmeter;
        private Reference _root;

        #region Properties

        /// <summary>
        /// Gets or sets the name of the project
        /// </summary>
        [XmlElement(ElementName = "name", Order = 1)]
        public string Name
        {
            get
            {
                return Root.Name;
            }
            set
            {
                Root.Name = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        /// <summary>
        /// Gets or sets the file path of this project
        /// </summary>
        [XmlIgnore]
        public string Path
        {
            get
            {
                return Root.StoragePath;
            }
            set
            {
                Root.StoragePath = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        /// <summary>
        /// Gets or sets the author of the project
        /// </summary>
        [XmlElement(ElementName = "author", Order = 2)]
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Author"));
            }
        }

        /// <summary>
        /// Gets or sets the version of the project
        /// </summary>
        [XmlElement(ElementName = "version", Order = 3)]
        public Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Version"));
            }
        }

        /// <summary>
        /// Gets or sets the reference to the file that automatically loads at package installation
        /// </summary>
        [XmlIgnore]
        public Reference AutoLoadFile
        {
            get
            {
                return _autoLoadFile;
            }
            set
            {
                _autoLoadFile = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("AutoLoadFile"));
            }
        }

        /// <summary>
        /// Gets or sets the qualified name of the auto load file
        /// </summary>
        [XmlElement(ElementName = "autoLoadFile", Order = 7)]
        public string AutoLoadFileQualifiedName
        {
            get
            {
                return ((AutoLoadFile == null) ? null : AutoLoadFile.QualifiedName);
            }
            set
            {
                AutoLoadFile = Root.GetReference(value);
            }
        }

        /// <summary>
        /// Gets or sets the list of variable files
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<Reference> VariableFiles { get; private set; }

        /// <summary>
        /// Gets or sets the list of variable files qualified names
        /// </summary>
        [XmlArray(ElementName = "variableFiles", Order = 8)]
        public string[] VariableFilesQualifiedNames
        {
            get
            {
                return VariableFiles.Select(x => x.QualifiedName).ToArray();
            }
            set
            {
                VariableFiles.Clear();
                value.Select(x => Root.GetReference(x)).ForEach(VariableFiles.Add);
            }
        }

        /// <summary>
        /// Gets or sets the minimum rainmeter version
        /// </summary>
        [XmlElement(ElementName = "minimumRainmeter", Order = 4)]
        public Version MinimumRainmeter
        {
            get
            {
                return _minimumRainmeter;
            }
            set
            {
                _minimumRainmeter = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("MinimumRainmeter"));
            }
        }

        /// <summary>
        /// Gets or sets the minimum Windows version
        /// </summary>
        [XmlElement(ElementName = "minimumWindows", Order = 5)]
        public Version MinimumWindows
        {
            get
            {
                return _minimumWindows;
            }
            set
            {
                _minimumWindows = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("MinimumWindows"));
            }
        }

        /// <summary>
        /// Gets or sets the root node
        /// </summary>
        [XmlElement(ElementName = "root", Order = 6)]
        public Reference Root
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Root"));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a project
        /// </summary>
        public Project()
        {
            Root = new Reference(String.Empty, ReferenceTargetKind.Project);
            VariableFiles = new ObservableCollection<Reference>();
            Version = new Version();
            MinimumRainmeter = new Version("3.1");
            MinimumWindows = new Version("5.1");
        }

        #endregion


        #region Operations

        /// <summary>
        /// Looks for reference in project
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <returns>True if reference was found</returns>
        public bool Contains(Reference reference)
        {
            return Root.TreeContains(reference);
        }

        #endregion

        /// <summary>
        /// Triggered when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
