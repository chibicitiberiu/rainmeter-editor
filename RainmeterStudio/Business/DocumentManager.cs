using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Model.Events;
using RainmeterStudio.Core.Storage;

namespace RainmeterStudio.Business
{
    /// <summary>
    /// Document manager
    /// </summary>
    public class DocumentManager
    {
        #region Events

        /// <summary>
        /// Triggered when a document is opened
        /// </summary>
        public event EventHandler<DocumentOpenedEventArgs> DocumentOpened;

        /// <summary>
        /// Triggered when a document is closed
        /// </summary>
        public event EventHandler<DocumentClosedEventArgs> DocumentClosed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of factories
        /// </summary>
        public IEnumerable<IDocumentEditorFactory> Factories { get { return _factories; } }

        /// <summary>
        /// Gets a list of editors
        /// </summary>
        public IEnumerable<IDocumentEditor> Editors { get { return _editors; } }

        /// <summary>
        /// Gets a list of storages
        /// </summary>
        public IEnumerable<IDocumentStorage> Storages { get { return _storages; } }

        /// <summary>
        /// Gets a list of document templates
        /// </summary>
        public IEnumerable<IDocumentTemplate> DocumentTemplates { get { return _templates; } }

        #endregion

        #region Private fields

        private List<IDocumentEditorFactory> _factories = new List<IDocumentEditorFactory>();
        private List<IDocumentEditor> _editors = new List<IDocumentEditor>();
        private List<IDocumentStorage> _storages = new List<IDocumentStorage>();
        private List<IDocumentTemplate> _templates = new List<IDocumentTemplate>();

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes this document manager
        /// </summary>
        public DocumentManager()
        {
        }

        /// <summary>
        /// Registers a document editor factory
        /// </summary>
        /// <param name="factory">Document editor factory</param>
        public void RegisterEditorFactory(IDocumentEditorFactory factory)
        {
            _factories.Add(factory);
        }

        /// <summary>
        /// Registers a document storage
        /// </summary>
        /// <param name="storage">The storage</param>
        public void RegisterStorage(IDocumentStorage storage)
        {
            _storages.Add(storage);
        }

        /// <summary>
        /// Registers a document template
        /// </summary>
        /// <param name="template">The document template</param>
        public void RegisterTemplate(IDocumentTemplate template)
        {
            _templates.Add(template);
        }

        #endregion

        #region Document operations

        /// <summary>
        /// Creates a new document in the specified path, with the specified format, and opens it
        /// </summary>
        /// <param name="format"></param>
        /// <param name="path"></param>
        public IDocumentEditor Create(IDocumentTemplate format)
        {
            // Create document
            var document = format.CreateDocument();
            document.IsDirty = true;

            // Find and create editor
            IDocumentEditor editor = CreateEditor(document);

            // Trigger event
            if (DocumentOpened != null)
                DocumentOpened(this, new DocumentOpenedEventArgs(editor));

            return editor;
        }

        /// <summary>
        /// Opens the specified document
        /// </summary>
        /// <param name="path">The path to the file to open</param>
        public IDocumentEditor Open(string path)
        {
            // Try to open
            IDocument document = Read(path);

            // Create factory
            var editor = CreateEditor(document);

            // Trigger event
            if (DocumentOpened != null)
                DocumentOpened(this, new DocumentOpenedEventArgs(editor));

            return editor;
        }

        /// <summary>
        /// Saves a document
        /// </summary>
        /// <param name="document">The document</param>
        public void Save(IDocument document)
        {
            // Find a storage
            var storage = FindStorage(document);

            if (document.Reference.StoragePath == null)
                throw new ArgumentException("Reference cannot be empty");

            // Save
            storage.WriteDocument(document, document.Reference.StoragePath);

            // Clear dirty flag
            document.IsDirty = false;
        }

        /// <summary>
        /// Saves the document as
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="document">Document</param>
        public void SaveAs(string path, IDocument document)
        {
            // Find a storage
            var storage = FindStorage(document);

            // Save
            storage.WriteDocument(document, path);

            // Update reference
            document.Reference = new Reference(Path.GetFileName(path), path);

            // Clear dirty flag
            document.IsDirty = false;
        }

        /// <summary>
        /// Saves a copy of the document
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="document">Document</param>
        public void SaveACopy(string path, IDocument document)
        {
            // Find a storage
            var storage = FindStorage(document);

            // Save
            storage.WriteDocument(document, path);
        }

        /// <summary>
        /// Closes a document editor
        /// </summary>
        /// <param name="editor"></param>
        public void Close(IDocumentEditor editor)
        {
            // Remove from list of opened editors
            _editors.Remove(editor);

            // Close event
            if (DocumentClosed != null)
                DocumentClosed(this, new DocumentClosedEventArgs(editor));
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Attempts to create an editor for the document
        /// </summary>
        /// <param name="document">The document</param>
        /// <exception cref="ArgumentException">Thrown if failed to create editor</exception>
        /// <returns>The editor</returns>
        private IDocumentEditor CreateEditor(IDocument document)
        {
            IDocumentEditor editor = null;

            foreach (var factory in Factories)
                if (factory.CanEdit(document.GetType()))
                {
                    editor = factory.CreateEditor(document);
                    break;
                }

            if (editor == null)
                throw new ArgumentException("Failed to create editor.");

            _editors.Add(editor);
            return editor;
        }

        /// <summary>
        /// Attempts to read a document
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <exception cref="ArgumentException">Thrown when failed to open the document</exception>
        /// <returns></returns>
        private IDocument Read(string path)
        {
            IDocument document = null;

            foreach (var storage in Storages)
                if (storage.CanReadDocument(path))
                {
                    document = storage.ReadDocument(path);
                    break;
                }

            // Failed to open
            if (document == null)
                throw new ArgumentException("Failed to open document.");

            return document;
        }

        /// <summary>
        /// Attempts to find a storage for the specified document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private IDocumentStorage FindStorage(IDocument document)
        {
            IDocumentStorage storage = null;

            foreach (var s in Storages)
                if (s.CanWriteDocument(document.GetType()))
                {
                    storage = s;
                    break;
                }

            if (storage == null)
                throw new ArgumentException("Failed to find storage object.");

            return storage;
        }

        #endregion
    }
}
