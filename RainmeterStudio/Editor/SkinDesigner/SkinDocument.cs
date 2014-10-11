using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Rainmeter;

namespace RainmeterStudio.Editor.SkinDesigner
{
    public class SkinDocument : IDocument
    {
        private Skin _skin;

        /// <summary>
        /// Triggered when the value of a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region IDocument

        private Reference _reference;
        private bool _isDirty;

        /// <summary>
        /// Gets or sets the reference to this document
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
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Reference"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if this document has unsaved changes
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
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("IsDirty"));
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the skin
        /// </summary>
        public Skin Skin
        {
            get
            {
                return _skin;
            }
            set
            {
                _skin = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Skin"));
            }
        }

        /// <summary>
        /// Initializes this skin document
        /// </summary>
        public SkinDocument()
        {
            Skin = new Skin();
        }
    }
}
