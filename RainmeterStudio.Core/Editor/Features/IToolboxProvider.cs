using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Core.Editor.Features
{
    public interface IToolboxProvider
    {
        bool SupportsToolboxDrop { get; }
        IEnumerable<string> ToolboxItems { get; }

        event EventHandler ToolboxItemsChanged;
    }
}
