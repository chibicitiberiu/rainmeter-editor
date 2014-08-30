using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.UI.ViewModel
{
    /// <summary>
    /// Contains the view model of a reference
    /// </summary>
    public class ReferenceViewModel : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Gets the linked reference
        /// </summary>
        public Tree<Reference> Reference { get; private set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get
            {
                return Reference.Data.Name;
            }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        public string Path
        {
            get
            {
                return Reference.Data.StoragePath;
            }
        }


        private bool _isExpanded = true;

        /// <summary>
        /// Gets or sets a property indicating if the tree view item is expanded
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                _isExpanded = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
            }
        }

        private bool _isSelected;

        /// <summary>
        /// Gets or sets a property indicating if the tree view item is selected
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Event triggered when a property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of reference view model
        /// </summary>
        /// <param name="reference">Reference</param>
        public ReferenceViewModel(Tree<Reference> reference)
        {
            Reference = reference;
        }

        #endregion
    }
}
