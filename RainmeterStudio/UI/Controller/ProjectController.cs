using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using Microsoft.Win32;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Model;
using RainmeterStudio.UI.Dialogs;
using RainmeterStudio.UI.ViewModel;

namespace RainmeterStudio.UI.Controller
{
    public class ProjectController
    {
        #region Properties

        /// <summary>
        /// Gets the project manager
        /// </summary>
        protected ProjectManager Manager { get; private set; }

        /// <summary>
        /// Gets or sets the owner window. Used for creating dialogs.
        /// </summary>
        public Window OwnerWindow { get; set; }

        /// <summary>
        /// Gets the active project
        /// </summary>
        public Project ActiveProject
        {
            get
            {
                return Manager.ActiveProject;
            }
        }

        /// <summary>
        /// Gets the active project path
        /// </summary>
        public string ActiveProjectPath
        {
            get
            {
                return Manager.ActiveProject.Path;
            }
        }

        /// <summary>
        /// Gets the project templates
        /// </summary>
        public IEnumerable<ProjectTemplateViewModel> ProjectTemplates
        {
            get
            {
                return Manager.ProjectTemplates.Select(pt => new ProjectTemplateViewModel(pt));
            }
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Called when a project is opened or the active project closes.
        /// </summary>
        public event EventHandler ActiveProjectChanged
        {
            add
            {
                Manager.ActiveProjectChanged += value;
            }
            remove
            {
                Manager.ActiveProjectChanged -= value;
            }
        }

        #endregion

        #region Commands

        public Command ProjectCreateCommand { get; private set; }

        public Command ProjectOpenCommand { get; private set; }

        #endregion

        /// <summary>
        /// Initializes the project controller
        /// </summary>
        /// <param name="manager">Project manager</param>
        public ProjectController(ProjectManager manager)
        {
            Manager = manager;

            ProjectCreateCommand = new Command("ProjectCreateCommand", () => CreateProject());
            ProjectOpenCommand = new Command("ProjectOpenCommand", () => OpenProject());
        }

        /// <summary>
        /// Displays the 'create project' dialog and creates a new project
        /// </summary>
        public void CreateProject(string name = null, string path = null)
        {
            // Create dialog
            var dialog = new CreateProjectDialog(this);
            dialog.Owner = OwnerWindow;
            dialog.SelectedLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Rainmeter Studio Projects");

            if (name != null) 
                dialog.Name = name;
            
            if (path != null) 
                dialog.SelectedPath = path;
            
            // Display
            bool? res = dialog.ShowDialog();
            if (!res.HasValue || !res.Value)
                return;

            string selectedName = dialog.SelectedName;
            string selectedPath = dialog.SelectedPath;

            // Call manager
            Manager.CreateProject(selectedName, selectedPath);
        }

        /// <summary>
        /// Displays an 'open file' dialog and opens an existing project
        /// </summary>
        /// <param name="path"></param>
        public void OpenProject(string path = null)
        {
            // Open dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Resources.Strings.Dialog_FileType_Project + "|*.rsproj|"
                + Resources.Strings.Dialog_FileType_AllFiles + "|*.*";
            dialog.Title = Resources.Strings.Dialog_OpenProject_Title;
            dialog.Multiselect = false;

            // Show dialog
            bool? res = dialog.ShowDialog(OwnerWindow);
            if (!res.HasValue || !res.Value)
                return;

            // Call manager
            string filename = dialog.FileName;
            Manager.OpenProject(filename);
        }

        /// <summary>
        /// Closes the active project
        /// </summary>
        public void CloseProject()
        {
        }
    }
}
