using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Core
{
    /// <summary>
    /// The assembly will be loaded as a plug-in.
    /// </summary>
    /// <remarks>RainmeterStudio will load assemblies with this flag as plugins.</remarks>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RainmeterStudioPluginAttribute : Attribute
    {
    }
}
