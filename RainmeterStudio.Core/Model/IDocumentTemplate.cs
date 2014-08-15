using System.Collections.Generic;
using System.Linq;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Core.Model
{
    /// <summary>
    /// Represents a document template
    /// </summary>
    public interface IDocumentTemplate
    {
        /// <summary>
        /// Gets the document template name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the default extension of this template
        /// </summary>
        string DefaultExtension { get; }

        /// <summary>
        /// Gets or sets the properties of this template
        /// </summary>
        /// <remarks>Properties are used to display a form dialog after the "New item" dialog closes.</remarks>
        IEnumerable<Property> Properties { get; }

        /// <summary>
        /// Creates a document using this template
        /// </summary>
        /// <returns>Created document.</returns>
        IDocument CreateDocument();
    }
}
