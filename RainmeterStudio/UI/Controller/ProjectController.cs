using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using RainmeterStudio.Business;
using RainmeterStudio.Model;
using RainmeterStudio.UI.Dialogs;

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

        private Command _projectCreateCommand;
        public Command ProjectCreateCommand
        {
            get
            {
                if (_projectCreateCommand == null)
                {
                    _projectCreateCommand = new Command("ProjectCreateCommand", () => CreateProject())
                    {
                        Shortcut = new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift)
                    };
                }

                return _projectCreateCommand;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the project controller
        /// </summary>
        /// <param name="manager">Project manager</param>
        public ProjectController(ProjectManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Displays the 'create project' dialog and creates a new project
        /// </summary>
        public void CreateProject(string name = null, string path = null)
        {
            // Create dialog
            var dialog = new CreateProjectDialog();
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

        }

        /// <summary>
        /// Closes the active project
        /// </summary>
        public void CloseProject()
        {
        }
    }
}
