using System;
using System.IO;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Storage;

namespace RainmeterStudio.TextEditorPlugin
{
    /// <summary>
    /// Storage for text files
    /// </summary>    
    [PluginExport]
    public class TextStorage : IDocumentStorage
    {
        /// <inheritdoc />
        public IDocument ReadDocument(string path)
        {
            TextDocument document = new TextDocument();
            document.Reference = new Reference(Path.GetFileName(path), path);
            document.Text = File.ReadAllText(path);

            return document;
        }

        /// <inheritdoc />
        public void WriteDocument(IDocument document, string path)
        {
            TextDocument textDocument = document as TextDocument;

            if (textDocument == null)
                throw new ArgumentException("Provided document is not supported by this storage.");

            File.WriteAllText(path, textDocument.Text);
        }

        /// <inheritdoc />
        public bool CanReadDocument(string path)
        {
            // Open the file
            FileStream file = File.OpenRead(path);

            // Read a small chunk (1kb should be enough)
            byte[] buffer = new byte[1024];
            int read = file.Read(buffer, 0, buffer.Length);

            // Close file
            file.Close();

            // Find 4 consecutive zero bytes
            int cnt = 0;
            for (int i = 0; i < read; i++)
            {
                // Count zero bytes
                if (buffer[i] == 0)
                    ++cnt;
                else cnt = 0;

                // Found > 4? Most likely binary file
                if (cnt >= 4)
                    return false;
            }

            // Not found, probably text file
            return true;
        }

        public bool CanWriteDocument(Type documentType)
        {
            return documentType.Equals(typeof(TextDocument));
        }
    }
}
