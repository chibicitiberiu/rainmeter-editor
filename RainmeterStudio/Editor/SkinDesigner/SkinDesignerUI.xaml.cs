using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Editor.Features;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Editor.SkinDesigner
{
    /// <summary>
    /// Interaction logic for SkinDesignerUI.xaml
    /// </summary>
    public partial class SkinDesignerUI : UserControl, IDocumentEditor, IToolboxProvider
    {
        private SkinDocument _document;

        #region IDocumentEditor

        /// <summary>
        /// Gets the attached document
        /// </summary>
        public IDocument AttachedDocument
        {
            get { return _document; }
        }

        /// <summary>
        /// Gets the Editor UI
        /// </summary>
        public UIElement EditorUI
        {
            get { return this; }
        }

        #endregion

        /// <summary>
        /// Gets or sets the attached skin document
        /// </summary>
        public SkinDocument Document
        {
            get
            {
                return _document;
            }
            protected set
            {
                _document = value;

                Reset();
            }
        }

        /// <summary>
        /// Initializes the skin designer UI
        /// </summary>
        /// <param name="document">Skin document to be edited</param>
        public SkinDesignerUI(SkinDocument document)
        {
            InitializeComponent();

            Document = document;
        }

        /// <summary>
        /// Reloads everything
        /// </summary>
        private void Reset()
        {
            meterControl.Skin = Document.Skin;
        }

        public IEnumerable<ToolboxItem> ToolboxItems
        {
            get 
            {
                yield return new ToolboxItem("Item 1");
                yield return new ToolboxItem("Item 2");
                yield return new ToolboxItem("Item 3");
                yield return new ToolboxItem("Item 4");
            }
        }

        public event EventHandler ToolboxItemsChanged;

        public void ToolboxItemDrop(ToolboxItem item)
        {
            
        }
    }
}
