using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RainmeterStudio.Model
{
    public abstract class IDocumentEditor : IDisposable
    {
        #region Dirty flag

        private bool _dirty = false;

        /// <summary>
        /// Gets a flag indicating if the active document is dirty (modified and not saved)
        /// </summary>
        public virtual bool Dirty
        {
            get
            {
                return _dirty;
            }
            protected set
            {
                _dirty = value;
                if (DirtyChanged != null)
                    DirtyChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Triggered when the dirty flag changes
        /// </summary>
        public event EventHandler DirtyChanged;

        #endregion

        #region Document

        /// <summary>
        /// Gets the opened document
        /// </summary>
        public abstract IDocument Document { get; }

        /// <summary>
        /// Gets the title to be displayed in the title bar
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Triggered when the title changes
        /// </summary>
        public event EventHandler TitleChanged;

        #endregion

        #region EditorUI

        /// <summary>
        /// Gets the editor UI
        /// </summary>
        public abstract UIElement EditorUI { get; }

        #endregion

        #region Selection properties

        /// <summary>
        /// Gets a value indicating if this editor uses the selection properties window
        /// </summary>
        public virtual bool UsesSelectionProperties
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of the selected object
        /// </summary>
        public virtual string SelectionName
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets a list of properties for the currently selected object
        /// </summary>
        public virtual IEnumerable<string> SelectionProperties
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Triggered when the selected object changes (used to update properties)
        /// </summary>
        public event EventHandler SelectionChanged;

        #endregion

        #region Toolbox

        /// <summary>
        /// Gets a list of items to populate the toolbox
        /// </summary>
        public virtual IEnumerable<string> ToolboxItems
        {
            get
            {
                yield break;
            }
        }

        public event EventHandler ToolboxChanged;

        #endregion

        #region Dispose
        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
