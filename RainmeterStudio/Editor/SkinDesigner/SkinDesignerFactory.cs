using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Editor.SkinDesigner
{
    /// <summary>
    /// Creates skin designers
    /// </summary>
    [PluginExport]
    public class SkinDesignerFactory : IDocumentEditorFactory
    {
        /// <inheritdoc />
        public IDocumentEditor CreateEditor(IDocument document)
        {
            return new SkinDesignerUI((SkinDocument)document);
        }

        /// <inheritdoc />
        public bool CanEdit(Type type)
        {
            return type == typeof(SkinDocument);
        }
    }
}
