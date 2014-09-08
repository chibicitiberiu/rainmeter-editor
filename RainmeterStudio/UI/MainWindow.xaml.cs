using System;
using System.Collections.Generic;
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
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Model.Events;
using RainmeterStudio.Storage;
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
            
            // Subscribe to events
            DocumentController.DocumentOpened += documentController_DocumentOpened;

            // Initialize panels
            projectPanel.Controller = ProjectController;
        }

        void documentController_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            // Create a new panel
            LayoutDocument document = new LayoutDocument();
            _openedDocuments.Add(document, e.Editor);

            document.Content = e.Editor.EditorUI;
            document.Closing += document_Closing;
            document.Closed += document_Closed;
            document.IsActiveChanged += new EventHandler((sender2, e2) =>
            {
                if (document.IsActive)
                    DocumentController.ActiveDocumentEditor = e.Editor;
            });

            documentPane.Children.Add(document);
            documentPane.SelectedContentIndex = documentPane.IndexOf(document);

            e.Document.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((obj, args) =>
            {
                string documentName;
                
                // Get title
                if (ProjectController.ActiveProject == null || !ProjectController.ActiveProject.Contains(e.Document.Reference))
                {
                    documentName = e.Document.Reference.StoragePath ?? "New document";
                }
                else
                {
                    documentName = e.Document.Reference.Name;
                }

                // Is document dirty? Append star
                if (e.Document.IsDirty)
                {
                    documentName += "*";
                }

                // Set document title
                document.Title = documentName;
            });
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