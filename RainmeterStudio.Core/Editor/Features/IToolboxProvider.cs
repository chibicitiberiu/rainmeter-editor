using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RainmeterStudio.Core.Editor.Features
{
    /// <summary>
    /// Editor provides toolbox items
    /// </summary>
    public interface IToolboxProvider
    {
        /// <summary>
        /// Gets a list of toolbox items
        /// </summary>
        IEnumerable<ToolboxItem> ToolboxItems { get; }

        /// <summary>
        /// Triggered if the toolbox items change
        /// </summary>
        event EventHandler ToolboxItemsChanged;

        /// <summary>
        /// Called when a toolbar item is dropped by double-click
        /// </summary>
        /// <param name="item">Toolbox item dropped</param>
        void ToolboxItemDrop(ToolboxItem item);
    }
}
