using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RainmeterStudio.Core.Model
{
    public interface IProperty : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the name of the property
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the value of the property
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Gets the data type of the property
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the children of this property
        /// </summary>
        ObservableCollection<IProperty> Children { get; }
    }

    /// <summary>
    /// Represents a property
    /// </summary>
    public class Property : IProperty
    {
        #region Name property

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public virtual string Name { get; private set; }

        #endregion

        #region Value property

        private object _value;

        /// <summary>
        /// Gets or sets the value of the property
        /// </summary>
        public virtual object Value
        {
            get
            {
                return _value;
            }
            set
            {
                // Test if type changed
                bool typeChanged;
                
                if (_value == null || value == null)
                    typeChanged = (_value != null) || (value != null);
                        
                else
                    typeChanged = (_value.GetType() != value.GetType());

                // Set value
                _value = value;

                // Trigger event
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));

                    if (typeChanged)
                        PropertyChanged(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }

        #endregion

        #region Type property

        /// <summary>
        /// Gets the type of the property
        /// </summary>
        public virtual Type Type
        {
            get
            {
                return Value.GetType();
            }
        }

        #endregion

        #region Children property

        /// <summary>
        /// Gets the children of this property
        /// </summary>
        public ObservableCollection<IProperty> Children { get; private set; }

        #endregion

        #region Property changed event

        /// <summary>
        /// Triggered when a property changes
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this property
        /// </summary>
        /// <param name="name">Name of the property</param>
        public Property(string name)
        {
            Name = name;
            Value = null;
            Children = new ObservableCollection<IProperty>();
        }

        /// <summary>
        /// Initializes this property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Value of the property</param>
        public Property(string name, object value)
        {
            Name = name;
            Value = value;
            Children = new ObservableCollection<IProperty>();
        }

        #endregion
    }

    namespace Generic
    {
        /// <summary>
        /// Generic property
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        public class Property<T> : IProperty
        {
            #region Value property

            private T _value;

            /// <summary>
            /// Gets or sets the value of this property
            /// </summary>
            public T Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    // Set value
                    _value = value;

                    // Trigger event
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }

            /// <summary>
            /// Gets or sets the value of this property. Overriden from the generic property.
            /// </summary>
            /// <exception cref="InvalidCastException">Thrown if value is not of the right type.</exception>
            object IProperty.Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    this.Value = (T)value;
                }
            }

            #endregion

            #region Type property

            /// <summary>
            /// Gets the type of this property
            /// </summary>
            public Type Type
            {
                get
                {
                    return typeof(T);
                }
            }

            #endregion

            #region Property changed event

            /// <summary>
            /// Triggered when a property changes
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes this property
            /// </summary>
            /// <param name="name">Name of the property</param>
            public Property(string name)
            {
                Name = name;
                Value = default(T);
                Children = new ObservableCollection<IProperty>();
            }

            /// <summary>
            /// Initializes this property
            /// </summary>
            /// <param name="name">Name of the property</param>
            /// <param name="value">Value of the property</param>
            public Property(string name, T value)
            {
                Name = name;
                Value = value;
                Children = new ObservableCollection<IProperty>();
            }

            #endregion

            /// <summary>
            /// Gets the name of the property
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the children of this property
            /// </summary>
            public ObservableCollection<IProperty> Children { get; private set; }
        }
    }
}
