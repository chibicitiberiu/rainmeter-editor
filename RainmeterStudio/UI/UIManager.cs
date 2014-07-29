using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Business;
using RainmeterStudio.UI.Controller;

namespace RainmeterStudio.UI
{
    public class UIManager
    {
        /// <summary>
        /// Gets the WPF app
        /// </summary>
        protected App App {get; private set;}

        /// <summary>
        /// Gets the project manager
        /// </summary>
        protected ProjectManager ProjectManager { get; private set; }

        /// <summary>
        /// Gets the document manager
        /// </summary>
        protected DocumentManager DocumentManager { get; private set; }

        /// <summary>
        /// Initializes the UI manager
        /// </summary>
        /// <param name="projectManager">Project manager</param>
        /// <param name="documentManager">Document manager</param>
        public UIManager(ProjectManager projectManager, DocumentManager documentManager)
        {
            App = new UI.App();
            ProjectManager = projectManager;
            DocumentManager = documentManager;
        }

        /// <summary>
        /// Runs the UI thread
        /// </summary>
        public void Run()
        {
            // Create controllers
            ProjectController projectController = new ProjectController(ProjectManager);
            DocumentController documentController = new DocumentController(DocumentManager, ProjectManager);

            // Create and set up main window
            MainWindow mainWindow = new MainWindow(projectController, documentController);
            projectController.OwnerWindow = mainWindow;
            documentController.OwnerWindow = mainWindow;
            mainWindow.Show();

            // Run app
            App.Run();
        }
    }
}
