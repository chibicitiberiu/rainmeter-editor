using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Core.Model
{
    /// <summary>
    /// Project template interface
    /// </summary>
    public interface IProjectTemplate
    {
        /// <summary>
        /// Gets or sets the name of the template
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the properties of this template
        /// </summary>
        /// <remarks>Properties are used to display a form dialog after the "New project" dialog closes.</remarks>
        IEnumerable<Property> Properties { get; }

        /// <summary>
        /// Creates a project.
        /// </summary>
        /// <returns>Created project</returns>
        Project CreateProject();
    }
}
