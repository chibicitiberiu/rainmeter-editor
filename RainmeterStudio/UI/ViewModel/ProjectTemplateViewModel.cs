using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.UI.ViewModel
{
    /// <summary>
    /// View model for project templates
    /// </summary>
    public class ProjectTemplateViewModel
    {
        /// <summary>
        /// Gets the project template
        /// </summary>
        public IProjectTemplate Template { get; private set; }

        /// <summary>
        /// Gets the name of the template
        /// </summary>
        public string Name
        {
            get
            {
                return Template.Name;
            }
        }

        /// <summary>
        /// Gets the display text
        /// </summary>
        public string DisplayText
        {
            get
            {
                return ResourceProvider.GetString("ProjectTemplate_" + Name + "_DisplayText");
            }
        }

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description
        {
            get
            {
                return ResourceProvider.GetString("ProjectTemplate_" + Name + "_Description");
            }
        }
        
        /// <summary>
        /// Gets the icon
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                return ResourceProvider.GetImage("ProjectTemplate_" + Name + "_Icon");
            }
        }

        /// <summary>
        /// Initializes the project template view model
        /// </summary>
        /// <param name="template">A project template</param>
        public ProjectTemplateViewModel(IProjectTemplate template)
        {
            Template = template;
        }
    }
}
