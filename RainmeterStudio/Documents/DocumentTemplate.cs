using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using RainmeterStudio.Model;
using RainmeterStudio.UI.Controller;

namespace RainmeterStudio.Documents
{
    /// <summary>
    /// Represents a document template
    /// </summary>
    public class DocumentTemplate<T> where T : IDocument
    {
        #region Private fields

        private Func<T> _createFunction;

        #endregion

        /// <summary>
        /// Gets the document template name
        /// </summary>
        public string Name { get; private set; }

        #region Icon property

        private ImageSource _icon = null;

        /// <summary>
        /// Gets or sets the template's icon
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
        public virtual string DisplayText
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
        /// Gets or sets the description
        /// </summary>
        public virtual string Description
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

        /// <summary>
        /// Gets or sets the default extension of this template
        /// </summary>
        public string DefaultExtension { get; set; }

        /// <summary>
        /// Gets or sets the category  in which this template belongs
        /// </summary>
        public virtual string Category { get; set; }

        /// <summary>
        /// Initializes the document template
        /// </summary>
        /// <param name="name">Name of document template</param>
        public DocumentTemplate(string name, string defaultExtension = null, string category = null, Func<T> createDocument = null)
        {
            Name = name;
            DefaultExtension = defaultExtension;
            _createFunction = createDocument;
        }

        /// <summary>
        /// Creates a document of type T
        /// </summary>
        /// <returns></returns>
        public virtual T CreateDocument()
        {
            if (_createFunction != null)
                return _createFunction();

            return default(T);
        }
    }
}
