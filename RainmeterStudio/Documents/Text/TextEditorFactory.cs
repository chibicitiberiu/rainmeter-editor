using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RainmeterStudio.Business;
using RainmeterStudio.Model;

namespace RainmeterStudio.Documents.Text
{
    public class TextEditorFactory : IDocumentEditorFactory
    {
        private TextStorage _storage = new TextStorage();

        /// <inheritdoc />
        public string EditorName
        {
            get { return Resources.Strings.DocumentEditor_Text_Name; }
        }

        /// <inheritdoc />
        public IEnumerable<DocumentTemplate> CreateDocumentFormats
        {
            get 
            {
                yield return new DocumentTemplate()
                {
                    Name = Resources.Strings.DocumentFormat_TextFile_Name,
                    Category = Resources.Strings.Category_Utility,
                    DefaultExtension = ".txt",
                    Description = Resources.Strings.DocumentFormat_TextFile_Description,
                    Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Resources/Icons/text_file_32.png", UriKind.RelativeOrAbsolute)),
                    Factory = this
                };
            }
        }


        public IDocumentEditor CreateEditor(IDocument document)
        {
            TextDocument textDocument = document as TextDocument;

            if (textDocument == null)
                throw new ArgumentException("Cannot edit provided document.");

            return new TextEditor(textDocument);
        }

        public IDocumentStorage Storage { get { return _storage; } }

        public IDocument CreateDocument(DocumentTemplate format, string path)
        {
            var document = new TextDocument();
            document.FilePath = path;

            return document;
        }
    }
}
