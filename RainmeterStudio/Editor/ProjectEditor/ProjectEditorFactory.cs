using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Editor.ProjectEditor
{
    /// <summary>
    /// Project editor factory
    /// </summary>
    [PluginExport]
    public class ProjectEditorFactory : IDocumentEditorFactory
    {
        /// <summary>
        /// Creates a new project editor
        /// </summary>
        /// <param name="document">The project document</param>
        /// <returns>Created editor</returns>
        public IDocumentEditor CreateEditor(IDocument document)
        {
            return new ProjectEditorUI((ProjectDocument)document);
        }

        /// <summary>
        /// Checks if this editor can edit documents of given type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>True if the project editor can edit that kind of document</returns>
        public bool CanEdit(Type type)
        {
            return (type.Equals(typeof(ProjectDocument)));
        }
    }
}
