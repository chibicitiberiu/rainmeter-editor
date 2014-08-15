using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Documents
{
    /// <summary>
    /// An empty project template
    /// </summary>
    [PluginExport]
    public class EmptyProjectTemplate : IProjectTemplate
    {
        /// <summary>
        /// Gets or sets the name of the template
        /// </summary>
        public string Name
        {
            get { return "EmptyProject"; }
        }

        /// <summary>
        /// Gets or sets the properties of this template
        /// </summary>
        public IEnumerable<Property> Properties
        {
            get { return Enumerable.Empty<Property>(); }
        }

        /// <summary>
        /// Creates a project.
        /// </summary>
        /// <returns>Created project</returns>
        public Project CreateProject()
        {
            return new Project();
        }
    }
}
