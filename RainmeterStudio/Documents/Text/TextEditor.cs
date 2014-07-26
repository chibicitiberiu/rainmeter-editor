using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RainmeterStudio.Model;

namespace RainmeterStudio.Documents.Text
{
    public class TextEditor : IDocumentEditor
    {
        private TextDocument _document;
        private TextEditorControl _control;

        public TextEditor(TextDocument document)
        {
            _document = document;
            _control = new TextEditorControl(document);
        }

        public override IDocument Document { get { return _document; } }

        public override string Title { get { return _document.Name; } }

        public override System.Windows.UIElement EditorUI { get { return _control; } }
    }
}
