using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterEditor.Model;
using RainmeterEditor.Model.Events;

namespace RainmeterEditor.Business
{
    public class DocumentManager
    {
        #region Singleton instance
        private static DocumentManager _instance = null;

        /// <summary>
        /// Gets the instance of DocumentManager
        /// </summary>
        public static DocumentManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DocumentManager();

                return _instance;
            }
        }

        #endregion

        private DocumentManager()
        {
        }

        List<IDocumentEditorFactory> _factories = new List<IDocumentEditorFactory>();
        List<IDocumentEditor> _editors = new List<IDocumentEditor>();

        public event EventHandler<DocumentOpenedEventArgs> DocumentOpened;

        /// <summary>
        /// Registers a document editor factory
        /// </summary>
        /// <param name="factory">Document editor factory</param>
        public void RegisterEditorFactory(IDocumentEditorFactory factory)
        {
            _factories.Add(factory);
        }

        /// <summary>
        /// Creates a new document in the specified path, with the specified format, and opens it
        /// </summary>
        /// <param name="format"></param>
        /// <param name="path"></param>
        public void Create(DocumentFormat format, string path)
        {
            // Create document
            var document = format.Factory.CreateDocument(format, path);

            // Create editor
            var editor = format.Factory.CreateEditor(document);
            _editors.Add(editor);

            // Trigger event
            if (DocumentOpened != null)
                DocumentOpened(this, new DocumentOpenedEventArgs(editor));
        }

        public IEnumerable<DocumentFormat> DocumentFormats
        {
            get
            {
                foreach (var f in _factories)
                    foreach (var df in f.CreateDocumentFormats)
                        yield return df;
            }
        }
    }
}
