using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Core.Editor
{
    /// <summary>
    /// A document editor
    /// </summary>
    public interface IDocumentEditor
    {
        /// <summary>
        /// Gets the document attached to this editor instance
        /// </summary>
        IDocument AttachedDocument { get; }
        
        /// <summary>
        /// Gets the UI control to display for this editor
        /// </summary>
        UIElement EditorUI { get; }
    }
}
