using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainmeterStudio.Core;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Editor.SkinDesigner
{
    [PluginExport]
    public class SkinDocumentEmptyTemplate : IDocumentTemplate
    {
        public string Name
        {
            get { return "Skin"; }
        }

        public string DefaultExtension
        {
            get { return "rsskin"; }
        }

        public IEnumerable<Property> Properties
        {
            get { yield break; }
        }

        public IDocument CreateDocument()
        {
            return new SkinDocument();
        }
    }
}
