using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Editor.Features;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Model.Events;
using RainmeterStudio.UI.Controller;
using RainmeterStudio.UI.Panels;
using Xceed.Wpf.AvalonDock.Layout;

namespace RainmeterStudio.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gets or sets the document controller
        /// </summary>
        public DocumentController DocumentController { get; set; }

        /// <summary>
        /// Gets or sets the project controller
        /// </summary>
        public ProjectController ProjectController { get; set; }

        /// <summary>
        /// Gets the project panel
        /// </summary>
        public ProjectPanel ProjectPanel { get { return projectPanel; } }

        private Dictionary<LayoutDocument, IDocumentEditor> _openedDocuments = new Dictionary<LayoutDocument, IDocumentEditor>();

        #region Constructor

        /// <summary>
        /// Initializes the main window
        /// </summary>
        /// <param name="projCtrl">The project controller</param>
        /// <param name="docCtrl">The document controller</param>
        public MainWindow(ProjectController projCtrl, DocumentController docCtrl)
        {
            InitializeComponent();

            // Set fields
            DataContext = this;
            DocumentController = docCtrl;
            ProjectController = projCtrl;

            // Add key bindings
            this.AddKeyBinding(DocumentController.DocumentCreateCommand);
            this.AddKeyBinding(DocumentController.DocumentOpenCommand);
            this.AddKeyBinding(DocumentController.DocumentSaveCommand);
            this.AddKeyBinding(DocumentController.DocumentCloseCommand);
            this.AddKeyBinding(ProjectController.ProjectCreateCommand);
            this.AddKeyBinding(ProjectController.ProjectOpenCommand);
            
            // Subscribe to events
            DocumentController.DocumentOpened += DocumentController_DocumentOpened;

            // Initialize panels
            projectPanel.ProjectController = ProjectController;
            projectPanel.DocumentController = DocumentController;
        }

        #endregion

        #region Document opened event handler

        void DocumentController_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            OpenDocument(e.Editor);
        }

        #endregion

        #region Document events

        void Document_IsActiveChanged(object sender, EventArgs e)
        {
            LayoutDocument document = (LayoutDocument)sender;
            IDocumentEditor editor = _openedDocuments[document];

            if (document.IsActive)
            {
                // Set active editor in controller
                DocumentController.ActiveDocumentEditor = editor;

                // Set up toolbox
                SetUpToolbox(editor);
            }
            else
            {
                // Disable drop, so that we don't drop invalid items in an editor
                editor.EditorUI.AllowDrop = false;
            }
        }

        void Document_Closed(object sender, EventArgs e)
        {
            CloseDocument((LayoutDocument)sender);
        }

        void Document_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Get editor
            var document = (LayoutDocument)sender;
            var editor = _openedDocuments[document];

            // Try to close active document
            if (!DocumentController.Close(editor))
            {
                e.Cancel = true;
            }
        }

        private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IDocument document = (IDocument)sender;

            // Find document object and update document title
            foreach (var pair in _openedDocuments)
            {
                if (pair.Value.AttachedDocument == document)
                {
                    pair.Key.Title = GetDocumentTitle(document);
                }
            }

            // If the reference changed, subscribe to reference changes as well
            if (e.PropertyName == "Reference" && document.Reference != null)
            {
                document.Reference.PropertyChanged += Reference_PropertyChanged;
            }
        }

        void Reference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Reference reference = (Reference)sender;
            bool found = false;

            // Find documents with this reference and update document title
            foreach (var pair in _openedDocuments)
            {
                if (pair.Value.AttachedDocument.Reference == reference)
                {
                    pair.Key.Title = GetDocumentTitle(pair.Value.AttachedDocument);
                    found = true;
                }
            }

            // No document found? Unsubscribe
            if (found == false)
            {
                reference.PropertyChanged -= Reference_PropertyChanged;
            }
        }

        #endregion

        #region Window events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Try to close
            if (!DocumentController.CloseAll())
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region Helper methods

        private string GetDocumentTitle(IDocument document)
        {
            string documentName;

            // Get title
            if (document.Reference == null)
            {
                documentName = "New document";
            }
            else if (ProjectController.ActiveProject == null || !ProjectController.ActiveProject.Contains(document.Reference))
            {
                documentName = document.Reference.StoragePath ?? "New document";
            }
            else
            {
                documentName = document.Reference.Name;
            }

            // Is document dirty? Append star
            if (document.IsDirty)
            {
                documentName += "*";
            }

            return documentName;
        }

        private LayoutDocument OpenDocument(IDocumentEditor editor)
        {
            // Create document
            LayoutDocument document = new LayoutDocument();
            document.Content = editor.EditorUI;
            document.Title = GetDocumentTitle(editor.AttachedDocument);

            // Set up events
            document.Closing += Document_Closing;
            document.Closed += Document_Closed;
            document.IsActiveChanged += Document_IsActiveChanged;

            // Add to dictionary
            _openedDocuments.Add(document, editor);

            // Add to layout
            documentPane.Children.Add(document);
            documentPane.SelectedContentIndex = documentPane.IndexOf(document);

            // Subscribe to document events
            editor.AttachedDocument.PropertyChanged += Document_PropertyChanged;
            if (editor.AttachedDocument.Reference != null)
                editor.AttachedDocument.Reference.PropertyChanged += Reference_PropertyChanged;

            return document;
        }

        private void CloseDocument(LayoutDocument document)
        {
            _openedDocuments.Remove(document);
        }

        private void SetUpToolbox(IDocumentEditor editor)
        {
            var toolboxProvider = editor as IToolboxProvider;

            // Set toolbar panel
            toolboxPanel.ItemsSource = toolboxProvider;

            // Enable 'allow drop'
            if (toolboxProvider != null)
            {
                editor.EditorUI.AllowDrop = true;
            }
        }

        #endregion
    }
}