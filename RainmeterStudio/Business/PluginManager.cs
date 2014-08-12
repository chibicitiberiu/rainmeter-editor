using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Utils;

namespace RainmeterStudio.Business
{
    /// <summary>
    /// Manages RainmeterStudio plugins
    /// </summary>
    public class PluginManager
    {
        public delegate void RegisterMethod(object objectToRegister);

        List<Assembly> _loadedPlugins = new List<Assembly>();
        Dictionary<Type, RegisterMethod> _registerTypes = new Dictionary<Type,RegisterMethod>();

        public PluginManager()
        {
        }

        public void AddRegisterType(Type interfaceType, RegisterMethod method)
        {
            _registerTypes.Add(interfaceType, method);
        }

        public void LoadPlugins()
        {
            // Get "Plugins" folder path
            var location = Assembly.GetExecutingAssembly().Location;
            var pluginsPath = Path.Combine(Path.GetDirectoryName(location), "Plugins");

            // Load all DLLs from "Plugins" folder
            foreach (var file in Directory.EnumerateFiles(pluginsPath, "*.dll"))
                LoadPlugin(file);
        }

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
                _loadedPlugins.Add(assembly);
                
                Initialize(assembly);

                Debug.WriteLine("Loaded plugin: {0}", assembly.FullName);
            }
        }

        private void Initialize(Assembly assembly)
        {
            // Register factories and stuff
            assembly.GetTypes()

                // Select only the classes
                .Where(type => type.IsClass)

                // That have the AutoRegister attribute
                .Where(type => type.GetCustomAttributes(typeof(PluginExportAttribute), false).Length > 0)

                // Perform register
                .ForEach((type) =>
                {
                    foreach (var pair in _registerTypes)
                    {
                        if (pair.Key.IsAssignableFrom(type))
                        {
                            var constructor = type.GetConstructor(new Type[0]);
                            var obj = constructor.Invoke(new object[0]);

                            pair.Value(obj);
                        }
                    }
                });
        }

        public IEnumerable<Assembly> LoadedPlugins
        {
            get
            {
                return _loadedPlugins;
            }
        }
    }
}
