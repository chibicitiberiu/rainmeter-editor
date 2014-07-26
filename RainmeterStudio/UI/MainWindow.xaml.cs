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
using RainmeterStudio.Model.Events;
using RainmeterStudio.Storage;
using RainmeterStudio.UI.Controller;
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

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            // Initialize project controller
            // TODO: put this in main
            ProjectStorage projectStorage = new ProjectStorage();
            ProjectManager projectManager = new ProjectManager(projectStorage);
            ProjectController = new Controller.ProjectController(projectManager);
            ProjectController.OwnerWindow = this;
            AddKeyBinding(ProjectController.ProjectCreateCommand);

            // Initialize document controller
            DocumentController = new DocumentController();
            DocumentController.OwnerWindow = this;
            DocumentController.DocumentOpened += documentController_DocumentOpened;
            AddKeyBinding(DocumentController.DocumentCreateCommand);
        }

        private void AddKeyBinding(Command c)
        {
            if (c.Shortcut != null)
                InputBindings.Add(new KeyBinding(c, c.Shortcut));
        }

        void documentController_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            // Spawn a new window
            LayoutDocument document = new LayoutDocument();
            document.Content = e.Editor.EditorUI;
            document.Title = e.Editor.Title;
            document.Closing += document_Closing;

            documentPane.Children.Add(document);
            documentPane.SelectedContentIndex = documentPane.IndexOf(document);
        }

        void document_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (MessageBox.Show("Are you sure?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    break;

                case MessageBoxResult.No:
                    break;

                default:
                    e.Cancel = true;
                    return;
            }
        }
    }
}