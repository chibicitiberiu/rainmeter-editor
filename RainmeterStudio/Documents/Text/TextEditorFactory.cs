using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RainmeterStudio.Business;
using RainmeterStudio.Model;

namespace RainmeterStudio.Documents.Text
{
    [AutoRegister]
    public class TextEditorFactory : IDocumentEditorFactory
    {
        public IDocumentEditor CreateEditor(IDocument document)
        {
            return new TextEditor((TextDocument)document);
        }

        public bool CanEdit(Type type)
        {
            return type.Equals(typeof(TextDocument));
        }
    }
}
