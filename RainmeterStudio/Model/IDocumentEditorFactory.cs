using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Storage;

namespace RainmeterStudio.Model
{
    public interface IDocumentEditorFactory
    {
        /// <summary>
        /// Name of the editor
        /// </summary>
        string EditorName { get; }

        /// <summary>
        /// Formats that will be used to populate the 'create document' dialog
        /// </summary>
        IEnumerable<DocumentTemplate> CreateDocumentFormats { get; }

        /// <summary>
        /// Creates a new editor object
        /// </summary>
        /// <param name="document">Document to be edited by the editor</param>
        /// <returns>A new document editor</returns>
        IDocumentEditor CreateEditor(IDocument document);

        /// <summary>
        /// Creates a new document
        /// </summary>
        /// <returns>A new document</returns>
        IDocument CreateDocument(DocumentTemplate format, string path);

        /// <summary>
        /// Gets the storage of this factory
        /// </summary>
        IDocumentStorage Storage { get; }
    }
}
