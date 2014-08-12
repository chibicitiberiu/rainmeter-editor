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
        IDocument IDocumentStorage.Read(string path)
        {
            TextDocument document = new TextDocument();
            document.Reference.Path = path;
            document.Reference.Name = Path.GetFileName(path);
            document.Lines.AddRange(File.ReadAllLines(path));

            return document;
        }

        /// <inheritdoc />
        public void Write(string path, IDocument document)
        {
            TextDocument textDocument = document as TextDocument;

            if (textDocument == null)
                throw new ArgumentException("Provided document is not supported by this storage.");

            File.WriteAllLines(path, textDocument.Lines);
        }

        /// <inheritdoc />
        public bool CanRead(string path)
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

        public bool CanWrite(Type documentType)
        {
            return documentType.Equals(typeof(TextDocument));
        }
    }
}
