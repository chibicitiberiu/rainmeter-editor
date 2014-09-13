using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Model.Events;
using RainmeterStudio.UI.Dialogs;
using RainmeterStudio.UI.ViewModel;
using RainmeterStudio.Core.Model;
using System.IO;
using Microsoft.Win32;
using RainmeterStudio.Core.Utils;

namespace RainmeterStudio.UI.Controller
{
    public class DocumentController
    {
        #region Managers

        /// <summary>
        /// Gets or sets the document manager
        /// </summary>
        protected DocumentManager DocumentManager { get; private set; }

        /// <summary>
        /// Gets or sets the project manager
        /// </summary>
        protected ProjectManager ProjectManager { get; private set; }

        #endregion

        #region Commands

        public Command DocumentCreateCommand { get; private set; }

        public Command DocumentOpenCommand { get; private set; }

        public Command DocumentSaveCommand { get; private set; }

        public Command DocumentSaveAsCommand { get; private set; }

        public Command DocumentSaveACopyCommand { get; private set; }

        public Command DocumentSaveAllCommand { get; private set; }

        public Command DocumentCloseCommand { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Triggered when a document is opened
        /// </summary>
        public event EventHandler<DocumentOpenedEventArgs> DocumentOpened
        {
            add { DocumentManager.DocumentOpened += value; }
            remove { DocumentManager.DocumentOpened -= value; }
        }

        /// <summary>
        /// Triggered when a document is closed
        /// </summary>
        public event EventHandler<DocumentClosedEventArgs> DocumentClosed
        {
            add { DocumentManager.DocumentClosed += value; }
            remove { DocumentManager.DocumentClosed -= value; }
        }

        /// <summary>
        /// Triggered when the active document editor changes.
        /// </summary>
        public event EventHandler ActiveDocumentEditorChanged;

        #endregion

        #region Properties

        private IDocumentEditor _activeDocumentEditor = null;

        /// <summary>
        /// Gets or sets the active document editor.
        /// This must be set by the main window when active document changes.
        /// </summary>
        public IDocumentEditor ActiveDocumentEditor
        {
            get
            {
                return _activeDocumentEditor;
            }
            set
            {
                _activeDocumentEditor = value;

                if (ActiveDocumentEditorChanged != null)
                    ActiveDocumentEditorChanged(this, new EventArgs());
            }
        }

        public MainWindow OwnerWindow { get; set; }


        #endregion

        /// <summary>
        /// Initializes a document controller
        /// </summary>
        /// <param name="documentManager"></param>
        /// <param name="projectManager"></param>
        public DocumentController(DocumentManager documentManager, ProjectManager projectManager)
        {
            DocumentManager = documentManager;
            ProjectManager = projectManager;

            DocumentCreateCommand = new Command("DocumentCreate", Create, () => ProjectManager.ActiveProject != null);
            DocumentOpenCommand = new Command("DocumentOpen", Open);
            DocumentSaveCommand = new Command("DocumentSave", () => Save(), HasActiveDocumentEditor);
            DocumentSaveAsCommand = new Command("DocumentSaveAs", () => SaveAs(), HasActiveDocumentEditor);
            DocumentSaveACopyCommand = new Command("DocumentSaveACopy", () => SaveACopy(), HasActiveDocumentEditor);
            DocumentSaveAllCommand = new Command("DocumentSaveAll", SaveAll, () => ProjectManager.ActiveProject != null);
            DocumentCloseCommand = new Command("DocumentClose", () => Close(), HasActiveDocumentEditor);

            ProjectManager.ActiveProjectChanged += new EventHandler((sender, e) => 
            {
                DocumentCreateCommand.NotifyCanExecuteChanged();
                DocumentSaveAllCommand.NotifyCanExecuteChanged();
            });

            ActiveDocumentEditorChanged += new EventHandler((sender, e) =>
            {
                DocumentSaveCommand.NotifyCanExecuteChanged();
                DocumentSaveAsCommand.NotifyCanExecuteChanged();
                DocumentSaveACopyCommand.NotifyCanExecuteChanged();
                DocumentCloseCommand.NotifyCanExecuteChanged();
            });
        }
        
        private bool HasActiveDocumentEditor()
        {
            return ActiveDocumentEditor != null;
        }

        #region Document operations

        /// <summary>
        /// Shows the new item dialog, and creates a new document
        /// </summary>
        public void Create()
        {
            // Show dialog
            var dialog = new CreateDocumentDialog(this);
            dialog.Owner = OwnerWindow;
            bool? res = dialog.ShowDialog();

            if (!res.HasValue || !res.Value)
                return;

            var format = dialog.SelectedTemplate;

            // Call manager
            var editor = DocumentManager.Create(format.Template);

            // Set the reference
            var name = dialog.SelectedName;
            
            string folder = OwnerWindow.ProjectPanel.ActiveItem.StoragePath;
            if (!Directory.Exists(folder))
                folder = Path.GetDirectoryName(folder);

            var reference = new Reference(name, Path.Combine(folder, name), Reference.ReferenceTargetKind.File);
            editor.AttachedDocument.Reference = reference;

            // Save document
            DocumentManager.Save(editor.AttachedDocument);

            // Add to parent
            OwnerWindow.ProjectPanel.ActiveItem.Add(reference);
            ProjectManager.SaveActiveProject();
        }

        /// <summary>
        /// Shows an 'open document' dialog, and opens a document
        /// </summary>
        public void Open()
        {
            // Show open dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = Resources.Strings.Dialog_OpenDocument_Title;
            dialog.Filter = Resources.Strings.Dialog_FileType_AllFiles + "|*.*";
            dialog.InitialDirectory = Properties.Settings.Default.Project_SavedLocation;

            bool? res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                // Open file
                DocumentManager.Open(dialog.FileName);
            }
        }

        /// <summary>
        /// Saves the active document
        /// </summary>
        public bool Save()
        {
            return Save(ActiveDocumentEditor);
        }

        /// <summary>
        /// Saves the active document
        /// </summary>
        public bool Save(IDocumentEditor editor)
        {
            if (editor.AttachedDocument.Reference != null)
            {
                DocumentManager.Save(editor.AttachedDocument);
                return true;
            }
            else
            {
                return SaveAs(editor);
            }
        }

        /// <summary>
        /// Displays a 'save as' dialog, and saves active document
        /// </summary>
        public bool SaveAs()
        {
            return SaveAs(ActiveDocumentEditor);
        }

        /// <summary>
        /// Displays a 'save as' dialog, and saves active document
        /// </summary>
        public bool SaveAs(IDocumentEditor editor)
        {
            // Show save dialog
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = Resources.Strings.Dialog_SaveDocument_Title;
            dialog.Filter = Resources.Strings.Dialog_FileType_AllFiles + "|*.*";
            dialog.FileName = editor.AttachedDocument.Reference.StoragePath;

            bool? res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                DocumentManager.SaveAs(dialog.FileName, editor.AttachedDocument);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Displays a 'save' dialog, and saves a copy of the active document
        /// </summary>
        public void SaveACopy()
        {
            // Show save dialog
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = Resources.Strings.Dialog_SaveDocument_Title;
            dialog.Filter = Resources.Strings.Dialog_FileType_AllFiles + "|*.*";
            dialog.FileName = ActiveDocumentEditor.AttachedDocument.Reference.StoragePath;

            bool? res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                DocumentManager.SaveACopy(dialog.FileName, ActiveDocumentEditor.AttachedDocument);
            }
        }

        /// <summary>
        /// Saves all opened documents
        /// </summary>
        public void SaveAll()
        {
            foreach (var editor in DocumentManager.Editors)
            {
                if (!Save(editor))
                    return;
            }
        }

        /// <summary>
        /// Closes an active document.
        /// </summary>
        /// <param name="editor">The document editor attached</param>
        /// <returns>True if closed successfully</returns>
        /// <remarks>Shows the 'are you sure' prompt if there are unsaved edits.</remarks>
        public bool Close(IDocumentEditor editor)
        {
            // Show the 'are you sure' prompt if necesary
            if (editor.AttachedDocument.IsDirty)
            {
                switch(CloseUnsavedDialog.ShowDialog(OwnerWindow, editor.AttachedDocument))
                {
                    case CloseUnsavedDialogResult.Save:
                        Save();
                        break;

                    case CloseUnsavedDialogResult.Cancel:
                        return false;
                }
            }

            // Close
            DocumentManager.Close(editor);

            // Update ActiveDocument
            if (editor == ActiveDocumentEditor)
                ActiveDocumentEditor = null;

            return true;
        }

        /// <summary>
        /// Closes the active document.
        /// </summary>
        /// <returns>True if closed successfully</returns>
        /// <remarks>Shows the 'are you sure' prompt if there are unsaved edits.</remarks>
        public bool Close()
        {
            return Close(ActiveDocumentEditor);
        }

        /// <summary>
        /// Closes all the opened documents
        /// </summary>
        /// <returns>True if closed successfully, false if user hit 'cancel'.</returns>
        public bool CloseAll()
        {
            // Get dirty documents
            var unsaved = DocumentManager.Editors
                .Select(editor => editor.AttachedDocument)
                .Where(document => document.IsDirty);

            // There are unsaved documents? Display save dialog
            if (unsaved.Any())
            {
                switch (CloseUnsavedDialog.ShowDialog(OwnerWindow, unsaved))
                {
                    case CloseUnsavedDialogResult.Save:
                        SaveAll();
                        break;

                    case CloseUnsavedDialogResult.Cancel:
                        return false;
                }
            }

            // Close all documents
            // To array is used because DocumentManager.Editors is modified when closing a document.
            DocumentManager.Editors.ToArray().ForEach(DocumentManager.Close);
            
            // Done
            return true;
        }

        #endregion

        /// <summary>
        /// Gets a list of document templates view models
        /// </summary>
        public IEnumerable<DocumentTemplateViewModel> DocumentTemplates
        {
            get
            {
                return DocumentManager.DocumentTemplates.Select(t => new DocumentTemplateViewModel(t));
            }
        }
    }
}
