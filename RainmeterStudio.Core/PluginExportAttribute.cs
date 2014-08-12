using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Core
{
    /// <summary>
    /// Exports a class
    /// </summary>
    /// <remarks>
    /// This attribute should be used to export factories, templates etc.
    /// If not used, these will have to be manually exported.
    /// The class marked with this flag must have a default constructor!
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginExportAttribute : Attribute
    {
    }
}
