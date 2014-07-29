using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using RainmeterStudio.Business;
using RainmeterStudio.Documents;
using RainmeterStudio.Storage;
using RainmeterStudio.UI;
using RainmeterStudio.UI.Controller;

namespace RainmeterStudio
{
    static class MainClass
    {
        [STAThread]
        public static void Main()
        {
            // Display splash
            SplashScreen splash = new SplashScreen("Resources/splash.png");
            splash.Show(true);

            // Initialize managers
            ProjectStorage projectStorage = new ProjectStorage();
            ProjectManager projectManager = new ProjectManager(projectStorage);

            DocumentManager documentManager = new DocumentManager();
            documentManager.PerformAutoRegister();

            // Create & run app
            var uiManager = new UIManager(projectManager, documentManager);
            uiManager.Run();
        }
    }
}
