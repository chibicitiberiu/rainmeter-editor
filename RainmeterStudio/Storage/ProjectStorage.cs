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
    public class ProjectStorage
    {
        public Project Load(string path)
        {
            // Open file
            var file = File.OpenText(path);

            // Deserialize file
            var serializer = new XmlSerializer(typeof(SerializableProject), new XmlRootAttribute("project"));
            SerializableProject project = serializer.Deserialize(file) as SerializableProject;
            if (project != null)
                project.Path = path;

            // Clean up
            file.Close();
            return project.Project;
        }

        public void Save(string path, Project project)
        {
            // Open file
            var file = File.OpenWrite(path);

            // Serialize file
            var sProject = new SerializableProject(project);
            var serializer = new XmlSerializer(typeof(SerializableProject), new XmlRootAttribute("project"));
            serializer.Serialize(file, sProject);

            // Clean up
            file.Close();
            project.Path = path;
        }
    }
}
