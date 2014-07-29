using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Model;

namespace RainmeterStudio.Documents.Text
{
    /// <summary>
    /// A blank text document template
    /// </summary>
    [AutoRegister]
    public class TextDocumentTemplate : DocumentTemplate
    {
        public TextDocumentTemplate()
            : base("TextDocument", "txt")
        {
        }

        public override IDocument CreateDocument()
        {
            return new TextDocument();
        }
    }
}
