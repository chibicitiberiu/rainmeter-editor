using System.Windows.Media;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.UI.ViewModel
{
    /// <summary>
    /// View model for document templates
    /// </summary>
    public class DocumentTemplateViewModel
    {
        /// <summary>
        /// Gets the document template
        /// </summary>
        public IDocumentTemplate Template { get; private set; }

        /// <summary>
        /// Gets the document template name
        /// </summary>
        public string Name { get { return Template.Name; } }

        /// <summary>
        /// Gets or sets the icon of this document template
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                return ResourceProvider.GetImage("DocumentTemplate_" + Name + "_Icon");
            }
        }

        /// <summary>
        /// Gets or sets the display text
        /// </summary>
        public string DisplayText
        {
            get
            {
                return ResourceProvider.GetString("DocumentTemplate_" + Name + "_DisplayText");
            }
        }

        /// <summary>
        /// Gets or sets the description of this document template
        /// </summary>
        public string Description
        {
            get
            {
                return ResourceProvider.GetString("DocumentTemplate_" + Name + "_Description");
            }
        }

        /// <summary>
        /// Initializes the document template view model
        /// </summary>
        /// <param name="template">The document template</param>
        public DocumentTemplateViewModel(IDocumentTemplate template)
        {
            this.Template = template;
        }
    }
}
