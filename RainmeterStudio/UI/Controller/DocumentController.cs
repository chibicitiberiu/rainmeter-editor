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

        public void CreateWindow(IDocumentTemplate defaultFormat = null, string defaultPath = "")
        {
            // Show dialog
            var dialog = new CreateDocumentDialog(this)
            {
                Owner = OwnerWindow,
                SelectedTemplate = new DocumentTemplateViewModel(defaultFormat),
                SelectedPath = defaultPath
            };
            bool? res = dialog.ShowDialog();

            if (!res.HasValue || !res.Value)
                return;

            var format = dialog.SelectedTemplate;
            var path = dialog.SelectedPath;

            // Call manager
            DocumentManager.Create(format.Template);
        }

        public void Create(IDocumentTemplate format)
        {
            // Call manager
            DocumentManager.Create(format);
        }

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
