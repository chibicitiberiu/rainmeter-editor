using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.TextEditorPlugin
{
    [PluginExport]
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
