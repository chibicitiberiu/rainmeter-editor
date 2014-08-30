using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core.Storage;

namespace RainmeterStudio.Core.Model
{
    /// <summary>
    /// Defines a Rainmeter Studio project
    /// </summary>
    public class Project
    {
        #region Private fields

        private string _name;
        private string _path;
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the project
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                if (Root != null)
                    Root.Data = new Reference(Name, Path);
            }
        }

        /// <summary>
        /// Gets or sets the file path of this project
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;

                if (Root != null)
                    Root.Data = new Reference(Name, Path);
            }
        }

        /// <summary>
        /// Gets or sets the author of the project
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the version of the project
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the reference to the file to automatically load at package installation
        /// </summary>
        public Reference AutoLoadFile { get; set; }

        /// <summary>
        /// Gets or sets the list of variable files
        /// </summary>
        public List<Reference> VariableFiles { get; set; }

        /// <summary>
        /// Gets or sets the minimum rainmeter version
        /// </summary>
        public Version MinimumRainmeter { get; set; }

        /// <summary>
        /// Gets or sets the minimum Windows version
        /// </summary>
        public Version MinimumWindows { get; set; }

        /// <summary>
        /// Gets or sets the root node
        /// </summary>
        public Tree<Reference> Root { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a project
        /// </summary>
        public Project()
        {
            Root = new Tree<Reference>();
            VariableFiles = new List<Reference>();
            Version = new Version();
            MinimumRainmeter = new Version("3.1");
            MinimumWindows = new Version("5.1");
        }

        #endregion
    }
}
