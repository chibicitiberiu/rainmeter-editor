using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.SkinDesignerPlugin
{
    /// <summary>
    /// Template of a skin which will be opened in the designer
    /// </summary>
    [PluginExport]
    public class SkinDocumentTemplate : IDocumentTemplate
    {
        /// <summary>
        /// Gets the document template name
        /// </summary>
        public string Name
        {
            get { return "Skin"; }
        }

        /// <summary>
        /// Gets the default extension of this template
        /// </summary>
        public string DefaultExtension
        {
            get { return "rsskin"; }
        }

        /// <summary>
        /// Gets or sets the properties of this template
        /// </summary>
        public IEnumerable<Property> Properties
        {
            get { return Enumerable.Empty<Property>(); }
        }

        /// <summary>
        /// Creates a new document using this template
        /// </summary>
        /// <returns>Newly created document</returns>
        public IDocument CreateDocument()
        {
            return new SkinDocument();
        }
    }
}
