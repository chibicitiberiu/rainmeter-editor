using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Utils;

namespace RainmeterStudio.Core.Storage
{
    /// <summary>
    /// Represents a reference that can be serialized
    /// </summary>
    public class SerializableReference
    {
        /// <summary>
        /// Gets or sets the name of the reference
        /// </summary>
        [XmlElement("storagePath")]
        public string StoragePath
        {
            get
            {
                // Return only relative paths
                if (Path.IsPathRooted(Reference.StoragePath))
                {
                    return PathHelper.GetRelativePath(Reference.StoragePath);
                }

                return Reference.StoragePath;
            }
            set
            {
                Reference = new Reference(value, ProjectPath);
            }
        }

        /// <summary>
        /// Gets or sets the path of the reference
        /// </summary>
        [XmlElement("projectPath")]
        public string ProjectPath
        {
            get
            {
                return Reference.ProjectPath;
            }
            set
            {
                Reference = new Reference(StoragePath, value);
            }
        }

        /// <summary>
        /// Gets or sets the (immutable) reference
        /// </summary>
        [XmlIgnore]
        public Reference Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes this serializable reference
        /// </summary>
        public SerializableReference()
        {
        }

        /// <summary>
        /// Initializes this serializable reference
        /// </summary>
        /// <param name="reference">Reference to use</param>
        public SerializableReference(Reference reference)
        {
            Reference = reference;
        }
    }
}
