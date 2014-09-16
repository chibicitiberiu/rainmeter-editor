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
using RainmeterStudio.Properties;
using RainmeterStudio.Core.Utils;

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

        /// <summary>
        /// Create project command
        /// </summary>
        public Command ProjectCreateCommand { get; private set; }

        /// <summary>
        /// Open project command
        /// </summary>
        public Command ProjectOpenCommand { get; private set; }

        /// <summary>
        /// Close project command
        /// </summary>
        public Command ProjectCloseCommand { get; private set; }

        /// <summary>
        /// Cut command
        /// </summary>
        public Command ProjectItemCutCommand { get; private set; }

        /// <summary>
        /// Copy command
        /// </summary>
        public Command ProjectItemCopyCommand { get; private set; }

        /// <summary>
        /// Paste command
        /// </summary>
        public Command ProjectItemPasteCommand { get; private set; }

        /// <summary>
        /// Rename command
        /// </summary>
        public Command ProjectItemRenameCommand { get; private set; }

        /// <summary>
        /// Delete command
        /// </summary>
        public Command ProjectItemDeleteCommand { get; private set; }

        /// <summary>
        /// Open folder command
        /// </summary>
        public Command ProjectItemOpenInExplorerCommand { get; private set; }

        /// <summary>
        /// Open folder command
        /// </summary>
        public Command ProjectItemOpenContainingFolderCommand { get; private set; }


        #endregion

        /// <summary>
        /// Initializes the project controller
        /// </summary>
        /// <param name="manager">Project manager</param>
        public ProjectController(ProjectManager manager)
        {
            Manager = manager;

            // Initialize commands
            ProjectCreateCommand = new Command("ProjectCreate", CreateProject);
            ProjectOpenCommand = new Command("ProjectOpen", OpenProject);
            ProjectCloseCommand = new Command("ProjectClose", CloseProject, () => ActiveProject != null);
            ProjectItemCutCommand = new Command("ProjectItemCut", r => ProjectItemCutClipboard((Reference)r));
            ProjectItemCopyCommand = new Command("ProjectItemCopy", r => ProjectItemCopyClipboard((Reference)r));
            ProjectItemPasteCommand = new Command("ProjectItemPaste", r => ProjectItemPasteClipboard((Reference)r), r => Manager.HaveProjectItemInClipboard());
            ProjectItemRenameCommand = new Command("ProjectItemRename", r => ProjectItemRename((Reference)r));
            ProjectItemDeleteCommand = new Command("ProjectItemDelete", r => ProjectItemDelete((Reference)r));
            ProjectItemOpenInExplorerCommand = new Command("ProjectItemOpenInExplorer", r => ProjectItemOpenInExplorer((Reference)r));
            ProjectItemOpenContainingFolderCommand = new Command("ProjectItemOpenContainingFolder", r => ProjectItemOpenInExplorer((Reference)r));

            ActiveProjectChanged += new EventHandler((sender, e) => ProjectCloseCommand.NotifyCanExecuteChanged());
        }

        #region Project operations

        /// <summary>
        /// Displays the 'create project' dialog and creates a new project
        /// </summary>
        public void CreateProject()
        {
            // Create dialog
            var dialog = new CreateProjectDialog(this);
            dialog.Owner = OwnerWindow;
            
            // Display
            bool? res = dialog.ShowDialog();
            if (!res.HasValue || !res.Value)
                return;

            string selectedName = dialog.SelectedName;
            string selectedPath = dialog.SelectedPath;
            IProjectTemplate selectedTemplate = dialog.SelectedTemplate;

            // Call manager
            Manager.CreateProject(selectedName, selectedPath, selectedTemplate);
        }

        /// <summary>
        /// Displays an 'open file' dialog and opens an existing project
        /// </summary>
        /// <param name="path"></param>
        public void OpenProject()
        {
            // Open dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Resources.Strings.Dialog_FileType_Project + "|*.rsproj|"
                + Resources.Strings.Dialog_FileType_AllFiles + "|*.*";
            dialog.Title = Resources.Strings.Dialog_OpenProject_Title;
            dialog.Multiselect = false;
            dialog.InitialDirectory = Settings.Default.Project_SavedLocation;

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
            Manager.Close();
        }

        #endregion

        #region Project item operations

        protected struct ClipboardData
        {
            public bool Cut;
            public Reference Ref;
        }

        /// <summary>
        /// Places a project item in the clipboard, and marks it for deletion
        /// </summary>
        /// <param name="ref">Project item to cut</param>
        public void ProjectItemCutClipboard(Reference @ref)
        {
            try
            {
                Manager.ProjectItemCutClipboard(@ref);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Places a project item in the clipboard
        /// </summary>
        /// <param name="ref">Project item to copy</param>
        public void ProjectItemCopyClipboard(Reference @ref)
        {
            try
            {
                Manager.ProjectItemCopyClipboard(@ref);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Pastes a project item from clipboard
        /// </summary>
        /// <param name="ref">Destination</param>
        public void ProjectItemPasteClipboard(Reference @ref)
        {
            try
            {
                Manager.ProjectItemPasteClipboard(@ref);
                Manager.SaveActiveProject();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Renames a project item
        /// </summary>
        /// <param name="ref">Reference to project item</param>
        public void ProjectItemRename(Reference @ref)
        {
            string initialValue = Path.GetFileName(@ref.StoragePath.TrimEnd('\\'));

            // Show an input dialog
            var newName = InputDialog.Show(Resources.Strings.RenameReferenceDialog_Prompt,
                Resources.Strings.RenameReferenceDialog_Caption,
                initialValue,
                PathHelper.IsFileNameValid,
                Resources.Strings.RenameReferenceDialog_OKCaption,
                Resources.Strings.Dialog_Cancel);

            if (newName != null)
            {
                try
                {
                    Manager.ProjectItemRename(@ref, newName);
                    Manager.SaveActiveProject();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        /// <summary>
        /// Deletes a project item
        /// </summary>
        /// <param name="ref">Reference to project item</param>
        public void ProjectItemDelete(Reference @ref)
        {
            var res = MessageBox.Show(Resources.Strings.DeleteReferenceDialog_Prompt,
                Resources.Strings.DeleteReferenceDialog_Caption,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            try
            {
                switch(res)
                {
                    case MessageBoxResult.Yes:
                        Manager.ProjectItemDelete(@ref, true);
                        Manager.SaveActiveProject();
                        break;

                    case MessageBoxResult.No:
                        Manager.ProjectItemDelete(@ref, false);
                        Manager.SaveActiveProject();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Opens the containing folder if reference is a file, or folder if reference is a folder in windows explorer
        /// </summary>
        /// <param name="ref">Reference</param>
        public void ProjectItemOpenInExplorer(Reference @ref)
        {
            if (@ref.TargetKind == ReferenceTargetKind.Directory)
            {
                System.Diagnostics.Process.Start(@ref.StoragePath);
            }
            else
            {
                System.Diagnostics.Process.Start(Path.GetDirectoryName(@ref.StoragePath));
            }
        }

        #endregion
    }
}
