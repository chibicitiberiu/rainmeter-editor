using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.TextEditorPlugin
{
    public class TextDocument : IDocument
    {
        private Reference _reference;
        private bool _isDirty;

        /// <summary>
        /// Gets or sets the text associated with this document
        /// </summary>
        public List<string> Lines
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the reference of this document
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

        /// <summary>
        /// Gets a property indicating if this file was modified and not saved
        /// </summary>
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
        /// Triggered when the value of a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes the text document
        /// </summary>
        public TextDocument()
        {
            Lines = new List<string>();
        }
    }
}
