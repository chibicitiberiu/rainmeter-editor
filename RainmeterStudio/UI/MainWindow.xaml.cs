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
        public DocumentController DocumentController { get; set; }
        public ProjectController ProjectController { get; set; }

        public ProjectPanel ProjectPanel { get { return projectPanel; } }

        private Dictionary<LayoutDocument, IDocumentEditor> _openedDocuments = new Dictionary<LayoutDocument, IDocumentEditor>();

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
            DocumentController.DocumentOpened += documentController_DocumentOpened;

            // Initialize panels
            projectPanel.ProjectController = ProjectController;
            projectPanel.DocumentController = DocumentController;
        }

        void documentController_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            // Create a new panel
            LayoutDocument document = new LayoutDocument();
            _openedDocuments.Add(document, e.Editor);

            document.Content = e.Editor.EditorUI;
            document.Closing += document_Closing;
            document.Closed += document_Closed;
            document.Title = GetDocumentTitle(e.Document);
            document.IsActiveChanged += new EventHandler((sender2, e2) =>
            {
                if (document.IsActive)
                    DocumentController.ActiveDocumentEditor = e.Editor;
            });

            documentPane.Children.Add(document);
            documentPane.SelectedContentIndex = documentPane.IndexOf(document);

            e.Document.PropertyChanged += Document_PropertyChanged;
            if (e.Document.Reference != null)
                e.Document.Reference.PropertyChanged += Reference_PropertyChanged;
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

        void document_Closed(object sender, EventArgs e)
        {
            var layoutDocument = (LayoutDocument)sender;
        }

        void document_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Try to close
            if (!DocumentController.CloseAll())
            {
                e.Cancel = true;
            }
        }
    }
}