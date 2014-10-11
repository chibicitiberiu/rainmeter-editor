using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Storage;

namespace RainmeterStudio.Editor.SkinDesigner
{
    [PluginExport]
    public class SkinStorage : IDocumentStorage
    {
        public Core.Model.IDocument ReadDocument(string path)
        {
            return new SkinDocument();
        }

        public void WriteDocument(Core.Model.IDocument document, string path)
        {
        }

        public bool CanReadDocument(string path)
        {
            return Path.GetExtension(path) == ".rsskin";
        }

        public bool CanWriteDocument(Type documentType)
        {
            return documentType == typeof(SkinDocument);
        }
    }
}
