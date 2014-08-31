using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Storage;

namespace RainmeterStudio.Storage
{
    /// <summary>
    /// Project storage, loads and saves project files
    /// </summary>
    public class ProjectStorage
    {
        /// <summary>
        /// Loads a project from file
        /// </summary>
        /// <param name="path">Path to file to load</param>
        /// <returns>Loaded project</returns>
        public Project Load(string path)
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
        public void Save(Project project, string path)
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
        public void Save(Project project)
        {
            Save(project, project.Path);
        }
    }
}
