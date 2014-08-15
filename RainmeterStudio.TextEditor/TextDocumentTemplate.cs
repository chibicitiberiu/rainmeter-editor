using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.TextEditorPlugin
{
    /// <summary>
    /// A blank text document template
    /// </summary>
    [PluginExport]
    public class TextDocumentTemplate : DocumentTemplate
    {
        public TextDocumentTemplate()
            : base("Text", "txt")
        {
        }

        public override IDocument CreateDocument()
        {
            return new TextDocument();
        }
    }
}
