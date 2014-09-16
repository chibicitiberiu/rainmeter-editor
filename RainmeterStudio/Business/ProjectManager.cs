using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Storage;
using RainmeterStudio.Core.Utils;
using RainmeterStudio.Editor.ProjectEditor;
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

        #endregion

        #region Events

        /// <summary>
        /// Called when a project is opened or the active project closes.
        /// </summary>
        public event EventHandler ActiveProjectChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the project manager
        /// </summary>
        /// <param name="storage">Project storage</param>
        public ProjectManager()
        {
            ActiveProject = null;
        }

        #endregion

        #region Project operations

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
            string directory = Path.GetDirectoryName(path);
            if (!String.IsNullOrEmpty(directory))
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
            ActiveProject = ProjectStorage.Read(path);
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
            ProjectStorage.Write(ActiveProject);
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

        #endregion

        #region Document templates

        /// <summary>
        /// Registers a project template
        /// </summary>
        /// <param name="template">Project template</param>
        public void RegisterProjectTemplate(IProjectTemplate template)
        {
            _projectTemplates.Add(template);
        }

        /// <summary>
        /// Gets a list of existing project templates
        /// </summary>
        public IEnumerable<IProjectTemplate> ProjectTemplates { get { return _projectTemplates; } }

        #endregion

        #region Project item operations

        [Serializable]
        protected struct ClipboardData
        {
            public bool Cut;
            public string QualifiedName;
        }

        /// <summary>
        /// Places a project item in the clipboard, and marks it for deletion
        /// </summary>
        /// <param name="ref">Project item to cut</param>
        public void ProjectItemCutClipboard(Reference @ref)
        {
            var dataFormat = DataFormats.GetDataFormat(typeof(ClipboardData).FullName);

            ClipboardData data = new ClipboardData();
            data.Cut = true;
            data.QualifiedName = @ref.QualifiedName;

            Clipboard.SetData(dataFormat.Name, data);
        }

        /// <summary>
        /// Places a project item in the clipboard
        /// </summary>
        /// <param name="ref">Project item to copy</param>
        public void ProjectItemCopyClipboard(Reference @ref)
        {
            var dataFormat = DataFormats.GetDataFormat(typeof(ClipboardData).FullName);

            ClipboardData data = new ClipboardData();
            data.Cut = false;
            data.QualifiedName = @ref.QualifiedName;

            Clipboard.SetData(dataFormat.Name, data);
        }

        /// <summary>
        /// Pastes a project item from clipboard
        /// </summary>
        /// <param name="dest">Destination</param>
        public void ProjectItemPasteClipboard(Reference dest)
        {
            var dataFormat = DataFormats.GetDataFormat(typeof(ClipboardData).FullName);

            if (Clipboard.ContainsData(dataFormat.Name))
            {
                ClipboardData data = (ClipboardData)Clipboard.GetData(dataFormat.Name);
                var reference = ActiveProject.Root.GetReference(data.QualifiedName);

                if (data.Cut)
                {
                    ProjectItemMove(reference, dest);
                    Clipboard.Clear();
                }
                else
                {
                    ProjectItemCopy(reference, dest);
                }
            }
        }

        /// <summary>
        /// Moves a project item to another folder
        /// </summary>
        /// <param name="ref">Project item to move</param>
        /// <param name="dest">Destination folder</param>
        public void ProjectItemMove(Reference @ref, Reference dest)
        {
            // Move storage file
            string refPath = Path.GetFileName(@ref.StoragePath.TrimEnd('\\'));
            string destinationPath = (dest.TargetKind == ReferenceTargetKind.Directory) ? dest.StoragePath : Path.GetDirectoryName(dest.StoragePath);
            string newPath = Path.Combine(destinationPath, refPath);
            
            if (@ref.TargetKind == ReferenceTargetKind.Directory)
            {
                Directory.Move(@ref.StoragePath, newPath);
                
                // Update children
                UpdateRenameChildren(@ref, @ref.StoragePath, newPath);
            }
            else
            {
                File.Move(@ref.StoragePath, newPath);
            }

            // Set up reference object
            @ref.Unparent();
            @ref.StoragePath = newPath;
            dest.Add(@ref);
        }

        private void UpdateRenameChildren(Reference root, string oldPath, string newPath)
        {
            foreach (var pair in root.ChildrenDictionary)
            {
                pair.Value.StoragePath = pair.Value.StoragePath.Replace(oldPath, newPath);
                UpdateRenameChildren(pair.Value, oldPath, newPath);
            }
        }

        /// <summary>
        /// Creates a copy of a project item to another folder
        /// </summary>
        /// <param name="ref">Project item to copy</param>
        /// <param name="dest">Destination folder</param>
        /// <returns>Reference to the copy</returns>
        public Reference ProjectItemCopy(Reference @ref, Reference dest)
        {
            // Create a clone reference
            var copyRef = (Reference)@ref.Clone();
            
            // Copy storage file
            string refPath = Path.GetFileName(@ref.StoragePath.TrimEnd('\\'));
            string destinationPath = (dest.TargetKind == ReferenceTargetKind.Directory) ? dest.StoragePath : Path.GetDirectoryName(dest.StoragePath);
            string newPath = Path.Combine(destinationPath, refPath);

            if (@ref.TargetKind == ReferenceTargetKind.Directory)
            {
                DirectoryHelper.CopyDirectory(@ref.StoragePath, newPath);

                // Update children
                UpdateRenameChildren(copyRef, copyRef.StoragePath, newPath);
            }
            else
            {
                // Find a nonconflicting file name
                newPath = GetNonConflictingPath(refPath, destinationPath);

                // Copy
                File.Copy(@ref.StoragePath, newPath);
            }

            // Parent reference
            copyRef.Name = Path.GetFileName(newPath);
            copyRef.StoragePath = newPath;
            dest.Add(copyRef);
            return copyRef;
        }

        private static string GetNonConflictingPath(string filename, string destinationPath)
        {
            // Initial path - destination path + file name
            string newPath = Path.Combine(destinationPath, filename);

            // Initial number
            int i = 1;

            // Try to find if there already is a number
            var match = Regex.Match(newPath, "_([0-9])$");
            if (match.Success)
            {
                i = Int32.Parse(match.Groups[1].Value);
            }

            // Find non-conflicting number
            while (File.Exists(newPath))
            {
                ++i;
                newPath = Path.Combine(destinationPath, Path.GetFileNameWithoutExtension(filename) + "_" + i.ToString() + Path.GetExtension(filename));
            }
            
            return newPath;
        }

        public void ProjectItemRename(Reference @ref, string newName)
        {
            // Rename on disk
            string refPath = @ref.StoragePath.TrimEnd('\\');
            string refDir = Path.GetDirectoryName(refPath);
            string newPath = Path.Combine(refDir, newName);

            if (@ref.TargetKind == ReferenceTargetKind.Directory)
            {
                Directory.Move(refPath, newPath);
                newPath += '\\';
            }
            else
            {
                File.Move(refPath, newPath);
            }

            // Set reference
            @ref.Name = newName;
            @ref.StoragePath = newPath;
        }

        /// <summary>
        /// Deletes a project item
        /// </summary>
        /// <param name="ref"></param>
        public void ProjectItemDelete(Reference @ref, bool fromDisk)
        {
            if (fromDisk)
            {
                if (@ref.TargetKind == ReferenceTargetKind.File)
                    File.Delete(@ref.StoragePath);

                else Directory.Delete(@ref.StoragePath, true);
            }

            @ref.Unparent();
        }

        /// <summary>
        /// Checks if there is a project item in the clipboard
        /// </summary>
        /// <returns>True if there is a project item in the clipboard</returns>
        public bool HaveProjectItemInClipboard()
        {
            var dataFormat = DataFormats.GetDataFormat(typeof(ClipboardData).FullName);
            return Clipboard.ContainsData(dataFormat.Name);
        }

        /// <summary>
        /// Creates a new folder with given name
        /// </summary>
        /// <param name="name">Name of folder</param>
        /// <param name="parent">Parent folder</param>
        public void CreateFolder(string name, Reference parent)
        {
            string dir = (parent.TargetKind == ReferenceTargetKind.Directory) ?
                parent.StoragePath : Path.GetDirectoryName(parent.StoragePath);
            string newDirPath = Path.Combine(dir, name);

            Directory.CreateDirectory(newDirPath);
            parent.Add(new Reference(name, newDirPath, ReferenceTargetKind.Directory));
        }

        #endregion
    }
}
