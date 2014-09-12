using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Core.Editor.Features
{
    interface ISelectionPropertiesProvider
    {
        string SelectionName { get; }
        IEnumerable<Property> SelectionProperties { get; }

        event EventHandler SelectionChanged;
    }
}
