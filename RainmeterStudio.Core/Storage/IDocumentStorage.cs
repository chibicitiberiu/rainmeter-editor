using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Core.Storage
{
    public interface IDocumentStorage
    {
        /// <summary>
        /// Reads a document from file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Read document</returns>
        IDocument ReadDocument(string path);

        /// <summary>
        /// Writes a document to a file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="document">Document to write</param>
        void WriteDocument(IDocument document, string path);

        /// <summary>
        /// Tests if the file can be read by this storage
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>True if file can be read</returns>
        bool CanReadDocument(string path);

        /// <summary>
        /// Tests if the document can be written by this storage
        /// </summary>
        /// <param name="documentType">Document type</param>
        /// <returns>True if the document can be written</returns>
        bool CanWriteDocument(Type documentType);
    }
}
