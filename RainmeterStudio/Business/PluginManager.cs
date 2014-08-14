using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Utils;
using RainmeterStudio.Resources;

namespace RainmeterStudio.Business
{
    /// <summary>
    /// Manages RainmeterStudio plugins
    /// </summary>
    public class PluginManager
    {
        #region Private fields

        List<Assembly> _loadedPlugins = new List<Assembly>();

        Dictionary<Type, RegisterExportHandler> _registerExportTypes = new Dictionary<Type, RegisterExportHandler>();

        #endregion

        /// <summary>
        /// Gets an enumerable of the loaded plugins
        /// </summary>
        public IEnumerable<Assembly> LoadedPlugins
        {
            get
            {
                return _loadedPlugins;
            }
        }

        /// <summary>
        /// A method which registers an object that was exported by a plugin.
        /// </summary>
        /// <param name="objectToRegister">Object to register</param>
        public delegate void RegisterExportHandler(object objectToRegister);

        #region Constructor

        /// <summary>
        /// Initializes the plugin manager
        /// </summary>
        public PluginManager()
        {
        }

        #endregion
        
        /// <summary>
        /// Adds a handler that registers exported objects of a specific type.
        /// </summary>
        /// <param name="interfaceType">The data type</param>
        /// <param name="method">Handler that does the registring</param>
        public void AddRegisterExportTypeHandler(Type interfaceType, RegisterExportHandler method)
        {
            _registerExportTypes.Add(interfaceType, method);
        }

        /// <summary>
        /// Initializes the plugin manager
        /// </summary>
        /// <remarks>This will load all the plugins from the "StudioPlugins" folder</remarks>
        public void Initialize()
        {
            // Initialize the executing assembly
            InitializePlugin(Assembly.GetExecutingAssembly());

            // Load plugins from StudioPlugins folder
            var location = Assembly.GetExecutingAssembly().Location;
            var pluginsPath = Path.Combine(Path.GetDirectoryName(location), "StudioPlugins");

            LoadPlugins(pluginsPath);
        }
        
        /// <summary>
        /// Loads all the plugins from the specified directory.
        /// </summary>
        /// <param name="pluginsPath">Directory path</param>
        public void LoadPlugins(string pluginsPath)
        {
            // Load all DLLs from "Plugins" folder
            foreach (var file in Directory.EnumerateFiles(pluginsPath, "*.dll"))
                LoadPlugin(file);
        }

        /// <summary>
        /// Tries to load the plugin.
        /// </summary>
        /// <param name="file">File name</param>
        /// <remarks>If plugin is not loaded, the function fails silently.</remarks>
        public void LoadPlugin(string file)
        {
            Assembly assembly = null;

            // Try to load assembly
            try
            {
                assembly = Assembly.LoadFile(file);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load assembly {0}: {1}", file, ex);
            }

            // Loaded, do initialization stuff
            if (assembly != null)
            {
                // Check for the RainmeterStudioPlugin attribute
                if (assembly.GetCustomAttributes(typeof(RainmeterStudioPluginAttribute), false).Count() > 0)
                {
                    _loadedPlugins.Add(assembly);
                    InitializePlugin(assembly);
                    Debug.WriteLine("Loaded plugin: {0}", (object)assembly.Location);
                }
            }
        }

        private void InitializePlugin(Assembly assembly)
        {
            // Register exports
            assembly.GetTypes()

                // Select only the classes
                .Where(type => type.IsClass)

                // That have the AutoRegister attribute
                .Where(type => type.GetCustomAttributes(typeof(PluginExportAttribute), false).Length > 0)

                // Perform register
                .ForEach((type) =>
                {
                    foreach (var pair in _registerExportTypes)
                    {
                        if (pair.Key.IsAssignableFrom(type))
                        {
                            var constructor = type.GetConstructor(new Type[0]);
                            var obj = constructor.Invoke(new object[0]);

                            pair.Value(obj);
                        }
                    }
                });

            // Register .resource files
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                var name = Path.GetFileNameWithoutExtension(resourceName);
                ResourceManager manager = new ResourceManager(name, assembly);
                ResourceProvider.RegisterManager(manager, assembly);
            }
        }
    }
}
