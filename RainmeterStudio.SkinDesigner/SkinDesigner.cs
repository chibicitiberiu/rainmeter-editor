using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Editor.Features;

namespace RainmeterStudio.SkinDesignerPlugin
{
    /// <summary>
    /// Skin designer document editor
    /// </summary>
    public class SkinDesigner : IDocumentEditor //TODO: , ISelectionPropertiesProvider, IToolboxProvider, IUndoSupport
    {
        /// <summary>
        /// Gets the document attached to this editor instance
        /// </summary>
        public SkinDocument AttachedDocument { get; private set; }

        /// <summary>
        /// Gets the document attached to this editor instance
        /// </summary>
        IDocument IDocumentEditor.AttachedDocument
        {
            get { return AttachedDocument; }
        }

        /// <summary>
        /// Gets the UI control to display for this editor
        /// </summary>
        public SkinDesignerControl EditorUI { get; private set; }

        /// <summary>
        /// Gets the UI control to display for this editor
        /// </summary>
        UIElement IDocumentEditor.EditorUI
        {
            get { return EditorUI; }
        }

        /// <summary>
        /// Initializes this editor
        /// </summary>
        /// <param name="document">The document</param>
        public SkinDesigner(SkinDocument document)
        {
            AttachedDocument = document;
            EditorUI = new SkinDesignerControl(document);
        }
    }
}
