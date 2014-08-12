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
        public IDocument Read(string path)
        {
            throw new NotImplementedException();
        }

        public void Write(string path, IDocument document)
        {
            throw new NotImplementedException();
        }

        public bool CanRead(string path)
        {
            throw new NotImplementedException();
        }

        public bool CanWrite(Type documentType)
        {
            throw new NotImplementedException();
        }
    }
}
