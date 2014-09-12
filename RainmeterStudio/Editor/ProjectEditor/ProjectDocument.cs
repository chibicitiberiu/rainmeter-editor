using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Editor.ProjectEditor
{
    /// <summary>
    /// A project document
    /// </summary>
    /// <remarks>Unlike the Project class, this class implements the 'IDocument' 
    /// interface. This is a proxy class for the actual project.</remarks>
    public class ProjectDocument : IDocument
    {
        private Reference _reference;
        private bool _isDirty = false;

        /// <summary>
        /// Gets or sets the reference of the document
        /// </summary>
        public Reference Reference
        {
            get
            {
                return _reference;
            }
            set
            {
                _reference = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Reference"));
            }
        }

        /// <inheritdoc />
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }

            set
            {
                _isDirty = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsDirty"));
            }
        }

        /// <summary>
        /// Gets the project this project document is linked to
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// Event triggered when a property changes value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes this project document
        /// </summary>
        /// <param name="project">The actual project this document is linked to</param>
        public ProjectDocument(Project project)
        {
            Project = project;
        }
    }
}
