using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.SkinDesignerPlugin
{
    /// <summary>
    /// Skin document
    /// </summary>
    public class SkinDocument : IDocument
    {
        private Reference _reference;

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

        public bool IsDirty { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SkinMetadata Metadata { get; private set; }

        /// <summary>
        /// Initializes this skin document
        /// </summary>
        public SkinDocument()
        {
            IsDirty = false;
            Metadata = new SkinMetadata();
        }
    }
}
