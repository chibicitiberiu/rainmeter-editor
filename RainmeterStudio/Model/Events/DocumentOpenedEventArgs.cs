using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Documents;

namespace RainmeterStudio.Model.Events
{
    public abstract class DocumentEventArgsBase : EventArgs
    {
        /// <summary>
        /// Gets the newly created document editor
        /// </summary>
        public IDocumentEditor Editor { get; private set; }

        /// <summary>
        /// Gets the opened document
        /// </summary>
        public IDocument Document { get { return Editor.AttachedDocument; } }

        /// <summary>
        /// Initializes the DocumentOpenedEventArgs
        /// </summary>
        /// <param name="editor">The document editor</param>
        public DocumentEventArgsBase(IDocumentEditor editor)
        {
            Editor = editor;
        }
    }

    /// <summary>
    /// Event arguments for the document opened event
    /// </summary>
    public class DocumentOpenedEventArgs : DocumentEventArgsBase
    {
        /// <summary>
        /// Initializes the DocumentOpenedEventArgs
        /// </summary>
        /// <param name="editor">The document editor</param>
        public DocumentOpenedEventArgs(IDocumentEditor editor)
            : base(editor)
        {
        }
    }

    /// <summary>
    /// Event arguments for the document closed event
    /// </summary>
    public class DocumentClosedEventArgs : DocumentEventArgsBase
    {
        /// <summary>
        /// Initializes the DocumentClosedEventArgs
        /// </summary>
        /// <param name="editor">The document editor</param>
        public DocumentClosedEventArgs(IDocumentEditor editor)
            : base(editor)
        {
        }
    }
}
