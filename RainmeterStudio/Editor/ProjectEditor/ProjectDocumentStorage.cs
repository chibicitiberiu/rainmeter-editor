using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Storage;
using RainmeterStudio.Storage;

namespace RainmeterStudio.Editor.ProjectEditor
{
    /// <summary>
    /// Project storage, loads and saves project files
    /// </summary>
    [PluginExport]
    public class ProjectDocumentStorage : IDocumentStorage
    {
        /// <summary>
        /// Reads the project as a ProjectDocument.
        /// Use Load to get only the Project.
        /// </summary>
        /// <param name="path">Path to project file</param>
        /// <returns>A project document</returns>
        public IDocument ReadDocument(string path)
        {
            Project project = ProjectStorage.Read(path);
            var document = new ProjectDocument(project);
            document.Reference = new Reference(Path.GetFileName(path), path, ReferenceTargetKind.Project);

            return document;
        }

        /// <summary>
        /// Writes a project document to file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="document"></param>
        public void WriteDocument(IDocument document, string path)
        {
            var projectDocument = (ProjectDocument)document;
            ProjectStorage.Write(projectDocument.Project, path);
        }

        /// <summary>
        /// Returns true if the file is a project storage
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>True if the file can be read by this storage</returns>
        public bool CanReadDocument(string path)
        {
            return (Path.GetExtension(path) == ".rsproj");
        }

        /// <summary>
        /// Returns true if this can write specified document type
        /// </summary>
        /// <param name="documentType">Document type</param>
        /// <returns>True if document can be written by this storage</returns>
        public bool CanWriteDocument(Type documentType)
        {
            return documentType.Equals(typeof(ProjectDocument));
        }
    }
}
