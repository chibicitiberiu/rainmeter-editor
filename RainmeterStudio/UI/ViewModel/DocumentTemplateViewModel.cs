using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.UI.Controller;

namespace RainmeterStudio.UI.ViewModel
{
    public class DocumentTemplateViewModel
    {
        /// <summary>
        /// Gets the document template
        /// </summary>
        public DocumentTemplate Template { get; private set; }

        /// <summary>
        /// Gets the document template name
        /// </summary>
        public string Name { get { return Template.Name; } }

        #region Icon property

        private ImageSource _icon = null;

        /// <summary>
        /// Gets or sets the icon of this document template
        /// </summary>
        public virtual ImageSource Icon
        {
            get
            {
                if (_icon == null)
                    return IconProvider.GetIcon("Template_" + Name);

                return _icon;
            }
            set
            {
                _icon = value;
            }
        }

        #endregion

        #region Display text property

        private string _displayText = null;

        /// <summary>
        /// Gets or sets the display text
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (_displayText == null)
                    return Resources.Strings.ResourceManager.GetString("Template_" + Name + "_DisplayText");

                return _displayText;
            }
            set
            {
                _displayText = value;
            }
        }

        #endregion

        #region Description property

        private string _description = null;

        /// <summary>
        /// Gets or sets the description of this document template
        /// </summary>
        public string Description
        {
            get
            {
                if (_description == null)
                    return Resources.Strings.ResourceManager.GetString("Template_" + Name + "_Description");

                return _description;
            }
            set
            {
                _description = value;
            }
        }

        #endregion

        #region Category property

        private string _category = null;

        /// <summary>
        /// Gets or sets the category of this template
        /// </summary>
        public string Category
        {
            get
            {
                if (_category == null)
                    return Resources.Strings.ResourceManager.GetString("Template_" + Name + "_Category");

                return _category;
            }
            set
            {
                _category = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the document template view model
        /// </summary>
        /// <param name="template">The document template</param>
        public DocumentTemplateViewModel(DocumentTemplate template)
        {
            this.Template = template;
        }
    }
}
