using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Core.Utils;

namespace RainmeterStudio.Core.Model
{
    /// <summary>
    /// The kind of item the reference points to
    /// </summary>
    public enum ReferenceTargetKind
    {
        /// <summary>
        /// Invalid state
        /// </summary>
        None,

        /// <summary>
        /// Reference points to a file
        /// </summary>
        File,

        /// <summary>
        /// Reference points to a directory
        /// </summary>
        Directory,

        /// <summary>
        /// Reference points to a project
        /// </summary>
        Project
    }

    /// <summary>
    /// Reference to a file or folder
    /// </summary>
    [DebuggerDisplay("QualifiedName = {QualifiedName}, StoragePath = {StoragePath}")]
    public class Reference : INotifyCollectionChanged, INotifyPropertyChanged, ICloneable
    {
        private Dictionary<string, Reference> _children;
        private Reference _parent;
        private string _name, _storagePath;
        private ReferenceTargetKind _kind;
        
        #region Events

        /// <summary>
        /// Triggered when children are added or removed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Triggered when a property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the parent of this reference
        /// </summary>
        [XmlIgnore]
        public Reference Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                // Unsubscribe from old parent
                if (_parent != null)
                    _parent.PropertyChanged -= Parent_PropertyChanged;

                // Set new parent
                _parent = value;

                // Subscribe to new parent
                if (_parent != null)
                    _parent.PropertyChanged += Parent_PropertyChanged;

                // Notify
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Parent"));
                    PropertyChanged(this, new PropertyChangedEventArgs("QualifiedName"));
                }
            }
        }

        /// <summary>
        /// Gets the children references
        /// </summary>
        [XmlIgnore]
        public ReadOnlyDictionary<string, Reference> ChildrenDictionary
        {
            get
            {
                return new ReadOnlyDictionary<string, Reference>(_children);
            }
        }

        /// <summary>
        /// Gets or sets children
        /// </summary>
        [XmlArray("children")]
        public Reference[] Children
        {
            get
            {
                return _children.Values.ToArray();
            }
            set
            {
                Clear();
                value.ForEach(Add);
            }
        }

        /// <summary>
        /// Gets the name of the reference
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                    PropertyChanged(this, new PropertyChangedEventArgs("QualifiedName"));
                }
            }
        }

        /// <summary>
        /// Gets the full qualified name of this reference
        /// </summary>
        [XmlIgnore]
        public string QualifiedName
        {
            get
            {
                if (Parent == null)
                {
                    // Return name
                    return Name;
                }
                else
                {
                    // If it has a parent, get the parent's name
                    return Parent.QualifiedName + '/' + Name;
                }
            }
        }

        /// <summary>
        /// Gets the parts of the full qualified name of this reference
        /// </summary>
        [XmlIgnore]
        public IEnumerable<string> QualifiedParts
        {
            get
            {
                if (Parent == null)
                {
                    return Enumerable.Repeat(Name, 1);
                }
                else
                {
                    return Parent.QualifiedParts.Append(Name);
                }
            }
        }

        /// <summary>
        /// Gets the path to the file on the disk. If reference is in a project, the path should be relative.
        /// </summary>
        [XmlAttribute("storagePath")]
        public string StoragePath
        {
            get
            {
                return _storagePath;
            }
            set
            {
                _storagePath = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("StoragePath"));
            }
        }

        /// <summary>
        /// Gets the target kind
        /// </summary>
        [XmlAttribute("targetKind")]
        public ReferenceTargetKind TargetKind
        {
            get
            {
                return _kind;
            }
            set
            {
                _kind = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TargetKind"));
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the reference as a file reference
        /// </summary>
        public Reference()
            : this(null, null, ReferenceTargetKind.File)
        {
        }

        /// <summary>
        /// Initializes the reference
        /// </summary>
        /// <param name="name">Name of this reference</param>
        /// <param name="kind">Reference kind</param>
        public Reference(string name, ReferenceTargetKind kind)
            : this(name, null, kind)
        {
        }

        /// <summary>
        /// Initializes the reference.
        /// Kind is inferred by testing the file on disk.
        /// </summary>
        /// <param name="name">Name of reference</param>
        /// <param name="storagePath">Path to item on disk</param>
        public Reference(string name, string storagePath)
            : this(name, storagePath, InferKind(storagePath))
        {
        }

        /// <summary>
        /// Initializes the reference
        /// </summary>
        /// <param name="name">Name of reference</param>
        /// <param name="storagePath">Path to item on disk</param>
        /// <param name="kind">Reference kind</param>
        public Reference(string name, string storagePath, ReferenceTargetKind kind)
        {
            StoragePath = storagePath;
            Name = name;
            TargetKind = kind;
            _children = new Dictionary<string, Reference>();
        }

        #endregion

        #region Exists

        /// <summary>
        /// Checks if the file exists
        /// </summary>
        /// <returns></returns>
        public bool ExistsOnStorage()
        {
            if (StoragePath != null)
            {
                return File.Exists(StoragePath) || Directory.Exists(StoragePath);
            }

            return false;
        }

        #endregion

        #region Children/parenting operations

        /// <summary>
        /// Adds a child reference
        /// </summary>
        /// <param name="reference"></param>
        public void Add(Reference reference)
        {
            // Make sure object is not parented yet
            if (reference.Parent != null)
                throw new ArgumentException("Reference must be removed from its current parent first.");

            // Add and parent
            _children.Add(reference.Name, reference);
            reference.Parent = this;

            // Trigger event
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, reference));
        }

        /// <summary>
        /// Removes a reference
        /// </summary>
        /// <param name="reference">Reference to remove</param>
        /// <returns>True if removed successfully</returns>
        public bool Remove(Reference reference)
        {
            // Make sure we are the parent
            if (reference.Parent != this)
                return false;

            // Remove
            reference.Parent = null;
            bool res = _children.Remove(reference.Name);

            // Trigger event
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, reference));

            return res;
        }

        /// <summary>
        /// Removes this reference from its parent
        /// </summary>
        /// <returns>True if unparented successfully</returns>
        public bool Unparent()
        {
            if (Parent != null)
                return Parent.Remove(this);

            return false;
        }

        /// <summary>
        /// Removes all children
        /// </summary>
        public void Clear()
        {
            // Unparent
            foreach (var pair in _children)
            {
                pair.Value.Parent = null;
            }

            // Clear
            _children.Clear();

            // Trigger event
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Gets the number of children
        /// </summary>
        public int Count
        {
            get
            {
                return _children.Count;
            }
        }

        private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null && e.PropertyName == "QualifiedName")
                PropertyChanged(this, new PropertyChangedEventArgs("QualifiedName"));
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Compares a reference to another objects
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>True if objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Reference)
            {
                Reference other = (Reference)obj;
                return (String.Equals(QualifiedName, other.QualifiedName));
            }

            return false;
        }

        /// <summary>
        /// Obtains the hash code of this reference
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return QualifiedName.GetHashCode();
        }

        /// <summary>
        /// Gets the string representation of this reference
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return QualifiedName;
        }

        #endregion

        #region Helper methods

        private static ReferenceTargetKind InferKind(string storagePath)
        {
            ReferenceTargetKind kind = ReferenceTargetKind.None;

            if (Path.GetExtension(storagePath) == ".rsproj")
                kind = ReferenceTargetKind.Project;

            else if (File.Exists(storagePath))
                kind = ReferenceTargetKind.File;

            else if (Directory.Exists(storagePath))
                kind = ReferenceTargetKind.Directory;

            return kind;
        }

        #endregion
        
        /// <summary>
        /// Creates a clone of this reference
        /// </summary>
        /// <returns>The clone</returns>
        /// <remarks>The clone doesn't keep the parent.</remarks>
        public object Clone()
        {
            var cloneReference = new Reference(Name, StoragePath, TargetKind);

            foreach (var r in Children)
            {
                cloneReference.Add((Reference)r.Clone());
            }

            return cloneReference;
        }
    }

    /// <summary>
    /// Provides useful methods for references
    /// </summary>
    public static class ReferenceExtensions
    {
        /// <summary>
        /// Tries to get a reference from the same tree having specified qualified name
        /// </summary>
        /// <param name="this">Reference contained in the tree</param>
        /// <param name="qualifiedName">Full qualified name</param>
        /// <param name="output">Found reference</param>
        /// <returns>True if succeeded to find the reference</returns>
        public static bool TryGetReference(this Reference @this, string qualifiedName, out Reference output)
        {
            var thisQualifiedName = @this.QualifiedName;

            // Am I the reference? return myself
            if (qualifiedName.Equals(thisQualifiedName))
            {
                output = @this;
                return true;
            }

            // Qualified name is a child, look child up
            else if (qualifiedName.StartsWith(thisQualifiedName))
            {
                int startIndex = thisQualifiedName.Length + 1;
                int endIndex = qualifiedName.IndexOf('/', startIndex);

                string child;
                Reference childRef;

                if (endIndex < 0)
                {
                    child = qualifiedName.Substring(startIndex);
                }
                else
                {
                    child = qualifiedName.Substring(startIndex, endIndex - startIndex);
                }

                // Try to get child
                if (@this.ChildrenDictionary.TryGetValue(child, out childRef))
                {
                    return childRef.TryGetReference(qualifiedName, out output);
                }
            }

            // Qualified name is not a child and not 'this', so ask parent to find it
            else if (@this.Parent != null)
            {
                return @this.Parent.TryGetReference(qualifiedName, out output);
            }

            // Failed to find child
            output = null;
            return false;
        }

        /// <summary>
        /// Gets a reference from the same tree having specified qualified name
        /// </summary>
        /// <param name="this">Reference contained in the tree</param>
        /// <param name="qualifiedName">Full qualified name</param>
        /// <returns>Found reference</returns>
        /// <exception cref="ArgumentException">If qualified name not found</exception>
        public static Reference GetReference(this Reference @this, string qualifiedName)
        {
            Reference res;

            if (TryGetReference(@this, qualifiedName, out res))
            {
                return res;
            }
            else
            {
                throw new ArgumentException("Could not find reference.");
            }
        }

        /// <summary>
        /// Checks if a reference is in the same tree as this
        /// </summary>
        /// <param name="this">Reference that is in the tree we search in</param>
        /// <param name="other">Reference to search</param>
        /// <returns>True if the tree contains the reference.</returns>
        public static bool TreeContains(this Reference @this, Reference reference)
        {
            Reference temp;
            return TryGetReference(@this, reference.QualifiedName, out temp);
        }
    }
}
