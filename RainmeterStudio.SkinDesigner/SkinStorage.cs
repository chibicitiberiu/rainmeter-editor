using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Storage;

namespace RainmeterStudio.SkinDesignerPlugin
{
    public class SkinStorage : IDocumentStorage
    {
        public IDocument ReadDocument(string path)
        {
            throw new NotImplementedException();
        }

        public void WriteDocument(IDocument document, string path)
        {
            throw new NotImplementedException();
        }

        public bool CanReadDocument(string path)
        {
            throw new NotImplementedException();
        }

        public bool CanWriteDocument(Type documentType)
        {
            throw new NotImplementedException();
        }
    }
}
