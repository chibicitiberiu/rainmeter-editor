namespace RainmeterStudio.Model
{
    public interface IDocumentStorage
    {
        /// <summary>
        /// Reads a document from file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Read document</returns>
        IDocument Read(string path);

        /// <summary>
        /// Writes a document to a file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="document">Document to write</param>
        void Write(string path, IDocument document);

        /// <summary>
        /// Tests if the file can be read by this storage
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>True if file can be read</returns>
        bool CanRead(string path);
    }
}
