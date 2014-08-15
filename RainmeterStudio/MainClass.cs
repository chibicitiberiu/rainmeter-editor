using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Storage;
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

            // Initialize project manager
            ProjectStorage projectStorage = new ProjectStorage();
            ProjectManager projectManager = new ProjectManager(projectStorage);

            // Initialize document manager
            DocumentManager documentManager = new DocumentManager();

            // Initialize plugin manager
            PluginManager pluginManager = new PluginManager();
            pluginManager.AddRegisterExportTypeHandler(typeof(IDocumentStorage), obj => documentManager.RegisterStorage((IDocumentStorage)obj));
            pluginManager.AddRegisterExportTypeHandler(typeof(DocumentTemplate), obj => documentManager.RegisterTemplate((DocumentTemplate)obj));
            pluginManager.AddRegisterExportTypeHandler(typeof(IDocumentEditorFactory), obj => documentManager.RegisterEditorFactory((IDocumentEditorFactory)obj));
            pluginManager.Initialize();

            // Create & run app
            var uiManager = new UIManager(projectManager, documentManager);
            uiManager.Run();

            // Run finished, persist settings
            SettingsProvider.SaveSettings();
        }
    }
}
