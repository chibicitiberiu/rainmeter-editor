using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RainmeterEditor.Model;

namespace RainmeterEditor.Documents.Text
{
    public class TextDocument : IDocument
    {
        public string Name
        {
            get
            {
                return Path.GetFileName(FilePath);
            }
        }

        public string FilePath
        {
            get; set;
        }

        public string Text
        {
            get; set;
        }

        public TextDocument()
        {
            Text = String.Empty;
        }
    }
}
