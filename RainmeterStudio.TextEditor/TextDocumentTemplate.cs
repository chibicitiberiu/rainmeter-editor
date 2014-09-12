using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.TextEditorPlugin
{
    /// <summary>
    /// A blank text document template
    /// </summary>
    [PluginExport]
    public class TextDocumentTemplate : IDocumentTemplate
    {
        /// <summary>
        /// Gets the document template name
        /// </summary>
        public string Name
        {
            get { return "Text"; }
        }

        /// <summary>
        /// Gets the default extension of this template
        /// </summary>
        public string DefaultExtension
        {
            get { return "txt"; }
        }

        /// <summary>
        /// Gets or sets the properties of this template
        /// </summary>
        public IEnumerable<Property> Properties
        {
            get { return Enumerable.Empty<Property>(); }
        }

        /// <summary>
        /// Creates a document using this template
        /// </summary>
        /// <returns>Created document.</returns>
        public IDocument CreateDocument()
        {
            return new TextDocument();
        }
    }
}
