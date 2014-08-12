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
    /// Skin designer factory
    /// </summary>
    [PluginExport]
    public class SkinDesignerFactory : IDocumentEditorFactory
    {
        /// <summary>
        /// Creates a new editor object
        /// </summary>
        /// <param name="document">Document to be edited by the editor</param>
        /// <returns>A new document editor</returns>
        public IDocumentEditor CreateEditor(IDocument document)
        {
            return new SkinDesigner(document as SkinDocument);
        }

        /// <summary>
        /// Tests if this editor can edit this document type
        /// </summary>
        /// <param name="type">Document type</param>
        /// <returns>True if the editor can edit the document type</returns>
        public bool CanEdit(Type type)
        {
            return (type == typeof(SkinDocument));
        }
    }
}
