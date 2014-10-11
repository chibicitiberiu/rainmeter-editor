using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RainmeterStudio.Core.Editor.Features;

namespace RainmeterStudio.UI.ViewModel
{
    public class ToolboxItemViewModel
    {
        /// <summary>
        /// Gets the toolbox item
        /// </summary>
        public ToolboxItem Item { get; private set; }

        /// <summary>
        /// Gets the display text of the toolbox item
        /// </summary>
        public string DisplayText
        {
            get
            {
                return Item.Name;
            }
        }

        /// <summary>
        /// Gets the tooltip text of the toolbox item
        /// </summary>
        public string ToolTip
        {
            get
            {
                return "Placeholder tooltip for " + Item.Name;
            }
        }

        /// <summary>
        /// Gets the icon
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes the toolbox item view model
        /// </summary>
        /// <param name="item"></param>
        public ToolboxItemViewModel(ToolboxItem item)
        {
            Item = item;
        }
    }
}
