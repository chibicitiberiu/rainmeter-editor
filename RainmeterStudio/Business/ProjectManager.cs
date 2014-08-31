using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Storage;
using RainmeterStudio.Storage;

namespace RainmeterStudio.Business
{
    public class ProjectManager
    {
        private List<IProjectTemplate> _projectTemplates = new List<IProjectTemplate>();

        #region Properties

        /// <summary>
        /// Gets the currently opened project
        /// </summary>
        public Project ActiveProject { get; protected set; }

        /// <summary>
        /// Gets or sets the project storage
        /// </summary>
        protected ProjectStorage Storage { get; set; }

        #endregion

        #region Callbacks

        /// <summary>
        /// Called when a project is opened or the active project closes.
        /// </summary>
        public event EventHandler ActiveProjectChanged;

        #endregion

        /// <summary>
        /// Initializes the project manager
        /// </summary>
        /// <param name="storage">Project storage</param>
        public ProjectManager(ProjectStorage storage)
        {
            Storage = storage;
            ActiveProject = null;
        }
        
        /// <summary>
        /// Creates a new project
        /// </summary>
        /// <param name="name">Name of project</param>
        /// <param name="path">Path of project file</param>
        public void CreateProject(string name, string path, IProjectTemplate template)
        {
            // If there is an opened project, close it
            if (ActiveProject != null)
                Close();

            // Create project object
            ActiveProject = template.CreateProject();
            ActiveProject.Name = name;
            ActiveProject.Path = path;

            // Save to file
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            SaveActiveProject();

            // Raise event
            if (ActiveProjectChanged != null)
                ActiveProjectChanged(this, new EventArgs());
        }

        /// <summary>
        /// Opens a project from disk
        /// </summary>
        /// <param name="path"></param>
        public void OpenProject(string path) 
        {
            // If there is an opened project, close it
            if (ActiveProject != null)
                Close();

            // Open using storage
            ActiveProject = Storage.Load(path);
            ActiveProject.Path = path;

            // Raise event
            if (ActiveProjectChanged != null)
                ActiveProjectChanged(this, new EventArgs());
        }

        /// <summary>
        /// Saves the changes to the current project to disk
        /// </summary>
        public void SaveActiveProject()
        {
            // Safety check
            if (ActiveProject == null)
                throw new InvalidOperationException("Cannot save a project that is not opened.");

            // Save
            Storage.Save(ActiveProject);
        }

        /// <summary>
        /// Closes an opened project
        /// </summary>
        public void Close() 
        {
            ActiveProject = null;

            // Raise event
            if (ActiveProjectChanged != null)
                ActiveProjectChanged(this, new EventArgs());
        }

        /// <summary>
        /// Registers a project template
        /// </summary>
        /// <param name="template">Project template</param>
        public void RegisterProjectTemplate(IProjectTemplate template)
        {
            _projectTemplates.Add(template);
        }

        public IEnumerable<IProjectTemplate> ProjectTemplates { get { return _projectTemplates; } }
    }
}
