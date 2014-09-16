using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Storage
{
    /// <summary>
    /// Reads or writes projects
    /// </summary>
    public static class ProjectStorage
    {
        /// <summary>
        /// Loads a project from file
        /// </summary>
        /// <param name="path">Path to file to load</param>
        /// <returns>Loaded project</returns>
        public static Project Read(string path)
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
        /// Saves a project
        /// </summary>
        /// <param name="project">Saves a project to the path specified in the 'Path' property</param>
        public static void Write(Project project)
        {
            Write(project, project.Path);
        }

        /// <summary>
        /// Saves a project to file
        /// </summary>
        /// <param name="project">Project to save</param>
        /// <param name="path">File to save to</param>
        public static void Write(Project project, string path)
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
    }
}
