using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Model;

namespace RainmeterStudio.Documents.DocumentEditorFeatures
{
    interface ISelectionPropertiesProvider
    {
        string SelectionName { get; }
        IEnumerable<Property> SelectionProperties { get; }

        event EventHandler SelectionChanged;
    }
}
