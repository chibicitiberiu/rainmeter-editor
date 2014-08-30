using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Model.Events;
using RainmeterStudio.UI.Dialogs;
using RainmeterStudio.UI.ViewModel;
using RainmeterStudio.Core.Model;
using System.IO;

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



        #endregion

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

        public MainWindow OwnerWindow { get; set; }

        public DocumentController(DocumentManager documentManager, ProjectManager projectManager)
        {
            DocumentManager = documentManager;
            ProjectManager = projectManager;

            DocumentCreateCommand = new Command("DocumentCreateCommand", () => Create(), () => ProjectManager.ActiveProject != null);
            ProjectManager.ActiveProjectChanged += new EventHandler((obj, e) => DocumentCreateCommand.NotifyCanExecuteChanged());
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

            string folder = OwnerWindow.ProjectPanel.ActiveItem.Data.StoragePath;
            if (!Directory.Exists(folder))
                folder = Path.GetDirectoryName(folder);

            var reference = new Reference(name, Path.Combine(folder, name));
            editor.AttachedDocument.Reference = reference;

            // Save document
            DocumentManager.Save(editor.AttachedDocument);

            // Add to parent
            OwnerWindow.ProjectPanel.ActiveItem.Add(reference);
        }

        /// <summary>
        /// Saves the document opened in specified editor
        /// </summary>
        /// <param name="editor">Editor</param>
        public void Save(IDocumentEditor editor)
        {
            if (!editor.AttachedDocument.Reference.IsOnStorage())
            {
                SaveAs(editor);
                return;
            }

            // TODO
        }

        public void SaveAs(IDocumentEditor editor)
        {
            // TODO
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
                bool? res = CloseUnsavedDialog.ShowDialog(OwnerWindow, editor.AttachedDocument);
                if (res.HasValue)
                {
                    // Save
                    if (res.Value)
                    {
                        Save(editor);
                    }
                }
                else
                {
                    return false;
                }
            }

            // Close
            DocumentManager.Close(editor);
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
