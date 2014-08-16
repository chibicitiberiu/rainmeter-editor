using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core.Model;

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
        [XmlElement("name")]
        public string Name
        {
            get
            {
                if (Reference != null)
                    return Reference.Name;

                return null;
            }
            set
            {
                Reference = new Reference(value, Path);
            }
        }

        /// <summary>
        /// Gets or sets the path of the reference
        /// </summary>
        [XmlElement("path")]
        public string Path
        {
            get
            {
                if (Reference != null)
                    return Reference.Path;

                return null;
            }
            set
            {
                Reference = new Reference(Name, value);
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

        public SerializableReference(Reference reference)
        {
            Reference = reference;
        }
    }
}
