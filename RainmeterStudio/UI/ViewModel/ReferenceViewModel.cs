using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.UI.ViewModel
{
    /// <summary>
    /// Contains the view model of a reference
    /// </summary>
    public class ReferenceViewModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        private List<ReferenceViewModel> _children = null;

        #region Properties

        /// <summary>
        /// Gets the linked reference
        /// </summary>
        public Reference Reference { get; private set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get
            {
                return Reference.Name;
            }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        public string Path
        {
            get
            {
                return Reference.StoragePath;
            }
        }

        /// <summary>
        /// Gets an enumerable of this object's children
        /// </summary>
        public IEnumerable<ReferenceViewModel> Children
        {
            get
            {
                return _children;
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
        public ReferenceViewModel(Reference reference)
        {
            Reference = reference;
            Reference.CollectionChanged += Reference_CollectionChanged;
            UpdateChildren();
        }

        void Reference_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<ReferenceViewModel> newItems = new List<ReferenceViewModel>();
            List<ReferenceViewModel> oldItems = new List<ReferenceViewModel>();

            // Update collection
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    newItems = e.NewItems.Cast<Reference>().Select(x => new ReferenceViewModel(x)).ToList();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    oldItems = _children.Where(x => e.OldItems.Contains(x.Reference)).ToList();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    newItems = e.NewItems.Cast<Reference>().Select(x => new ReferenceViewModel(x)).ToList();
                    oldItems = _children.Where(x => e.OldItems.Contains(x.Reference)).ToList();
                    break;

                default:
                    UpdateChildren();
                    break;
            }

            _children.RemoveAll(oldItems.Contains);
            _children.AddRange(newItems);

            // Pass event
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(e.Action, newItems, oldItems));
        }

        #endregion

        #region Operations

        /// <summary>
        /// Sets the 'IsExpanded' property for the entire subtree
        /// </summary>
        /// <param name="value">Value to set</param>
        public void TreeExpand(bool value)
        {
            IsExpanded = value;
        }

        private void UpdateChildren()
        {
            _children = Reference.Children.Select(x => new ReferenceViewModel(x)).ToList();
        }

        #endregion

        /// <summary>
        /// Triggered when the linked reference collection changes
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
