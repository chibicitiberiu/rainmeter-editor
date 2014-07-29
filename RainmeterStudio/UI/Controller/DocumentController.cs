using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Business;
using RainmeterStudio.UI.Dialogs;
using RainmeterStudio.Model.Events;
using RainmeterStudio.Model;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using RainmeterStudio.Documents;

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

        public Window OwnerWindow { get; set; }

        public DocumentController(DocumentManager documentManager, ProjectManager projectManager)
        {
            DocumentManager = documentManager;
            ProjectManager = projectManager;

            DocumentCreateCommand = new Command("DocumentCreateCommand", () => CreateWindow());
        }

        public void CreateWindow(DocumentTemplate defaultFormat = null, string defaultPath = "")
        {
            // Show dialog
            var dialog = new CreateDocumentDialog()
            {
                Owner = OwnerWindow,
                SelectedTemplate = defaultFormat,
                SelectedPath = defaultPath
            };
            bool? res = dialog.ShowDialog();

            if (!res.HasValue || !res.Value)
                return;

            var format = dialog.SelectedTemplate;
            var path = dialog.SelectedPath;

            // Call manager
            DocumentManager.Create(format);
        }

        public void Create(DocumentTemplate format)
        {
            // Call manager
            DocumentManager.Create(format);
        }

    }
}
