using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Storage;

namespace RainmeterStudio.Editor.ProjectEditor
{
    /// <summary>
    /// Project storage, loads and saves project files
    /// </summary>
    [PluginExport]
    public class ProjectStorage : IDocumentStorage
    {
        /// <summary>
        /// Loads a project from file
        /// </summary>
        /// <param name="path">Path to file to load</param>
        /// <returns>Loaded project</returns>
        public Project Read(string path)
        {
            // Open file
            var file = File.OpenText(path);

            // Deserialize file
            var serializer = new XmlSerializer(typeof(Project), new XmlRootAttribute("project"));
            Project project = serializer.Deserialize(file) as Project;

            if (project != null)
            {
                project.Path = path;
            }

            // Clean up
            file.Close();
            return project;
        }

        /// <summary>
        /// Saves a project to file
        /// </summary>
        /// <param name="project">Project to save</param>
        /// <param name="path">File to save to</param>
        public void Write(Project project, string path)
        {
            // Open file
            var file = File.OpenWrite(path);

            // Serialize file
            var serializer = new XmlSerializer(typeof(Project), new XmlRootAttribute("project"));
            serializer.Serialize(file, project);

            // Clean up
            file.Close();
            project.Path = path;
        }

        /// <summary>
        /// Saves a project
        /// </summary>
        /// <param name="project">Saves a project to the path specified in the 'Path' property</param>
        public void Write(Project project)
        {
            Write(project, project.Path);
        }

        /// <summary>
        /// Reads the project as a ProjectDocument.
        /// Use Load to get only the Project.
        /// </summary>
        /// <param name="path">Path to project file</param>
        /// <returns>A project document</returns>
        public IDocument ReadDocument(string path)
        {
            Project project = Read(path);
            var document = new ProjectDocument(project);
            document.Reference = new Reference(Path.GetFileName(path), path);

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
            Write(projectDocument.Project, path);
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
