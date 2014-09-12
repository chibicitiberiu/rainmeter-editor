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
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Editor.ProjectEditor
{
    /// <summary>
    /// Interaction logic for ProjectEditorUI.xaml
    /// </summary>
    public partial class ProjectEditorUI : UserControl, IDocumentEditor
    {
        /// <summary>
        /// Gets the attached project document being edited
        /// </summary>
        public ProjectDocument Document { get; private set; }

        /// <summary>
        /// Gets the attached document
        /// </summary>
        public IDocument AttachedDocument
        {
            get { return Document; }
        }

        /// <summary>
        /// Gets the UI element to be displayed in the document window
        /// </summary>
        public UIElement EditorUI
        {
            get { return this; }
        }

        /// <summary>
        /// Initializes this project editor UI
        /// </summary>
        /// <param name="document"></param>
        public ProjectEditorUI(ProjectDocument document)
        {
            InitializeComponent();
            Document = document;
            Document.Project.PropertyChanged += Project_PropertyChanged;
            DataContext = Document.Project;
        }

        private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Document.IsDirty = true;
        }
    }
}
