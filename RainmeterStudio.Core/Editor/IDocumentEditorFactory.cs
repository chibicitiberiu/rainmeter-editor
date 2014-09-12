using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Core.Editor
{
    public interface IDocumentEditorFactory
    {
        /// <summary>
        /// Creates a new editor object
        /// </summary>
        /// <param name="document">Document to be edited by the editor</param>
        /// <returns>A new document editor</returns>
        IDocumentEditor CreateEditor(IDocument document);

        /// <summary>
        /// Tests if this editor can edit this document type
        /// </summary>
        /// <param name="type">Document type</param>
        /// <returns>True if the editor can edit the document type</returns>
        bool CanEdit(Type type);
    }
}
