﻿using System.Collections.Generic;
using System.Linq;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Core.Documents
{
    /// <summary>
    /// Represents a document template
    /// </summary>
    public abstract class DocumentTemplate
    {
        /// <summary>
        /// Gets the document template name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the default extension of this template
        /// </summary>
        public string DefaultExtension { get; private set; }

        /// <summary>
        /// Gets or sets the properties of this template
        /// </summary>
        /// <remarks>Properties are used to display a form dialog after the "New item" dialog closes.</remarks>
        public virtual IEnumerable<Property> Properties
        {
            get
            {
                return Enumerable.Empty<Property>();
            }
        }

        /// <summary>
        /// Initializes the document template
        /// </summary>
        /// <param name="name">Name of template</param>
        /// <param name="defaultExtension">Default extension</param>
        public DocumentTemplate(string name, string defaultExtension)
        {
            Name = name;
            DefaultExtension = defaultExtension;
        }

        /// <summary>
        /// Creates a document using this template
        /// </summary>
        /// <returns></returns>
        public abstract IDocument CreateDocument();
    }
}
